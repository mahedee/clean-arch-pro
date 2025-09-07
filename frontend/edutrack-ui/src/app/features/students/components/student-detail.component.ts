import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';

import { StudentService } from '../../../core/services/student.service';
import { Student } from '../../../shared/models/student.model';

@Component({
  selector: 'app-student-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatDividerModule
  ],
  template: `
    <mat-card *ngIf="student">
      <mat-card-header>
        <mat-card-title>{{ student.fullName }}</mat-card-title>
        <div class="header-actions">
          <button mat-icon-button [routerLink]="['/students', student.id, 'edit']" matTooltip="Edit">
            <mat-icon>edit</mat-icon>
          </button>
          <button mat-icon-button (click)="deleteStudent()" matTooltip="Delete" color="warn">
            <mat-icon>delete</mat-icon>
          </button>
        </div>
      </mat-card-header>

      <mat-card-content>
        <div class="student-info">
          <!-- Status -->
          <div class="info-row">
            <mat-chip-set>
              <mat-chip [class]="'status-' + student.status.toLowerCase()">
                {{ student.status }}
              </mat-chip>
            </mat-chip-set>
          </div>

          <mat-divider></mat-divider>

          <!-- Personal Information -->
          <div class="info-section">
            <h3>Personal Information</h3>
            <div class="info-grid">
              <div class="info-item">
                <label>Full Name:</label>
                <span>{{ student.fullName }}</span>
              </div>
              <div class="info-item">
                <label>Age:</label>
                <span>{{ student.age }} years old</span>
              </div>
              <div class="info-item">
                <label>Enrollment Date:</label>
                <span>{{ student.enrollmentDate | date: 'medium' }}</span>
              </div>
              <div class="info-item" *ngIf="student.gpa">
                <label>GPA:</label>
                <span>{{ student.gpa }}</span>
              </div>
            </div>
          </div>

          <mat-divider></mat-divider>

          <!-- Contact Information -->
          <div class="info-section">
            <h3>Contact Information</h3>
            <div class="info-grid">
              <div class="info-item">
                <label>Email:</label>
                <span>
                  <a [href]="'mailto:' + student.email">{{ student.email }}</a>
                </span>
              </div>
              <div class="info-item">
                <label>Phone:</label>
                <span>
                  <a [href]="'tel:' + student.phoneNumber">{{ student.phoneNumber }}</a>
                </span>
              </div>
            </div>
          </div>

          <!-- Address Information -->
          <div class="info-section" *ngIf="student.address && hasAddress()">
            <mat-divider></mat-divider>
            <h3>Address Information</h3>
            <div class="address-info">
              <div *ngIf="student.address.street">{{ student.address.street }}</div>
              <div *ngIf="student.address.city || student.address.state || student.address.zipCode">
                {{ student.address.city }}{{ student.address.city && student.address.state ? ', ' : '' }}{{ student.address.state }} {{ student.address.zipCode }}
              </div>
              <div *ngIf="student.address.country">{{ student.address.country }}</div>
            </div>
          </div>
        </div>
      </mat-card-content>

      <mat-card-actions>
        <button mat-button routerLink="/students">
          <mat-icon>arrow_back</mat-icon>
          Back to List
        </button>
        <button mat-raised-button color="primary" [routerLink]="['/students', student.id, 'edit']">
          <mat-icon>edit</mat-icon>
          Edit Student
        </button>
      </mat-card-actions>
    </mat-card>

    <div *ngIf="!student && !loading" class="not-found">
      <mat-card>
        <mat-card-content>
          <h2>Student Not Found</h2>
          <p>The student you're looking for doesn't exist.</p>
          <button mat-raised-button color="primary" routerLink="/students">
            Back to Students
          </button>
        </mat-card-content>
      </mat-card>
    </div>

    <div *ngIf="loading" class="loading">
      <mat-card>
        <mat-card-content>
          Loading student information...
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .header-actions {
      margin-left: auto;
      display: flex;
      gap: 8px;
    }

    .student-info {
      margin-top: 16px;
    }

    .info-row {
      margin-bottom: 16px;
    }

    .info-section {
      margin: 24px 0;
    }

    .info-section h3 {
      margin-bottom: 16px;
      color: #333;
      font-weight: 500;
    }

    .info-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
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

    .info-item span, .info-item a {
      font-size: 16px;
    }

    .address-info {
      line-height: 1.5;
    }

    .status-active { 
      background-color: #e8f5e8; 
      color: #2e7d2e; 
    }
    .status-inactive { 
      background-color: #f5f5f5; 
      color: #666; 
    }
    .status-graduated { 
      background-color: #e3f2fd; 
      color: #1976d2; 
    }
    .status-suspended { 
      background-color: #ffebee; 
      color: #d32f2f; 
    }

    mat-card {
      max-width: 800px;
      margin: 0 auto;
    }

    mat-card-header {
      display: flex;
      align-items: center;
    }

    .not-found, .loading {
      display: flex;
      justify-content: center;
      margin-top: 50px;
    }

    .not-found mat-card, .loading mat-card {
      text-align: center;
      padding: 32px;
    }
  `]
})
export class StudentDetailComponent implements OnInit {
  student?: Student;
  loading = true;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private studentService: StudentService
  ) {}

  ngOnInit(): void {
    const studentId = this.route.snapshot.params['id'];
    if (studentId) {
      this.loadStudent(studentId);
    }
  }

  loadStudent(id: string): void {
    this.loading = true;
    this.studentService.getStudent(id).subscribe({
      next: (student: Student) => {
        this.student = student;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading student:', error);
        this.loading = false;
      }
    });
  }

  hasAddress(): boolean {
    const address = this.student?.address;
    return !!(address && (address.street || address.city || address.state || address.zipCode || address.country));
  }

  deleteStudent(): void {
    if (!this.student) return;

    if (confirm(`Are you sure you want to delete ${this.student.fullName}?`)) {
      this.studentService.deleteStudent(this.student.id).subscribe({
        next: () => {
          this.router.navigate(['/students']);
        },
        error: (error) => {
          console.error('Error deleting student:', error);
        }
      });
    }
  }
}
