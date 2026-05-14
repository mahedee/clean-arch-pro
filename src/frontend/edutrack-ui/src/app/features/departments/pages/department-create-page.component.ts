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
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { DepartmentService } from '../services/department.service';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-department-create-page',
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
    MatSnackBarModule,
    PageHeaderComponent,
    LoadingSpinnerComponent
  ],
  template: `
    <app-page-header [title]="isEditMode ? 'Edit Department' : 'Create Department'">
      <button mat-button routerLink="/departments">
        <mat-icon>arrow_back</mat-icon>
        Back to Departments
      </button>
    </app-page-header>

    <mat-card>
      <mat-card-content>
        <app-loading-spinner *ngIf="isLoading" [message]="isEditMode ? 'Updating department...' : 'Creating department...'" [overlay]="true"></app-loading-spinner>

        <form [formGroup]="departmentForm" (ngSubmit)="onSubmit()" class="department-form">
          <div class="form-section">
            <h3>Department Information</h3>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Department Name</mat-label>
              <input matInput formControlName="name" placeholder="Enter department name">
              <mat-error *ngIf="departmentForm.get('name')?.hasError('required')">Name is required</mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width" *ngIf="!isEditMode">
              <mat-label>Department Code</mat-label>
              <input matInput formControlName="code" placeholder="e.g. CS, MATH, PHYS">
              <mat-error *ngIf="departmentForm.get('code')?.hasError('required')">Code is required</mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Description</mat-label>
              <textarea matInput formControlName="description" rows="3" placeholder="Department description (optional)"></textarea>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Location / Building</mat-label>
              <input matInput formControlName="location" placeholder="e.g. Science Building, Room 101 (optional)">
            </mat-form-field>
          </div>

          <div class="form-section">
            <h3>Contact Information (Optional)</h3>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Contact Email</mat-label>
              <input matInput formControlName="contactEmail" placeholder="department@university.edu">
              <mat-error *ngIf="departmentForm.get('contactEmail')?.hasError('email')">Enter a valid email</mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Contact Phone</mat-label>
              <input matInput formControlName="contactPhone" placeholder="Enter contact phone number">
            </mat-form-field>
          </div>

          <div class="form-actions">
            <button mat-button type="button" routerLink="/departments">Cancel</button>
            <button mat-raised-button color="primary" type="submit" [disabled]="departmentForm.invalid || isLoading">
              {{isEditMode ? 'Update Department' : 'Create Department'}}
            </button>
          </div>
        </form>
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .department-form { max-width: 700px; margin: 0 auto; }
    .form-section { margin-bottom: 32px; }
    .form-section h3 { margin-bottom: 16px; font-size: 1.1rem; font-weight: 600; color: #333; }
    .full-width { width: 100%; }
    .form-actions { display: flex; gap: 12px; justify-content: flex-end; margin-top: 16px; }
  `]
})
export class DepartmentCreatePageComponent implements OnInit {
  departmentForm!: FormGroup;
  isLoading = false;
  isEditMode = false;
  departmentId = '';

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private departmentService: DepartmentService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.departmentId = this.route.snapshot.paramMap.get('id') || '';
    this.isEditMode = !!this.departmentId && this.route.snapshot.url.some(s => s.path === 'edit');
    this.buildForm();
    if (this.isEditMode) this.loadDepartment();
  }

  buildForm(): void {
    this.departmentForm = this.fb.group({
      name: ['', [Validators.required]],
      code: [{ value: '', disabled: this.isEditMode }, this.isEditMode ? [] : [Validators.required]],
      description: [''],
      location: [''],
      contactEmail: ['', [Validators.email]],
      contactPhone: ['']
    });
  }

  loadDepartment(): void {
    this.isLoading = true;
    this.departmentService.getDepartment(this.departmentId).subscribe({
      next: (d) => {
        this.departmentForm.patchValue({
          name: d.name,
          description: d.description || '',
          location: d.location || '',
          contactEmail: d.contactEmail || '',
          contactPhone: d.contactPhone || ''
        });
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.snackBar.open('Failed to load department.', 'Close', { duration: 3000 });
      }
    });
  }

  onSubmit(): void {
    if (this.departmentForm.invalid) return;
    this.isLoading = true;
    const value = this.departmentForm.getRawValue();

    if (this.isEditMode) {
      this.departmentService.updateDepartment(this.departmentId, {
        name: value.name,
        description: value.description || undefined,
        location: value.location || undefined,
        contactEmail: value.contactEmail || undefined,
        contactPhone: value.contactPhone || undefined
      }).subscribe({
        next: () => {
          this.isLoading = false;
          this.snackBar.open('Department updated successfully.', 'Close', { duration: 3000 });
          this.router.navigate(['/departments', this.departmentId]);
        },
        error: () => {
          this.isLoading = false;
          this.snackBar.open('Failed to update department.', 'Close', { duration: 3000 });
        }
      });
    } else {
      this.departmentService.createDepartment({
        name: value.name,
        code: value.code,
        description: value.description || undefined,
        location: value.location || undefined,
        contactEmail: value.contactEmail || undefined,
        contactPhone: value.contactPhone || undefined
      }).subscribe({
        next: (id) => {
          this.isLoading = false;
          this.snackBar.open('Department created successfully.', 'Close', { duration: 3000 });
          this.router.navigate(['/departments', id]);
        },
        error: () => {
          this.isLoading = false;
          this.snackBar.open('Failed to create department.', 'Close', { duration: 3000 });
        }
      });
    }
  }
}
