import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { StudentService } from '../services/student.service';
import { IStudent } from '../../../shared/interfaces';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-student-detail-page',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatDividerModule,
    MatSnackBarModule,
    PageHeaderComponent,
    LoadingSpinnerComponent
  ],
  template: `
    <app-page-header [title]="student?.fullName || 'Student Details'">
      <button mat-button routerLink="/students">
        <mat-icon>arrow_back</mat-icon>
        Back to Students
      </button>
      <button mat-raised-button color="primary" [routerLink]="['/students', studentId, 'edit']" 
              *ngIf="student && !isLoading">
        <mat-icon>edit</mat-icon>
        Edit Student
      </button>
    </app-page-header>

    <app-loading-spinner 
      *ngIf="isLoading"
      message="Loading student details..."
      [diameter]="50">
    </app-loading-spinner>

    <div *ngIf="!isLoading && student" class="student-detail-container">
      <!-- Basic Information -->
      <mat-card class="info-card">
        <mat-card-header>
          <mat-card-title>Personal Information</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <div class="info-grid">
            <div class="info-item">
              <label>Full Name:</label>
              <span>{{ student.fullName }}</span>
            </div>
            <div class="info-item">
              <label>Email:</label>
              <span>{{ student.email }}</span>
            </div>
            <div class="info-item">
              <label>Date of Birth:</label>
              <span>{{ student.dateOfBirth | date: 'longDate' }}</span>
            </div>
            <div class="info-item" *ngIf="student.age">
              <label>Age:</label>
              <span>{{ student.age }} years old</span>
            </div>
            <div class="info-item" *ngIf="student.phoneNumber">
              <label>Phone Number:</label>
              <span>{{ student.phoneNumber }}</span>
            </div>
            <div class="info-item">
              <label>Status:</label>
              <mat-chip-set>
                <mat-chip [class]="'status-' + student.status.toLowerCase()">
                  {{ student.status }}
                </mat-chip>
              </mat-chip-set>
            </div>
          </div>
        </mat-card-content>
      </mat-card>

      <!-- Address Information -->
      <mat-card class="info-card" *ngIf="student.address">
        <mat-card-header>
          <mat-card-title>Address Information</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <div class="address-info">
            <p>{{ student.address.street }}</p>
            <p *ngIf="student.address.street2">{{ student.address.street2 }}</p>
            <p>{{ student.address.city }}, {{ student.address.state }} {{ student.address.zipCode }}</p>
            <p>{{ student.address.country }}</p>
          </div>
        </mat-card-content>
      </mat-card>

      <!-- Academic Information -->
      <mat-card class="info-card">
        <mat-card-header>
          <mat-card-title>Academic Information</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <div class="info-grid">
            <div class="info-item">
              <label>Current GPA:</label>
              <span>{{ student.gpa?.toFixed(2) || 'N/A' }}</span>
            </div>
            <div class="info-item">
              <label>Enrollment Date:</label>
              <span>{{ student.enrollmentDate | date: 'longDate' }}</span>
            </div>
            <div class="info-item" *ngIf="student.createdAt">
              <label>Record Created:</label>
              <span>{{ student.createdAt | date: 'short' }}</span>
            </div>
            <div class="info-item" *ngIf="student.updatedAt">
              <label>Last Updated:</label>
              <span>{{ student.updatedAt | date: 'short' }}</span>
            </div>
          </div>
        </mat-card-content>
      </mat-card>

      <!-- Actions -->
      <mat-card class="actions-card">
        <mat-card-content>
          <div class="action-buttons">
            <button mat-raised-button color="primary" [routerLink]="['/students', studentId, 'edit']">
              <mat-icon>edit</mat-icon>
              Edit Student
            </button>
            <button mat-stroked-button>
              <mat-icon>assignment</mat-icon>
              View Enrollments
            </button>
            <button mat-stroked-button>
              <mat-icon>grade</mat-icon>
              View Grades
            </button>
          </div>
        </mat-card-content>
      </mat-card>
    </div>

    <!-- Error State -->
    <mat-card *ngIf="!isLoading && !student" class="error-card">
      <mat-card-content>
        <div class="error-content">
          <mat-icon>error_outline</mat-icon>
          <h3>Student Not Found</h3>
          <p>The requested student could not be found.</p>
          <button mat-raised-button color="primary" routerLink="/students">
            Back to Students
          </button>
        </div>
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .student-detail-container {
      display: flex;
      flex-direction: column;
      gap: 24px;
    }
    
    .info-card {
      margin-bottom: 24px;
    }
    
    .info-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 16px;
    }
    
    .info-item {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }
    
    .info-item label {
      font-weight: 500;
      color: #666;
      font-size: 14px;
    }
    
    .info-item span {
      font-size: 16px;
      color: #333;
    }
    
    .address-info {
      line-height: 1.6;
    }
    
    .address-info p {
      margin: 0 0 8px 0;
    }
    
    .actions-card {
      background: #f5f5f5;
    }
    
    .action-buttons {
      display: flex;
      gap: 16px;
      flex-wrap: wrap;
    }
    
    .error-card {
      text-align: center;
      padding: 48px 24px;
    }
    
    .error-content mat-icon {
      font-size: 64px;
      height: 64px;
      width: 64px;
      color: #f44336;
      margin-bottom: 16px;
    }
    
    .error-content h3 {
      margin: 0 0 8px 0;
      color: #333;
    }
    
    .error-content p {
      margin: 0 0 24px 0;
      color: #666;
    }
    
    /* Status chip styles */
    mat-chip.status-active {
      background: #e8f5e8;
      color: #2e7d30;
    }
    
    mat-chip.status-inactive {
      background: #fff3e0;
      color: #f57c00;
    }
    
    mat-chip.status-graduated {
      background: #e3f2fd;
      color: #1976d2;
    }
    
    mat-chip.status-withdrawn {
      background: #ffebee;
      color: #d32f2f;
    }
    
    mat-chip.status-onprobation {
      background: #fce4ec;
      color: #c2185b;
    }
  `]
})
export class StudentDetailPageComponent implements OnInit {
  student: IStudent | null = null;
  studentId: string;
  isLoading = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private studentService: StudentService,
    private snackBar: MatSnackBar
  ) {
    this.studentId = this.route.snapshot.paramMap.get('id') || '';
  }

  ngOnInit(): void {
    if (this.studentId) {
      this.loadStudent();
    } else {
      this.router.navigate(['/students']);
    }
  }

  private loadStudent(): void {
    this.isLoading = true;
    
    this.studentService.getStudent(this.studentId).subscribe({
      next: (student) => {
        this.student = student;
        // Calculate age if dateOfBirth is available
        if (student.dateOfBirth && !student.age) {
          this.student!.age = this.calculateAge(student.dateOfBirth);
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading student:', error);
        this.snackBar.open('Error loading student details', 'Close', { duration: 3000 });
        this.isLoading = false;
      }
    });
  }

  private calculateAge(dateOfBirth: string): number {
    const today = new Date();
    const birthDate = new Date(dateOfBirth);
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();
    
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }
    
    return age;
  }
}
