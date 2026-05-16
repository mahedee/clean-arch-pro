import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { CourseService } from '../../../core/services/course.service';
import { ICourseCreateRequest, ICourseUpdateRequest, ICourse } from '../../../shared/interfaces';

@Component({
  selector: 'app-course-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatIconModule,
    MatSnackBarModule,
    MatProgressSpinnerModule
  ],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>{{ isEditMode ? 'Edit Course' : 'Create Course' }}</mat-card-title>
      </mat-card-header>

      <mat-card-content>
        <div *ngIf="isLoading" class="loading-container">
          <mat-spinner></mat-spinner>
        </div>

        <form [formGroup]="courseForm" (ngSubmit)="onSubmit()" *ngIf="!isLoading">
          <!-- Basic Information -->
          <div class="form-section">
            <h3>Course Information</h3>
            
            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>Course Code</mat-label>
                <input matInput formControlName="code" required placeholder="e.g., CS101">
                <mat-error *ngIf="courseForm.get('code')?.hasError('required')">
                  Course code is required
                </mat-error>
                <mat-error *ngIf="courseForm.get('code')?.hasError('pattern')">
                  Please enter a valid course code
                </mat-error>
              </mat-form-field>

              <mat-form-field appearance="outline">
                <mat-label>Credits</mat-label>
                <input matInput type="number" formControlName="credits" required min="1" max="6">
                <mat-error *ngIf="courseForm.get('credits')?.hasError('required')">
                  Credits are required
                </mat-error>
                <mat-error *ngIf="courseForm.get('credits')?.hasError('min')">
                  Minimum 1 credit required
                </mat-error>
                <mat-error *ngIf="courseForm.get('credits')?.hasError('max')">
                  Maximum 6 credits allowed
                </mat-error>
              </mat-form-field>
            </div>

            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>Course Title</mat-label>
                <input matInput formControlName="title" required>
                <mat-error *ngIf="courseForm.get('title')?.hasError('required')">
                  Course title is required
                </mat-error>
                <mat-error *ngIf="courseForm.get('title')?.hasError('minlength')">
                  Title must be at least 3 characters
                </mat-error>
              </mat-form-field>
            </div>

            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>Description</mat-label>
                <textarea matInput formControlName="description" required rows="4"></textarea>
                <mat-error *ngIf="courseForm.get('description')?.hasError('required')">
                  Description is required
                </mat-error>
                <mat-error *ngIf="courseForm.get('description')?.hasError('minlength')">
                  Description must be at least 10 characters
                </mat-error>
              </mat-form-field>
            </div>
          </div>

          <!-- Academic Information -->
          <div class="form-section">
            <h3>Academic Details</h3>
            
            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>Department</mat-label>
                <mat-select formControlName="department" required>
                  <mat-option *ngFor="let dept of departments" [value]="dept">{{dept}}</mat-option>
                </mat-select>
                <mat-error *ngIf="courseForm.get('department')?.hasError('required')">
                  Department is required
                </mat-error>
              </mat-form-field>

              <mat-form-field appearance="outline">
                <mat-label>Level</mat-label>
                <mat-select formControlName="level" required>
                  <mat-option *ngFor="let level of courseLevels" [value]="level">{{level}}</mat-option>
                </mat-select>
                <mat-error *ngIf="courseForm.get('level')?.hasError('required')">
                  Level is required
                </mat-error>
              </mat-form-field>

              <mat-form-field appearance="outline">
                <mat-label>Capacity</mat-label>
                <input matInput type="number" formControlName="capacity" min="1" max="500" required>
                <mat-error *ngIf="courseForm.get('capacity')?.hasError('required')">
                  Capacity is required
                </mat-error>
                <mat-error *ngIf="courseForm.get('capacity')?.hasError('min')">
                  Minimum capacity is 1
                </mat-error>
                <mat-error *ngIf="courseForm.get('capacity')?.hasError('max')">
                  Maximum capacity is 500
                </mat-error>
              </mat-form-field>
            </div>

            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>Prerequisite Credit Hours</mat-label>
                <input matInput type="number" formControlName="prerequisiteCreditHours" min="0" max="100">
                <mat-hint>Number of credit hours required as prerequisites (0 if none)</mat-hint>
                <mat-error *ngIf="courseForm.get('prerequisiteCreditHours')?.hasError('min')">
                  Cannot be negative
                </mat-error>
                <mat-error *ngIf="courseForm.get('prerequisiteCreditHours')?.hasError('max')">
                  Maximum 100 credit hours allowed
                </mat-error>
              </mat-form-field>
            </div>
          </div>

          <!-- Form Actions -->
          <div class="form-actions">
            <button mat-button type="button" (click)="onCancel()">Cancel</button>
            <button mat-raised-button color="primary" type="submit" [disabled]="courseForm.invalid || isSubmitting">
              <mat-icon *ngIf="isSubmitting">hourglass_empty</mat-icon>
              {{ isEditMode ? 'Update' : 'Create' }} Course
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

    textarea {
      min-height: 100px;
    }
  `]
})
export class CourseFormComponent implements OnInit {
  courseForm!: FormGroup;
  isEditMode = false;
  isLoading = false;
  isSubmitting = false;
  courseId?: string;

  departments = [
    'Computer Science', 'Mathematics', 'Physics', 'Chemistry', 
    'Biology', 'Engineering', 'Business', 'Psychology', 'History'
  ];

  courseLevels = [
    'Undergraduate', 'Graduate', 'Postgraduate', 
    'Doctoral', 'Certificate', 'Continuing'
  ];

  constructor(
    private fb: FormBuilder,
    private courseService: CourseService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar
  ) {
    this.initializeForm();
  }

  ngOnInit(): void {
    this.courseId = this.route.snapshot.paramMap.get('id') || undefined;
    this.isEditMode = !!this.courseId;

    if (this.isEditMode && this.courseId) {
      this.loadCourse();
    }
  }

  private initializeForm(): void {
    this.courseForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      code: ['', [Validators.required, Validators.pattern(/^[A-Z]{2,4}\d{3,4}$/)]],
      description: ['', [Validators.required, Validators.minLength(10)]],
      credits: [null, [Validators.required, Validators.min(1), Validators.max(12)]],
      department: ['', Validators.required],
      level: ['', Validators.required],
      capacity: [null, [Validators.required, Validators.min(1), Validators.max(500)]],
      prerequisiteCreditHours: [0, [Validators.min(0), Validators.max(100)]]
    });
  }

  private loadCourse(): void {
    if (!this.courseId) return;
    
    this.isLoading = true;
    this.courseService.getCourse(this.courseId).subscribe({
      next: (course) => {
        this.populateForm(course);
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading course:', error);
        this.snackBar.open('Error loading course data', 'Close', { duration: 3000 });
        this.isLoading = false;
        this.router.navigate(['/courses']);
      }
    });
  }

  private populateForm(course: ICourse): void {
    this.courseForm.patchValue({
      title: course.title,
      code: course.code,
      description: course.description,
      credits: course.creditHours,
      department: course.department,
      level: course.level,
      capacity: course.maxCapacity || 30,
      prerequisiteCreditHours: 0 // Default value, can be enhanced later
    });
  }

  onSubmit(): void {
    if (this.courseForm.invalid) {
      this.courseForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.courseForm.value;

    if (this.isEditMode && this.courseId) {
      this.updateCourse(formValue);
    } else {
      this.createCourse(formValue);
    }
  }

  private createCourse(formData: any): void {
    const createCommand: ICourseCreateRequest = {
      title: formData.title,
      description: formData.description || 'No description provided',
      courseCode: formData.code,
      credits: formData.credits,
      maxCapacity: formData.capacity || 30,
      department: formData.department,
      level: formData.level,
      academicPeriod: formData.academicPeriod || new Date().getFullYear().toString(),
      prerequisites: []
    };

    this.courseService.createCourse(createCommand).subscribe({
      next: (courseId) => {
        this.snackBar.open('Course created successfully', 'Close', { duration: 3000 });
        this.router.navigate(['/courses', courseId]);
      },
      error: (error) => {
        console.error('Error creating course:', error);
        this.snackBar.open('Error creating course', 'Close', { duration: 3000 });
        this.isSubmitting = false;
      }
    });
  }

  private updateCourse(formData: any): void {
    const updateCommand: ICourseUpdateRequest = {
      title: formData.title,
      description: formData.description || 'No description provided',
      courseCode: formData.code,
      credits: formData.credits,
      maxCapacity: formData.capacity || 30,
      department: formData.department,
      level: formData.level,
      prerequisiteCreditHours: formData.prerequisiteCreditHours || 0
    };

    this.courseService.updateCourse(this.courseId!, updateCommand).subscribe({
      next: () => {
        this.snackBar.open('Course updated successfully', 'Close', { duration: 3000 });
        this.router.navigate(['/courses', this.courseId]);
      },
      error: (error) => {
        console.error('Error updating course:', error);
        this.snackBar.open('Error updating course', 'Close', { duration: 3000 });
        this.isSubmitting = false;
      }
    });
  }

  onCancel(): void {
    if (this.isEditMode) {
      this.router.navigate(['/courses', this.courseId]);
    } else {
      this.router.navigate(['/courses']);
    }
  }
}