import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { StudentService } from '../../../core/services/student.service';
import { StudentDto, CreateStudentDto, UpdateStudentDto, Address } from '../../../models/student.model';

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
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>{{ isEditMode ? 'Edit Student' : 'Create Student' }}</mat-card-title>
      </mat-card-header>

      <mat-card-content>
        <div class="loading-container" *ngIf="isLoading">
          <mat-spinner></mat-spinner>
        </div>

        <form [formGroup]="studentForm" (ngSubmit)="onSubmit()" *ngIf="!isLoading">
          <!-- Personal Information -->
          <div class="form-section">
            <h3>Personal Information</h3>
            
            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>Full Name</mat-label>
                <input matInput formControlName="fullName" required>
                <mat-error *ngIf="studentForm.get('fullName')?.hasError('required')">
                  Full name is required
                </mat-error>
                <mat-error *ngIf="studentForm.get('fullName')?.hasError('minlength')">
                  Name must be at least 2 characters
                </mat-error>
              </mat-form-field>

              <mat-form-field appearance="outline">
                <mat-label>Date of Birth</mat-label>
                <input matInput [matDatepicker]="picker" formControlName="dateOfBirth" required>
                <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
                <mat-datepicker #picker></mat-datepicker>
                <mat-error *ngIf="studentForm.get('dateOfBirth')?.hasError('required')">
                  Date of birth is required
                </mat-error>
              </mat-form-field>
            </div>
          </div>

          <!-- Contact Information -->
          <div class="form-section">
            <h3>Contact Information</h3>
            
            <div class="form-row">
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
                <input matInput formControlName="phoneNumber">
                <mat-error *ngIf="studentForm.get('phoneNumber')?.hasError('pattern')">
                  Please enter a valid phone number
                </mat-error>
              </mat-form-field>
            </div>
          </div>

          <!-- Address Information -->
          <div class="form-section" formGroupName="address">
            <h3>Address Information</h3>
            
            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>Street</mat-label>
                <input matInput formControlName="street">
              </mat-form-field>

              <mat-form-field appearance="outline">
                <mat-label>City</mat-label>
                <input matInput formControlName="city">
              </mat-form-field>
            </div>

            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>State</mat-label>
                <input matInput formControlName="state">
              </mat-form-field>

              <mat-form-field appearance="outline">
                <mat-label>ZIP Code</mat-label>
                <input matInput formControlName="zipCode">
              </mat-form-field>

              <mat-form-field appearance="outline">
                <mat-label>Country</mat-label>
                <input matInput formControlName="country">
              </mat-form-field>
            </div>
          </div>

          <!-- Academic Information (Edit Mode Only) -->
          <div class="form-section" *ngIf="isEditMode">
            <h3>Academic Information</h3>
            
            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>GPA</mat-label>
                <input matInput type="number" formControlName="gpa" min="0" max="4" step="0.01">
                <mat-error *ngIf="studentForm.get('gpa')?.hasError('min')">
                  GPA cannot be negative
                </mat-error>
                <mat-error *ngIf="studentForm.get('gpa')?.hasError('max')">
                  GPA cannot exceed 4.0
                </mat-error>
              </mat-form-field>
            </div>
          </div>

          <!-- Form Actions -->
          <div class="form-actions">
            <button mat-button type="button" (click)="onCancel()">Cancel</button>
            <button mat-raised-button color="primary" type="submit" [disabled]="studentForm.invalid || isSubmitting">
              <mat-icon *ngIf="isSubmitting">hourglass_empty</mat-icon>
              {{ isEditMode ? 'Update' : 'Create' }} Student
            </button>
          </div>
        </form>
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .form-section {
      margin-bottom: 32px;
    }
    
    .form-section h3 {
      margin-bottom: 16px;
      color: #424242;
      font-weight: 500;
    }
    
    .form-row {
      display: flex;
      gap: 16px;
      margin-bottom: 16px;
    }
    
    .form-row mat-form-field {
      flex: 1;
    }
    
    .form-actions {
      display: flex;
      gap: 16px;
      justify-content: flex-end;
      margin-top: 32px;
      padding-top: 16px;
      border-top: 1px solid #e0e0e0;
    }
    
    .loading-container {
      display: flex;
      justify-content: center;
      align-items: center;
      padding: 40px;
    }

    mat-card {
      max-width: 800px;
      margin: 20px auto;
    }
  `]
})
export class StudentFormComponent implements OnInit {
  studentForm!: FormGroup;
  isEditMode = false;
  isLoading = false;
  isSubmitting = false;
  studentId?: string;

  constructor(
    private fb: FormBuilder,
    private studentService: StudentService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar
  ) {
    this.initializeForm();
  }

  ngOnInit(): void {
    this.studentId = this.route.snapshot.paramMap.get('id') || undefined;
    this.isEditMode = !!this.studentId;

    if (this.isEditMode && this.studentId) {
      this.loadStudent();
    }
  }

  private initializeForm(): void {
    this.studentForm = this.fb.group({
      fullName: ['', [Validators.required, Validators.minLength(2)]],
      dateOfBirth: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', Validators.pattern(/^[\+]?[\d\s\-\(\)]+$/)],
      address: this.fb.group({
        street: [''],
        city: [''],
        state: [''],
        zipCode: [''],
        country: ['']
      }),
      gpa: [null, [Validators.min(0), Validators.max(4.0)]]
    });
  }

  private loadStudent(): void {
    if (!this.studentId) return;
    
    this.isLoading = true;
    this.studentService.getStudent(this.studentId).subscribe({
      next: (student) => {
        this.populateForm(student);
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading student:', error);
        this.snackBar.open('Error loading student data', 'Close', { duration: 3000 });
        this.isLoading = false;
        this.router.navigate(['/students']);
      }
    });
  }

  private populateForm(student: StudentDto): void {
    this.studentForm.patchValue({
      fullName: student.fullName,
      dateOfBirth: new Date(student.dateOfBirth),
      email: student.email,
      phoneNumber: student.phoneNumber || '',
      address: {
        street: student.address?.street || '',
        city: student.address?.city || '',
        state: student.address?.state || '',
        zipCode: student.address?.zipCode || '',
        country: student.address?.country || ''
      },
      gpa: student.gpa || null
    });
  }

  onSubmit(): void {
    if (this.studentForm.invalid) {
      this.studentForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.studentForm.value;

    if (this.isEditMode && this.studentId) {
      this.updateStudent(formValue);
    } else {
      this.createStudent(formValue);
    }
  }

  private createStudent(formData: any): void {
    const createDto: CreateStudentDto = {
      fullName: formData.fullName,
      dateOfBirth: formData.dateOfBirth,
      email: formData.email,
      phoneNumber: formData.phoneNumber || undefined,
      address: this.hasAddressData(formData.address) ? formData.address : undefined
    };

    this.studentService.createStudent(createDto).subscribe({
      next: (studentId) => {
        this.snackBar.open('Student created successfully', 'Close', { duration: 3000 });
        this.router.navigate(['/students', studentId]);
      },
      error: (error) => {
        console.error('Error creating student:', error);
        this.snackBar.open('Error creating student', 'Close', { duration: 3000 });
        this.isSubmitting = false;
      }
    });
  }

  private updateStudent(formData: any): void {
    const updateDto: UpdateStudentDto = {
      fullName: formData.fullName,
      email: formData.email,
      phoneNumber: formData.phoneNumber || undefined,
      address: this.hasAddressData(formData.address) ? formData.address : undefined,
      gpa: formData.gpa || undefined
    };

    this.studentService.updateStudent(this.studentId!, updateDto).subscribe({
      next: () => {
        this.snackBar.open('Student updated successfully', 'Close', { duration: 3000 });
        this.router.navigate(['/students', this.studentId]);
      },
      error: (error) => {
        console.error('Error updating student:', error);
        this.snackBar.open('Error updating student', 'Close', { duration: 3000 });
        this.isSubmitting = false;
      }
    });
  }

  private hasAddressData(address: Address): boolean {
    return !!(address.street || address.city || address.state || address.zipCode || address.country);
  }

  onCancel(): void {
    if (this.isEditMode) {
      this.router.navigate(['/students', this.studentId]);
    } else {
      this.router.navigate(['/students']);
    }
  }
}
