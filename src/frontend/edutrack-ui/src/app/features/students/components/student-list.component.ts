import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { StudentService } from '../../../core/services/student.service';
import { StudentDto, GetStudentListQuery, PaginatedStudentListDto, StudentStatus } from '../../../models/student.model';

@Component({
  selector: 'app-student-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatPaginatorModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDialogModule
  ],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>Students</mat-card-title>
        <div class="header-actions">
          <button mat-raised-button color="primary" routerLink="/students/create">
            <mat-icon>add</mat-icon>
            Add Student
          </button>
        </div>
      </mat-card-header>

      <mat-card-content>
        <!-- Search and Filter Section -->
        <div class="filters">
          <mat-form-field appearance="outline">
            <mat-label>Search</mat-label>
            <input matInput [(ngModel)]="searchQuery.searchTerm" 
                   (keyup.enter)="loadStudents()" 
                   placeholder="Search by name or email">
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Sort By</mat-label>
            <mat-select [(ngModel)]="searchQuery.sortBy" (selectionChange)="loadStudents()">
              <mat-option value="fullName">Full Name</mat-option>
              <mat-option value="email">Email</mat-option>
              <mat-option value="enrollmentDate">Enrollment Date</mat-option>
              <mat-option value="gpa">GPA</mat-option>
            </mat-select>
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Sort Direction</mat-label>
            <mat-select [(ngModel)]="searchQuery.sortDirection" (selectionChange)="loadStudents()">
              <mat-option value="asc">Ascending</mat-option>
              <mat-option value="desc">Descending</mat-option>
            </mat-select>
          </mat-form-field>

          <button mat-raised-button (click)="loadStudents()" [disabled]="isLoading">
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

        <!-- Students Table -->
        <div *ngIf="!isLoading">
          <table mat-table [dataSource]="students" class="mat-elevation-z2">
            <ng-container matColumnDef="fullName">
              <th mat-header-cell *matHeaderCellDef>Full Name</th>
              <td mat-cell *matCellDef="let student">{{student.fullName}}</td>
            </ng-container>

            <ng-container matColumnDef="email">
              <th mat-header-cell *matHeaderCellDef>Email</th>
              <td mat-cell *matCellDef="let student">{{student.email}}</td>
            </ng-container>

            <ng-container matColumnDef="phoneNumber">
              <th mat-header-cell *matHeaderCellDef>Phone</th>
              <td mat-cell *matCellDef="let student">{{student.phoneNumber || 'N/A'}}</td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let student">
                <span class="status-badge status-{{student.status?.toLowerCase()}}">
                  {{student.status}}
                </span>
              </td>
            </ng-container>

            <ng-container matColumnDef="gpa">
              <th mat-header-cell *matHeaderCellDef>GPA</th>
              <td mat-cell *matCellDef="let student">{{student.gpa?.toFixed(2) || 'N/A'}}</td>
            </ng-container>

            <ng-container matColumnDef="enrollmentDate">
              <th mat-header-cell *matHeaderCellDef>Enrollment Date</th>
              <td mat-cell *matCellDef="let student">{{student.enrollmentDate | date: 'short'}}</td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>Actions</th>
              <td mat-cell *matCellDef="let student">
                <button mat-icon-button [routerLink]="[student.id]" matTooltip="View Details">
                  <mat-icon>visibility</mat-icon>
                </button>
                <button mat-icon-button [routerLink]="[student.id, 'edit']" matTooltip="Edit Student">
                  <mat-icon>edit</mat-icon>
                </button>
                <button mat-icon-button (click)="deleteStudent(student)" matTooltip="Delete Student" color="warn">
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
    
    .status-active { background-color: #e8f5e8; color: #2e7d32; }
    .status-inactive { background-color: #fff3e0; color: #f57c00; }
    .status-graduated { background-color: #e3f2fd; color: #1976d2; }
    .status-withdrawn { background-color: #ffebee; color: #d32f2f; }
    .status-onprobation { background-color: #fff8e1; color: #f9a825; }
    
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
  `]
})
export class StudentListComponent implements OnInit {
  students: StudentDto[] = [];
  paginationData: PaginatedStudentListDto | null = null;
  isLoading = false;
  
  displayedColumns: string[] = ['fullName', 'email', 'phoneNumber', 'status', 'gpa', 'enrollmentDate', 'actions'];
  
  searchQuery: GetStudentListQuery = {
    pageNumber: 1,
    pageSize: 10,
    searchTerm: '',
    sortBy: 'fullName',
    sortDirection: 'asc'
  };

  constructor(
    private studentService: StudentService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadStudents();
  }

  loadStudents(): void {
    this.isLoading = true;
    
    this.studentService.getStudents(this.searchQuery).subscribe({
      next: (response) => {
        this.paginationData = response;
        this.students = response.students;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading students:', error);
        this.snackBar.open('Error loading students', 'Close', { duration: 3000 });
        this.isLoading = false;
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.searchQuery.pageNumber = event.pageIndex + 1;
    this.searchQuery.pageSize = event.pageSize;
    this.loadStudents();
  }

  clearFilters(): void {
    this.searchQuery = {
      pageNumber: 1,
      pageSize: 10,
      searchTerm: '',
      sortBy: 'fullName',
      sortDirection: 'asc'
    };
    this.loadStudents();
  }

  deleteStudent(student: StudentDto): void {
    if (confirm(`Are you sure you want to delete ${student.fullName}?`)) {
      this.studentService.deleteStudent(student.id).subscribe({
        next: () => {
          this.snackBar.open('Student deleted successfully', 'Close', { duration: 3000 });
          this.loadStudents();
        },
        error: (error) => {
          console.error('Error deleting student:', error);
          this.snackBar.open('Error deleting student', 'Close', { duration: 3000 });
        }
      });
    }
  }
}
