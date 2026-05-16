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

import { TeacherService } from '../services/teacher.service';
import { ITeacherListItem, ITeacherListQuery, IPaginatedTeacherList, TeacherStatus } from '../../../shared/interfaces';
import { ConfirmationDialogComponent, ConfirmationDialogData } from '../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';

@Component({
  selector: 'app-teacher-list-page',
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
    MatDialogModule,
    ConfirmationDialogComponent,
    LoadingSpinnerComponent,
    PageHeaderComponent
  ],
  template: `
    <app-page-header title="Teachers">
      <button mat-raised-button color="primary" routerLink="/teachers/create">
        <mat-icon>add</mat-icon>
        Add Teacher
      </button>
    </app-page-header>

    <mat-card>
      <mat-card-content>
        <div class="filters-section">
          <mat-form-field appearance="outline">
            <mat-label>Search Teachers</mat-label>
            <input matInput [(ngModel)]="searchQuery.searchTerm" (keyup.enter)="loadTeachers()" placeholder="Search by name or email">
            <mat-icon matSuffix>search</mat-icon>
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Department</mat-label>
            <input matInput [(ngModel)]="searchQuery.department" (keyup.enter)="loadTeachers()" placeholder="Filter by department">
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Status</mat-label>
            <mat-select [(ngModel)]="searchQuery.status" (selectionChange)="loadTeachers()">
              <mat-option [value]="undefined">All Statuses</mat-option>
              <mat-option *ngFor="let s of teacherStatuses" [value]="s">{{s}}</mat-option>
            </mat-select>
          </mat-form-field>

          <div class="filter-actions">
            <button mat-button (click)="clearFilters()">Clear Filters</button>
            <button mat-raised-button color="primary" (click)="loadTeachers()">Search</button>
          </div>
        </div>

        <app-loading-spinner *ngIf="isLoading" message="Loading teachers..." [diameter]="50"></app-loading-spinner>

        <div class="table-container" *ngIf="!isLoading">
          <table mat-table [dataSource]="teachers" class="teachers-table">

            <ng-container matColumnDef="fullName">
              <th mat-header-cell *matHeaderCellDef>Name</th>
              <td mat-cell *matCellDef="let t">{{t.fullName}}</td>
            </ng-container>

            <ng-container matColumnDef="employeeId">
              <th mat-header-cell *matHeaderCellDef>Employee ID</th>
              <td mat-cell *matCellDef="let t">{{t.employeeId}}</td>
            </ng-container>

            <ng-container matColumnDef="email">
              <th mat-header-cell *matHeaderCellDef>Email</th>
              <td mat-cell *matCellDef="let t">{{t.email}}</td>
            </ng-container>

            <ng-container matColumnDef="department">
              <th mat-header-cell *matHeaderCellDef>Department</th>
              <td mat-cell *matCellDef="let t">{{t.department}}</td>
            </ng-container>

            <ng-container matColumnDef="title">
              <th mat-header-cell *matHeaderCellDef>Title</th>
              <td mat-cell *matCellDef="let t">{{t.title}}</td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let t">{{t.status}}</td>
            </ng-container>

            <ng-container matColumnDef="courseLoad">
              <th mat-header-cell *matHeaderCellDef>Course Load</th>
              <td mat-cell *matCellDef="let t">{{t.currentCourseLoad}}</td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>Actions</th>
              <td mat-cell *matCellDef="let t">
                <button mat-icon-button color="primary" [routerLink]="['/teachers', t.id]" matTooltip="View">
                  <mat-icon>visibility</mat-icon>
                </button>
                <button mat-icon-button color="accent" [routerLink]="['/teachers', t.id, 'edit']" matTooltip="Edit">
                  <mat-icon>edit</mat-icon>
                </button>
                <button mat-icon-button color="warn" (click)="confirmDelete(t)" matTooltip="Delete">
                  <mat-icon>delete</mat-icon>
                </button>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>

          <div class="no-data" *ngIf="teachers.length === 0">
            <mat-icon>school</mat-icon>
            <p>No teachers found.</p>
          </div>
        </div>

        <mat-paginator
          [length]="totalCount"
          [pageSize]="searchQuery.pageSize || 10"
          [pageSizeOptions]="[5, 10, 25, 50]"
          (page)="onPageChange($event)"
          *ngIf="totalCount > 0">
        </mat-paginator>
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .filters-section { display: flex; gap: 16px; flex-wrap: wrap; margin-bottom: 16px; align-items: flex-start; }
    .filter-actions { display: flex; gap: 8px; align-items: center; padding-top: 4px; }
    .table-container { overflow-x: auto; }
    .teachers-table { width: 100%; }
    .no-data { text-align: center; padding: 48px; color: #666; }
    .no-data mat-icon { font-size: 64px; height: 64px; width: 64px; }
  `]
})
export class TeacherListPageComponent implements OnInit {
  teachers: ITeacherListItem[] = [];
  totalCount = 0;
  isLoading = false;
  displayedColumns = ['fullName', 'employeeId', 'email', 'department', 'title', 'status', 'courseLoad', 'actions'];
  teacherStatuses = Object.values(TeacherStatus);
  searchQuery: ITeacherListQuery = { pageNumber: 1, pageSize: 10 };

  constructor(
    private teacherService: TeacherService,
    private router: Router,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadTeachers();
  }

  loadTeachers(): void {
    this.isLoading = true;
    this.teacherService.getTeachers(this.searchQuery).subscribe({
      next: (result) => {
        this.teachers = result.teachers || [];
        this.totalCount = result.totalCount;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.snackBar.open('Failed to load teachers.', 'Close', { duration: 3000 });
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.searchQuery = { ...this.searchQuery, pageNumber: event.pageIndex + 1, pageSize: event.pageSize };
    this.loadTeachers();
  }

  clearFilters(): void {
    this.searchQuery = { pageNumber: 1, pageSize: 10 };
    this.loadTeachers();
  }

  confirmDelete(teacher: ITeacherListItem): void {
    const data: ConfirmationDialogData = {
      title: 'Delete Teacher',
      message: `Are you sure you want to delete ${teacher.fullName}?`,
      confirmButtonText: 'Delete',
      cancelButtonText: 'Cancel'
    };
    const ref = this.dialog.open(ConfirmationDialogComponent, { data });
    ref.afterClosed().subscribe(confirmed => {
      if (confirmed) this.deleteTeacher(teacher.id);
    });
  }

  deleteTeacher(id: string): void {
    this.teacherService.deleteTeacher(id).subscribe({
      next: () => {
        this.snackBar.open('Teacher deleted successfully.', 'Close', { duration: 3000 });
        this.loadTeachers();
      },
      error: () => this.snackBar.open('Failed to delete teacher.', 'Close', { duration: 3000 })
    });
  }
}
