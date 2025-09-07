import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';

import { StudentService } from '../../../core/services/student.service';
import { CreateStudentDto, UpdateStudentDto, Student } from '../../../shared/models/student.model';

@Component({
  selector: 'app-student-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatIconModule
  ],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>{{ isEditMode ? 'Edit Student' : 'Create Student' }}</mat-card-title>
      </mat-card-header>

      <mat-card-content>
        <form [formGroup]="studentForm" (ngSubmit)="onSubmit()">
          <!-- Personal Information -->
          <div class="form-section">
            <h3>Personal Information</h3>
            
            <mat-form-field appearance="outline">
              <mat-label>Full Name</mat-label>
              <input matInput formControlName="fullName" required>
              <mat-error *ngIf="studentForm.get('fullName')?.hasError('required')">
                Full name is required
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Date of Birth</mat-label>
              <input matInput [matDatepicker]="picker" formControlName="dateOfBirth" required>
              <mat-datepicker-toggle matIconSuffix [for]="picker"></mat-datepicker-toggle>
              <mat-datepicker #picker></mat-datepicker>
              <mat-error *ngIf="studentForm.get('dateOfBirth')?.hasError('required')">
                Date of birth is required
              </mat-error>
            </mat-form-field>
          </div>

          <!-- Contact Information -->
          <div class="form-section">
            <h3>Contact Information</h3>
            
            <mat-form-field appearance="outline">
              <mat-label>Email</mat-label>
              <input matInput type="email" formControlName="email" required>
              <mat-error *ngIf="studentForm.get('email')?.hasError('required')">
                Email is required
              </mat-error>
              <mat-error *ngIf="studentForm.get('email')?.hasError('email')">
                Please enter a valid email
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Phone Number</mat-label>
              <input matInput formControlName="phoneNumber" required>
              <mat-hint>Format: +1-555-123-4567</mat-hint>
              <mat-error *ngIf="studentForm.get('phoneNumber')?.hasError('required')">
                Phone number is required
              </mat-error>
            </mat-form-field>
          </div>

          <!-- Address Information -->
          <div class="form-section">
            <h3>Address Information</h3>
            
            <div formGroupName="address">
              <mat-form-field appearance="outline">
                <mat-label>Street</mat-label>
                <input matInput formControlName="street">
              </mat-form-field>

              <div class="address-row">
                <mat-form-field appearance="outline">
                  <mat-label>City</mat-label>
                  <input matInput formControlName="city">
                </mat-form-field>

                <mat-form-field appearance="outline">
                  <mat-label>State/Province</mat-label>
                  <input matInput formControlName="state">
                </mat-form-field>
              </div>

              <div class="address-row">
                <mat-form-field appearance="outline">
                  <mat-label>Zip Code</mat-label>
                  <input matInput formControlName="zipCode">
                  <mat-hint>Format: 12345 or 12345-6789</mat-hint>
                </mat-form-field>

                <mat-form-field appearance="outline">
                  <mat-label>Country</mat-label>
                  <input matInput formControlName="country">
                </mat-form-field>
              </div>
            </div>
          </div>

          <!-- GPA (only for edit mode) -->
          <div class="form-section" *ngIf="isEditMode">
            <h3>Academic Information</h3>
            
            <mat-form-field appearance="outline">
              <mat-label>GPA</mat-label>
              <input matInput type="number" step="0.01" min="0" max="4" formControlName="gpa">
              <mat-hint>Enter GPA (0.00 - 4.00)</mat-hint>
            </mat-form-field>
          </div>

          <!-- Form Actions -->
          <div class="form-actions">
            <button mat-button type="button" (click)="onCancel()">Cancel</button>
            <button mat-raised-button color="primary" type="submit" [disabled]="studentForm.invalid || isSubmitting">
              {{ isSubmitting ? 'Saving...' : (isEditMode ? 'Update' : 'Create') }}
            </button>
          </div>
        </form>
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .form-section {
      margin-bottom: 24px;
    }

    .form-section h3 {
      margin-bottom: 16px;
      color: #333;
      font-weight: 500;
    }

    mat-form-field {
      width: 100%;
      margin-bottom: 16px;
    }

    .address-row {
      display: flex;
      gap: 16px;
    }

    .address-row mat-form-field {
      flex: 1;
    }

    .form-actions {
      display: flex;
      gap: 16px;
      justify-content: flex-end;
      margin-top: 24px;
    }

    mat-card {
      max-width: 600px;
      margin: 0 auto;
    }
  `]
})
export class StudentFormComponent implements OnInit {
  studentForm: FormGroup;
  isEditMode = false;
  isSubmitting = false;
  studentId?: string;

  constructor(
    private fb: FormBuilder,
    private studentService: StudentService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.studentForm = this.createForm();
  }

  ngOnInit(): void {
    this.studentId = this.route.snapshot.params['id'];
    this.isEditMode = !!this.studentId;

    if (this.isEditMode && this.studentId) {
      this.loadStudent();
    }
  }

  private createForm(): FormGroup {
    return this.fb.group({
      fullName: ['', [Validators.required]],
      dateOfBirth: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required]],
      gpa: [null],
      address: this.fb.group({
        street: [''],
        city: [''],
        state: [''],
        zipCode: [''],
        country: ['']
      })
    });
  }

  private loadStudent(): void {
    if (!this.studentId) return;

    this.studentService.getStudent(this.studentId).subscribe({
      next: (student: Student) => {
        this.studentForm.patchValue({
          fullName: student.fullName,
          dateOfBirth: student.dateOfBirth ? new Date(student.dateOfBirth) : null,
          email: student.email,
          phoneNumber: student.phoneNumber,
          gpa: student.gpa ? parseFloat(student.gpa) : null,
          address: {
            street: student.address?.street || '',
            city: student.address?.city || '',
            state: student.address?.state || '',
            zipCode: student.address?.zipCode || '',
            country: student.address?.country || ''
          }
        });
      },
      error: (error) => {
        console.error('Error loading student:', error);
        this.router.navigate(['/students']);
      }
    });
  }

  onSubmit(): void {
    if (this.studentForm.invalid) return;

    this.isSubmitting = true;
    const formValue = this.studentForm.value;

    if (this.isEditMode && this.studentId) {
      const updateDto: UpdateStudentDto = {
        fullName: formValue.fullName,
        email: formValue.email,
        phoneNumber: formValue.phoneNumber,
        gpa: formValue.gpa,
        address: formValue.address
      };

      this.studentService.updateStudent(this.studentId, updateDto).subscribe({
        next: () => {
          this.router.navigate(['/students']);
        },
        error: (error) => {
          console.error('Error updating student:', error);
          this.isSubmitting = false;
        }
      });
    } else {
      const createDto: CreateStudentDto = {
        fullName: formValue.fullName,
        dateOfBirth: formValue.dateOfBirth.toISOString(),
        email: formValue.email,
        phoneNumber: formValue.phoneNumber,
        address: formValue.address
      };

      this.studentService.createStudent(createDto).subscribe({
        next: () => {
          this.router.navigate(['/students']);
        },
        error: (error) => {
          console.error('Error creating student:', error);
          this.isSubmitting = false;
        }
      });
    }
  }

  onCancel(): void {
    this.router.navigate(['/students']);
  }
}
