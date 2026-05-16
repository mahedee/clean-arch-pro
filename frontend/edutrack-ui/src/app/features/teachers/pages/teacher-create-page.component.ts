import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { TeacherService } from '../services/teacher.service';
import { AcademicTitle } from '../../../shared/interfaces';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-teacher-create-page',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSnackBarModule,
    PageHeaderComponent,
    LoadingSpinnerComponent
  ],
  template: `
    <app-page-header [title]="isEditMode ? 'Edit Teacher' : 'Create Teacher'">
      <button mat-button routerLink="/teachers">
        <mat-icon>arrow_back</mat-icon>
        Back to Teachers
      </button>
    </app-page-header>

    <mat-card>
      <mat-card-content>
        <app-loading-spinner *ngIf="isLoading" [message]="isEditMode ? 'Updating teacher...' : 'Creating teacher...'" [overlay]="true"></app-loading-spinner>

        <form [formGroup]="teacherForm" (ngSubmit)="onSubmit()" class="teacher-form">
          <div class="form-section">
            <h3>Personal Information</h3>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Full Name</mat-label>
              <input matInput formControlName="fullName" placeholder="Enter full name">
              <mat-error *ngIf="teacherForm.get('fullName')?.hasError('required')">Full name is required</mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Email</mat-label>
              <input matInput formControlName="email" placeholder="Enter email address">
              <mat-error *ngIf="teacherForm.get('email')?.hasError('required')">Email is required</mat-error>
              <mat-error *ngIf="teacherForm.get('email')?.hasError('email')">Enter a valid email</mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Phone Number</mat-label>
              <input matInput formControlName="phoneNumber" placeholder="Enter phone number (optional)">
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width" *ngIf="!isEditMode">
              <mat-label>Date of Birth</mat-label>
              <input matInput [matDatepicker]="picker" formControlName="dateOfBirth">
              <mat-datepicker-toggle matIconSuffix [for]="picker"></mat-datepicker-toggle>
              <mat-datepicker #picker></mat-datepicker>
              <mat-error *ngIf="teacherForm.get('dateOfBirth')?.hasError('required')">Date of birth is required</mat-error>
            </mat-form-field>
          </div>

          <div class="form-section">
            <h3>Academic Information</h3>

            <mat-form-field appearance="outline" class="full-width" *ngIf="!isEditMode">
              <mat-label>Employee ID</mat-label>
              <input matInput formControlName="employeeId" placeholder="Enter employee ID">
              <mat-error *ngIf="teacherForm.get('employeeId')?.hasError('required')">Employee ID is required</mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Department</mat-label>
              <input matInput formControlName="department" placeholder="Enter department">
              <mat-error *ngIf="teacherForm.get('department')?.hasError('required')">Department is required</mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Academic Title</mat-label>
              <mat-select formControlName="title">
                <mat-option *ngFor="let t of academicTitles" [value]="t">{{t}}</mat-option>
              </mat-select>
              <mat-error *ngIf="teacherForm.get('title')?.hasError('required')">Academic title is required</mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Office Location</mat-label>
              <input matInput formControlName="officeLocation" placeholder="Office location (optional)">
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Office Hours</mat-label>
              <input matInput formControlName="officeHours" placeholder="e.g. Mon/Wed 2-4pm (optional)">
            </mat-form-field>
          </div>

          <div class="form-actions">
            <button mat-button type="button" routerLink="/teachers">Cancel</button>
            <button mat-raised-button color="primary" type="submit" [disabled]="teacherForm.invalid || isLoading">
              {{isEditMode ? 'Update Teacher' : 'Create Teacher'}}
            </button>
          </div>
        </form>
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .teacher-form { max-width: 800px; margin: 0 auto; }
    .form-section { margin-bottom: 32px; }
    .form-section h3 { margin-bottom: 16px; font-size: 1.1rem; font-weight: 600; color: #333; }
    .full-width { width: 100%; }
    .form-actions { display: flex; gap: 12px; justify-content: flex-end; margin-top: 16px; }
  `]
})
export class TeacherCreatePageComponent implements OnInit {
  teacherForm!: FormGroup;
  isLoading = false;
  isEditMode = false;
  teacherId = '';
  academicTitles = Object.values(AcademicTitle);

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private teacherService: TeacherService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.teacherId = this.route.snapshot.paramMap.get('id') || '';
    this.isEditMode = !!this.teacherId && this.route.snapshot.url.some(s => s.path === 'edit');
    this.buildForm();

    if (this.isEditMode) {
      this.loadTeacher();
    }
  }

  buildForm(): void {
    this.teacherForm = this.fb.group({
      fullName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: [''],
      dateOfBirth: [null, this.isEditMode ? [] : [Validators.required]],
      employeeId: [{ value: '', disabled: this.isEditMode }, this.isEditMode ? [] : [Validators.required]],
      department: ['', [Validators.required]],
      title: ['', [Validators.required]],
      officeLocation: [''],
      officeHours: ['']
    });
  }

  loadTeacher(): void {
    this.isLoading = true;
    this.teacherService.getTeacher(this.teacherId).subscribe({
      next: (t) => {
        this.teacherForm.patchValue({
          fullName: t.fullName,
          email: t.email,
          phoneNumber: t.phoneNumber || '',
          department: t.department,
          title: t.title,
          officeLocation: t.officeLocation || '',
          officeHours: t.officeHours || ''
        });
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.snackBar.open('Failed to load teacher.', 'Close', { duration: 3000 });
      }
    });
  }

  onSubmit(): void {
    if (this.teacherForm.invalid) return;
    this.isLoading = true;
    const value = this.teacherForm.getRawValue();

    if (this.isEditMode) {
      this.teacherService.updateTeacher(this.teacherId, {
        fullName: value.fullName,
        email: value.email,
        phoneNumber: value.phoneNumber || undefined,
        department: value.department,
        title: value.title,
        officeLocation: value.officeLocation || undefined,
        officeHours: value.officeHours || undefined
      }).subscribe({
        next: () => {
          this.isLoading = false;
          this.snackBar.open('Teacher updated successfully.', 'Close', { duration: 3000 });
          this.router.navigate(['/teachers', this.teacherId]);
        },
        error: () => {
          this.isLoading = false;
          this.snackBar.open('Failed to update teacher.', 'Close', { duration: 3000 });
        }
      });
    } else {
      this.teacherService.createTeacher({
        fullName: value.fullName,
        email: value.email,
        phoneNumber: value.phoneNumber || undefined,
        employeeId: value.employeeId,
        department: value.department,
        title: value.title,
        dateOfBirth: value.dateOfBirth instanceof Date ? value.dateOfBirth.toISOString() : value.dateOfBirth
      }).subscribe({
        next: (id) => {
          this.isLoading = false;
          this.snackBar.open('Teacher created successfully.', 'Close', { duration: 3000 });
          this.router.navigate(['/teachers', id]);
        },
        error: () => {
          this.isLoading = false;
          this.snackBar.open('Failed to create teacher.', 'Close', { duration: 3000 });
        }
      });
    }
  }
}
