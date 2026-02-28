import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { FormsModule } from '@angular/forms';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';

import { CourseService } from '../../../core/services/course.service';
import { ICourse, ICourseListQuery, CourseStatus, CourseLevel } from '../../../shared/interfaces';
import { IPaginationResponse } from '../../../shared/interfaces';

@Component({
  selector: 'app-course-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatPaginatorModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    FormsModule,
    MatToolbarModule,
    MatDialogModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatChipsModule
  ],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>Courses Management</mat-card-title>
        <div class="header-actions">
          <button mat-raised-button color="primary" routerLink="create">
            <mat-icon>add</mat-icon>
            Add Course
          </button>
        </div>
      </mat-card-header>

      <mat-card-content>
        <!-- Search and Filter Section -->
        <div class="filters">
          <mat-form-field appearance="outline">
            <mat-label>Search</mat-label>
            <input matInput [(ngModel)]="searchQuery.searchTerm" 
                   (keyup.enter)="loadCourses()" 
                   placeholder="Search by title or code">
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Department</mat-label>
            <mat-select [(ngModel)]="searchQuery.department" (selectionChange)="loadCourses()">
              <mat-option value="">All Departments</mat-option>
              <mat-option *ngFor="let dept of departments" [value]="dept">{{dept}}</mat-option>
            </mat-select>
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Level</mat-label>
            <mat-select [(ngModel)]="searchQuery.level" (selectionChange)="loadCourses()">
              <mat-option value="">All Levels</mat-option>
              <mat-option *ngFor="let level of courseLevels" [value]="level">{{level}}</mat-option>
            </mat-select>
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Status</mat-label>
            <mat-select [(ngModel)]="searchQuery.status" (selectionChange)="loadCourses()">
              <mat-option value="">All Statuses</mat-option>
              <mat-option *ngFor="let status of courseStatuses" [value]="status">{{status}}</mat-option>
            </mat-select>
          </mat-form-field>

          <button mat-raised-button (click)="loadCourses()" [disabled]="isLoading">
            <mat-icon>search</mat-icon>
            Search
          </button>
          <button mat-button (click)="clearFilters()" [disabled]="isLoading">
            <mat-icon>clear</mat-icon>
            Clear
          </button>
        </div>

        <!-- Loading Spinner -->
        <div *ngIf="isLoading" class="loading-container">
          <mat-spinner></mat-spinner>
        </div>

        <!-- Courses Table -->
        <div *ngIf="!isLoading">
          <table mat-table [dataSource]="courses" class="mat-elevation-z2">
            <ng-container matColumnDef="code">
              <th mat-header-cell *matHeaderCellDef>Code</th>
              <td mat-cell *matCellDef="let course">
                <strong>{{course.code}}</strong>
              </td>
            </ng-container>

            <ng-container matColumnDef="title">
              <th mat-header-cell *matHeaderCellDef>Title</th>
              <td mat-cell *matCellDef="let course">{{course.title}}</td>
            </ng-container>

            <ng-container matColumnDef="department">
              <th mat-header-cell *matHeaderCellDef>Department</th>
              <td mat-cell *matCellDef="let course">{{course.department}}</td>
            </ng-container>

            <ng-container matColumnDef="level">
              <th mat-header-cell *matHeaderCellDef>Level</th>
              <td mat-cell *matCellDef="let course">
                <mat-chip-set>
                  <mat-chip>{{course.level}}</mat-chip>
                </mat-chip-set>
              </td>
            </ng-container>

            <ng-container matColumnDef="creditHours">
              <th mat-header-cell *matHeaderCellDef>Credits</th>
              <td mat-cell *matCellDef="let course">{{course.creditHours}}</td>
            </ng-container>

            <ng-container matColumnDef="enrollment">
              <th mat-header-cell *matHeaderCellDef>Enrollment</th>
              <td mat-cell *matCellDef="let course">
                <div class="enrollment-info">
                  <span>{{course.enrollment}}</span>
                </div>
              </td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let course">
                <span class="status-badge status-{{course.status.toLowerCase()}}">
                  {{course.status}}
                </span>
              </td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>Actions</th>
              <td mat-cell *matCellDef="let course">
                <button mat-icon-button [routerLink]="[course.id]" matTooltip="View Details">
                  <mat-icon>visibility</mat-icon>
                </button>
                <button mat-icon-button [routerLink]="[course.id, 'edit']" matTooltip="Edit Course">
                  <mat-icon>edit</mat-icon>
                </button>
                <button mat-icon-button (click)="activateCourse(course)" 
                        *ngIf="course.status === 'Scheduled'" 
                        matTooltip="Activate Course" color="primary">
                  <mat-icon>play_arrow</mat-icon>
                </button>
                <button mat-icon-button (click)="deleteCourse(course)" 
                        *ngIf="course.status === 'Draft'" 
                        matTooltip="Delete Course" color="warn">
                  <mat-icon>delete</mat-icon>
                </button>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>

          <!-- Pagination -->
          <mat-paginator 
            [length]="paginationData?.totalCount || 0"
            [pageSize]="paginationData?.pageSize || 10"
            [pageIndex]="(paginationData?.pageNumber || 1) - 1"
            [pageSizeOptions]="[5, 10, 25, 50]"
            (page)="onPageChange($event)"
            showFirstLastButtons>
          </mat-paginator>
        </div>
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .filters {
      display: flex;
      gap: 16px;
      margin-bottom: 24px;
      flex-wrap: wrap;
      align-items: center;
    }
    
    .header-actions {
      margin-left: auto;
    }
    
    .status-badge {
      padding: 4px 8px;
      border-radius: 12px;
      font-size: 12px;
      font-weight: 500;
    }
    
    .status-draft { background-color: #f5f5f5; color: #616161; }
    .status-scheduled { background-color: #fff3e0; color: #f57c00; }
    .status-active { background-color: #e8f5e8; color: #2e7d32; }
    .status-completed { background-color: #e3f2fd; color: #1976d2; }
    .status-cancelled { background-color: #ffebee; color: #d32f2f; }
    
    .enrollment-info {
      display: flex;
      align-items: center;
      gap: 4px;
    }
    
    .capacity {
      color: #666;
    }
    
    .loading-container {
      display: flex;
      justify-content: center;
      align-items: center;
      padding: 40px;
    }
    
    table {
      width: 100%;
    }
    
    mat-card-header {
      display: flex;
      align-items: center;
      margin-bottom: 16px;
    }

    mat-chip-set {
      justify-content: center;
    }
  `]
})
export class CourseListComponent implements OnInit {
  courses: ICourse[] = [];
  paginationData: IPaginationResponse<ICourse> | null = null;
  isLoading = false;
  
  displayedColumns: string[] = ['code', 'title', 'department', 'level', 'creditHours', 'enrollment', 'status', 'actions'];
  
  searchQuery: ICourseListQuery = {
    pageNumber: 1,
    pageSize: 10,
    searchTerm: '',
    sortBy: 'Title',
    sortDirection: 'asc'
  };

  departments = ['Computer Science', 'Mathematics', 'Physics', 'Chemistry', 'Biology', 'Engineering', 'Business'];
  courseLevels = ['Undergraduate', 'Graduate', 'Doctoral'];
  courseStatuses = ['Draft', 'Scheduled', 'Active', 'Completed', 'Cancelled'];

  constructor(
    private courseService: CourseService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadCourses();
  }

  loadCourses(): void {
    this.isLoading = true;
    
    this.courseService.getCourses(this.searchQuery).subscribe({
      next: (response) => {
        this.paginationData = response;
        this.courses = response.courses || response.data || [];
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading courses:', error);
        this.snackBar.open('Error loading courses', 'Close', { duration: 3000 });
        this.isLoading = false;
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.searchQuery.pageNumber = event.pageIndex + 1;
    this.searchQuery.pageSize = event.pageSize;
    this.loadCourses();
  }

  clearFilters(): void {
    this.searchQuery = {
      pageNumber: 1,
      pageSize: 10,
      searchTerm: '',
      sortBy: 'Title',
      sortDirection: 'asc'
    };
    this.loadCourses();
  }

  activateCourse(course: ICourse): void {
    if (confirm(`Are you sure you want to activate ${course.title}?`)) {
      this.courseService.activateCourse(course.id).subscribe({
        next: () => {
          this.snackBar.open('Course activated successfully', 'Close', { duration: 3000 });
          this.loadCourses();
        },
        error: (error) => {
          console.error('Error activating course:', error);
          this.snackBar.open('Error activating course', 'Close', { duration: 3000 });
        }
      });
    }
  }

  deleteCourse(course: ICourse): void {
    if (confirm(`Are you sure you want to delete ${course.title}?`)) {
      this.courseService.deleteCourse(course.id).subscribe({
        next: () => {
          this.snackBar.open('Course deleted successfully', 'Close', { duration: 3000 });
          this.loadCourses();
        },
        error: (error) => {
          console.error('Error deleting course:', error);
          this.snackBar.open('Error deleting course', 'Close', { duration: 3000 });
        }
      });
    }
  }
}