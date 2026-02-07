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

import { StudentService } from '../../../core/services/student.service';
import { Student, PaginatedStudentList, GetStudentListQuery, StudentStatus } from '../../../shared/models/student.model';

@Component({
  selector: 'app-student-list',
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
    MatToolbarModule
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
            <input matInput [(ngModel)]="searchQuery.searchTerm" (keyup.enter)="loadStudents()" placeholder="Search by name or email">
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Status</mat-label>
            <mat-select [(ngModel)]="searchQuery.status" (selectionChange)="loadStudents()">
              <mat-option value="">All</mat-option>
              <mat-option *ngFor="let status of studentStatuses" [value]="status">{{status}}</mat-option>
            </mat-select>
          </mat-form-field>

          <button mat-raised-button (click)="loadStudents()">Search</button>
          <button mat-button (click)="clearFilters()">Clear</button>
        </div>

        <!-- Students Table -->
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
            <td mat-cell *matCellDef="let student">{{student.phoneNumber}}</td>
          </ng-container>

          <ng-container matColumnDef="status">
            <th mat-header-cell *matHeaderCellDef>Status</th>
            <td mat-cell *matCellDef="let student">
              <span class="status-badge status-{{student.status.toLowerCase()}}">{{student.status}}</span>
            </td>
          </ng-container>

          <ng-container matColumnDef="enrollmentDate">
            <th mat-header-cell *matHeaderCellDef>Enrollment Date</th>
            <td mat-cell *matCellDef="let student">{{student.enrollmentDate | date: 'short'}}</td>
          </ng-container>

          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef>Actions</th>
            <td mat-cell *matCellDef="let student">
              <button mat-icon-button [routerLink]="['/students', student.id]" matTooltip="View">
                <mat-icon>visibility</mat-icon>
              </button>
              <button mat-icon-button [routerLink]="['/students', student.id, 'edit']" matTooltip="Edit">
                <mat-icon>edit</mat-icon>
              </button>
              <button mat-icon-button (click)="deleteStudent(student)" matTooltip="Delete" color="warn">
                <mat-icon>delete</mat-icon>
              </button>
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
          <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
        </table>

        <!-- Pagination -->
        <mat-paginator 
          [length]="totalCount"
          [pageSize]="pageSize"
          [pageSizeOptions]="[5, 10, 25, 50]"
          [pageIndex]="pageNumber - 1"
          (page)="onPageChange($event)"
          showFirstLastButtons>
        </mat-paginator>
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .header-actions {
      margin-left: auto;
    }

    .filters {
      display: flex;
      gap: 16px;
      margin-bottom: 16px;
      align-items: center;
    }

    .filters mat-form-field {
      min-width: 200px;
    }

    table {
      width: 100%;
    }

    .status-badge {
      padding: 4px 8px;
      border-radius: 4px;
      font-size: 12px;
      font-weight: 500;
    }

    .status-active { background-color: #e8f5e8; color: #2e7d2e; }
    .status-inactive { background-color: #f5f5f5; color: #666; }
    .status-graduated { background-color: #e3f2fd; color: #1976d2; }
    .status-suspended { background-color: #ffebee; color: #d32f2f; }

    mat-card-header {
      display: flex;
      align-items: center;
    }
  `]
})
export class StudentListComponent implements OnInit {
  students: Student[] = [];
  displayedColumns: string[] = ['fullName', 'email', 'phoneNumber', 'status', 'enrollmentDate', 'actions'];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  
  searchQuery: GetStudentListQuery = {
    pageNumber: 1,
    pageSize: 10,
    sortBy: 'FullName',
    sortDirection: 'asc'
  };

  studentStatuses = Object.values(StudentStatus);

  constructor(private studentService: StudentService) {}

  ngOnInit(): void {
    this.loadStudents();
  }

  loadStudents(): void {
    this.studentService.getStudents(this.searchQuery).subscribe({
      next: (result: PaginatedStudentList) => {
        this.students = result.students;
        this.totalCount = result.totalCount;
        this.pageNumber = result.pageNumber;
        this.pageSize = result.pageSize;
      },
      error: (error) => {
        console.error('Error loading students:', error);
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
      sortBy: 'FullName',
      sortDirection: 'asc'
    };
    this.loadStudents();
  }

  deleteStudent(student: Student): void {
    if (confirm(`Are you sure you want to delete ${student.fullName}?`)) {
      this.studentService.deleteStudent(student.id).subscribe({
        next: () => {
          this.loadStudents();
        },
        error: (error) => {
          console.error('Error deleting student:', error);
        }
      });
    }
  }
}
