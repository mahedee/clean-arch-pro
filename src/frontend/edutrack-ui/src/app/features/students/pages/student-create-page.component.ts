import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { StudentService } from '../services/student.service';
import { IStudentCreateRequest } from '../../../shared/interfaces';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-student-create-page',
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
    MatDatepickerModule,
    MatNativeDateModule,
    MatSnackBarModule,
    PageHeaderComponent,
    LoadingSpinnerComponent
  ],
  template: `
    <app-page-header title="Create Student">
      <button mat-button routerLink="/students">
        <mat-icon>arrow_back</mat-icon>
        Back to Students
      </button>
    </app-page-header>

    <mat-card>
      <mat-card-content>
        <app-loading-spinner 
          *ngIf="isLoading"
          message="Creating student..."
          [overlay]="true">
        </app-loading-spinner>

        <form [formGroup]="studentForm" (ngSubmit)="onSubmit()" class="student-form">
          <!-- Personal Information -->
          <div class="form-section">
            <h3>Personal Information</h3>
            
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Full Name</mat-label>
              <input matInput formControlName="fullName" placeholder="Enter full name">
              <mat-error *ngIf="studentForm.get('fullName')?.hasError('required')">
                Full name is required
              </mat-error>
              <mat-error *ngIf="studentForm.get('fullName')?.hasError('minlength')">
                Full name must be at least 2 characters
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Email</mat-label>
              <input matInput formControlName="email" placeholder="Enter email address">
              <mat-error *ngIf="studentForm.get('email')?.hasError('required')">
                Email is required
              </mat-error>
              <mat-error *ngIf="studentForm.get('email')?.hasError('email')">
                Please enter a valid email address
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Date of Birth</mat-label>
              <input matInput [matDatepicker]="picker" formControlName="dateOfBirth">
              <mat-datepicker-toggle matIconSuffix [for]="picker"></mat-datepicker-toggle>
              <mat-datepicker #picker></mat-datepicker>
              <mat-error *ngIf="studentForm.get('dateOfBirth')?.hasError('required')">
                Date of birth is required
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Phone Number</mat-label>
              <input matInput formControlName="phoneNumber" placeholder="Enter phone number (optional)">
            </mat-form-field>
          </div>

          <!-- Address Information -->
          <div class="form-section">
            <h3>Address Information (Optional)</h3>
            
            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>Street</mat-label>
                <input matInput formControlName="street" placeholder="Street address">
              </mat-form-field>

              <mat-form-field appearance="outline">
                <mat-label>City</mat-label>
                <input matInput formControlName="city" placeholder="City">
              </mat-form-field>
            </div>

            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>State</mat-label>
                <input matInput formControlName="state" placeholder="State">
              </mat-form-field>

              <mat-form-field appearance="outline">
                <mat-label>Zip Code</mat-label>
                <input matInput formControlName="zipCode" placeholder="Zip code">
              </mat-form-field>
            </div>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Country</mat-label>
              <input matInput formControlName="country" placeholder="Country">
            </mat-form-field>
          </div>

          <!-- Form Actions -->
          <div class="form-actions">
            <button mat-button type="button" routerLink="/students">Cancel</button>
            <button mat-raised-button color="primary" type="submit" 
                    [disabled]="studentForm.invalid || isLoading">
              Create Student
            </button>
          </div>
        </form>
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .student-form {
      max-width: 800px;
      margin: 0 auto;
    }
    
    .form-section {
      margin-bottom: 32px;
    }
    
    .form-section h3 {
      margin: 0 0 16px 0;
      color: #333;
      font-weight: 500;
    }
    
    .form-row {
      display: flex;
      gap: 16px;
      align-items: flex-start;
    }
    
    .form-row mat-form-field {
      flex: 1;
    }
    
    .full-width {
      width: 100%;
    }
    
    .form-actions {
      display: flex;
      justify-content: flex-end;
      gap: 16px;
      margin-top: 32px;
      padding-top: 16px;
      border-top: 1px solid #e0e0e0;
    }
    
    mat-form-field {
      margin-bottom: 16px;
    }
  `]
})
export class StudentCreatePageComponent implements OnInit {
  studentForm: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private studentService: StudentService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.studentForm = this.createForm();
  }

  ngOnInit(): void {
    // Component initialization logic if needed
  }

  private createForm(): FormGroup {
    return this.fb.group({
      fullName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      dateOfBirth: ['', [Validators.required]],
      phoneNumber: [''],
      street: [''],
      city: [''],
      state: [''],
      zipCode: [''],
      country: ['']
    });
  }

  onSubmit(): void {
    if (this.studentForm.valid) {
      this.isLoading = true;
      
      const formValue = this.studentForm.value;
      const studentData: IStudentCreateRequest = {
        fullName: formValue.fullName,
        email: formValue.email,
        dateOfBirth: formValue.dateOfBirth.toISOString(),
        phoneNumber: formValue.phoneNumber || undefined,
        street: formValue.street || undefined,
        city: formValue.city || undefined,
        state: formValue.state || undefined,
        zipCode: formValue.zipCode || undefined,
        country: formValue.country || undefined
      };

      this.studentService.createStudent(studentData).subscribe({
        next: (studentId) => {
          this.isLoading = false;
          this.snackBar.open('Student created successfully', 'Close', { duration: 3000 });
          this.router.navigate(['/students', studentId]);
        },
        error: (error) => {
          this.isLoading = false;
          console.error('Error creating student:', error);
          this.snackBar.open('Error creating student. Please try again.', 'Close', { duration: 5000 });
        }
      });
    }
  }
}
