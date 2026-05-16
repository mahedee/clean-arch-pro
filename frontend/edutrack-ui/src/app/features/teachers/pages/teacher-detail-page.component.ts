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

import { TeacherService } from '../services/teacher.service';
import { ITeacher } from '../../../shared/interfaces';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-teacher-detail-page',
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
    <app-page-header [title]="teacher?.fullName || 'Teacher Details'">
      <button mat-button routerLink="/teachers">
        <mat-icon>arrow_back</mat-icon>
        Back to Teachers
      </button>
      <button mat-raised-button color="primary" [routerLink]="['/teachers', teacherId, 'edit']" *ngIf="teacher && !isLoading">
        <mat-icon>edit</mat-icon>
        Edit Teacher
      </button>
    </app-page-header>

    <app-loading-spinner *ngIf="isLoading" message="Loading teacher details..." [diameter]="50"></app-loading-spinner>

    <div *ngIf="!isLoading && teacher" class="detail-container">
      <mat-card class="info-card">
        <mat-card-header><mat-card-title>Personal Information</mat-card-title></mat-card-header>
        <mat-card-content>
          <div class="info-grid">
            <div class="info-item"><label>Full Name:</label><span>{{teacher.fullName}}</span></div>
            <div class="info-item"><label>Email:</label><span>{{teacher.email}}</span></div>
            <div class="info-item"><label>Employee ID:</label><span>{{teacher.employeeId}}</span></div>
            <div class="info-item" *ngIf="teacher.phoneNumber"><label>Phone:</label><span>{{teacher.phoneNumber}}</span></div>
            <div class="info-item"><label>Date of Birth:</label><span>{{teacher.dateOfBirth | date: 'longDate'}}</span></div>
            <div class="info-item">
              <label>Status:</label>
              <mat-chip-set><mat-chip>{{teacher.status}}</mat-chip></mat-chip-set>
            </div>
          </div>
        </mat-card-content>
      </mat-card>

      <mat-card class="info-card">
        <mat-card-header><mat-card-title>Academic Information</mat-card-title></mat-card-header>
        <mat-card-content>
          <div class="info-grid">
            <div class="info-item"><label>Department:</label><span>{{teacher.department}}</span></div>
            <div class="info-item"><label>Academic Title:</label><span>{{teacher.title}}</span></div>
            <div class="info-item"><label>Hire Date:</label><span>{{teacher.hireDate | date: 'longDate'}}</span></div>
            <div class="info-item"><label>Course Load:</label><span>{{teacher.currentCourseLoad}} / {{teacher.maxCoursesPerSemester}}</span></div>
            <div class="info-item" *ngIf="teacher.officeLocation"><label>Office:</label><span>{{teacher.officeLocation}}</span></div>
            <div class="info-item" *ngIf="teacher.officeHours"><label>Office Hours:</label><span>{{teacher.officeHours}}</span></div>
          </div>
        </mat-card-content>
      </mat-card>

      <mat-card class="info-card" *ngIf="teacher.specializations?.length">
        <mat-card-header><mat-card-title>Specializations</mat-card-title></mat-card-header>
        <mat-card-content>
          <mat-chip-set>
            <mat-chip *ngFor="let s of teacher.specializations">{{s}}</mat-chip>
          </mat-chip-set>
        </mat-card-content>
      </mat-card>

      <mat-card class="info-card" *ngIf="teacher.qualifications?.length">
        <mat-card-header><mat-card-title>Qualifications</mat-card-title></mat-card-header>
        <mat-card-content>
          <mat-chip-set>
            <mat-chip *ngFor="let q of teacher.qualifications">{{q}}</mat-chip>
          </mat-chip-set>
        </mat-card-content>
      </mat-card>
    </div>

    <div *ngIf="!isLoading && !teacher" class="not-found">
      <mat-icon>error_outline</mat-icon>
      <p>Teacher not found.</p>
      <button mat-raised-button color="primary" routerLink="/teachers">Back to Teachers</button>
    </div>
  `,
  styles: [`
    .detail-container { display: flex; flex-direction: column; gap: 16px; }
    .info-card { margin-bottom: 0; }
    .info-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(260px, 1fr)); gap: 16px; padding: 8px 0; }
    .info-item { display: flex; flex-direction: column; gap: 4px; }
    .info-item label { font-weight: 600; color: #666; font-size: 0.85rem; }
    .not-found { text-align: center; padding: 64px; color: #666; }
    .not-found mat-icon { font-size: 64px; height: 64px; width: 64px; }
  `]
})
export class TeacherDetailPageComponent implements OnInit {
  teacher: ITeacher | null = null;
  teacherId = '';
  isLoading = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private teacherService: TeacherService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.teacherId = this.route.snapshot.paramMap.get('id') || '';
    if (this.teacherId) this.loadTeacher();
  }

  loadTeacher(): void {
    this.isLoading = true;
    this.teacherService.getTeacher(this.teacherId).subscribe({
      next: (t) => { this.teacher = t; this.isLoading = false; },
      error: () => { this.isLoading = false; this.snackBar.open('Failed to load teacher.', 'Close', { duration: 3000 }); }
    });
  }
}
