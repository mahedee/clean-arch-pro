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

import { DepartmentService } from '../services/department.service';
import { IDepartmentListItem, IDepartmentListQuery, DepartmentStatus } from '../../../shared/interfaces';
import { ConfirmationDialogComponent, ConfirmationDialogData } from '../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';

@Component({
  selector: 'app-department-list-page',
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
    <app-page-header title="Departments">
      <button mat-raised-button color="primary" routerLink="/departments/create">
        <mat-icon>add</mat-icon>
        Add Department
      </button>
    </app-page-header>

    <mat-card>
      <mat-card-content>
        <div class="filters-section">
          <mat-form-field appearance="outline">
            <mat-label>Search Departments</mat-label>
            <input matInput [(ngModel)]="searchQuery.searchTerm" (keyup.enter)="loadDepartments()" placeholder="Search by name or code">
            <mat-icon matSuffix>search</mat-icon>
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Status</mat-label>
            <mat-select [(ngModel)]="searchQuery.status" (selectionChange)="loadDepartments()">
              <mat-option [value]="undefined">All Statuses</mat-option>
              <mat-option *ngFor="let s of departmentStatuses" [value]="s">{{s}}</mat-option>
            </mat-select>
          </mat-form-field>

          <div class="filter-actions">
            <button mat-button (click)="clearFilters()">Clear Filters</button>
            <button mat-raised-button color="primary" (click)="loadDepartments()">Search</button>
          </div>
        </div>

        <app-loading-spinner *ngIf="isLoading" message="Loading departments..." [diameter]="50"></app-loading-spinner>

        <div class="table-container" *ngIf="!isLoading">
          <table mat-table [dataSource]="departments" class="departments-table">

            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>Name</th>
              <td mat-cell *matCellDef="let d">{{d.name}}</td>
            </ng-container>

            <ng-container matColumnDef="code">
              <th mat-header-cell *matHeaderCellDef>Code</th>
              <td mat-cell *matCellDef="let d">{{d.code}}</td>
            </ng-container>

            <ng-container matColumnDef="location">
              <th mat-header-cell *matHeaderCellDef>Location</th>
              <td mat-cell *matCellDef="let d">{{d.location || '-'}}</td>
            </ng-container>

            <ng-container matColumnDef="facultyCount">
              <th mat-header-cell *matHeaderCellDef>Faculty</th>
              <td mat-cell *matCellDef="let d">{{d.facultyCount}}</td>
            </ng-container>

            <ng-container matColumnDef="studentCount">
              <th mat-header-cell *matHeaderCellDef>Students</th>
              <td mat-cell *matCellDef="let d">{{d.studentCount}}</td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let d">{{d.status}}</td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>Actions</th>
              <td mat-cell *matCellDef="let d">
                <button mat-icon-button color="primary" [routerLink]="['/departments', d.id]" matTooltip="View">
                  <mat-icon>visibility</mat-icon>
                </button>
                <button mat-icon-button color="accent" [routerLink]="['/departments', d.id, 'edit']" matTooltip="Edit">
                  <mat-icon>edit</mat-icon>
                </button>
                <button mat-icon-button color="warn" (click)="confirmDelete(d)" matTooltip="Delete">
                  <mat-icon>delete</mat-icon>
                </button>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>

          <div class="no-data" *ngIf="departments.length === 0">
            <mat-icon>business</mat-icon>
            <p>No departments found.</p>
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
    .departments-table { width: 100%; }
    .no-data { text-align: center; padding: 48px; color: #666; }
    .no-data mat-icon { font-size: 64px; height: 64px; width: 64px; }
  `]
})
export class DepartmentListPageComponent implements OnInit {
  departments: IDepartmentListItem[] = [];
  totalCount = 0;
  isLoading = false;
  displayedColumns = ['name', 'code', 'location', 'facultyCount', 'studentCount', 'status', 'actions'];
  departmentStatuses = Object.values(DepartmentStatus);
  searchQuery: IDepartmentListQuery = { pageNumber: 1, pageSize: 10 };

  constructor(
    private departmentService: DepartmentService,
    private router: Router,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadDepartments();
  }

  loadDepartments(): void {
    this.isLoading = true;
    this.departmentService.getDepartments(this.searchQuery).subscribe({
      next: (result) => {
        this.departments = result.departments || [];
        this.totalCount = result.totalCount;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.snackBar.open('Failed to load departments.', 'Close', { duration: 3000 });
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.searchQuery = { ...this.searchQuery, pageNumber: event.pageIndex + 1, pageSize: event.pageSize };
    this.loadDepartments();
  }

  clearFilters(): void {
    this.searchQuery = { pageNumber: 1, pageSize: 10 };
    this.loadDepartments();
  }

  confirmDelete(department: IDepartmentListItem): void {
    const data: ConfirmationDialogData = {
      title: 'Delete Department',
      message: `Are you sure you want to delete "${department.name}"?`,
      confirmButtonText: 'Delete',
      cancelButtonText: 'Cancel'
    };
    const ref = this.dialog.open(ConfirmationDialogComponent, { data });
    ref.afterClosed().subscribe(confirmed => {
      if (confirmed) this.deleteDepartment(department.id);
    });
  }

  deleteDepartment(id: string): void {
    this.departmentService.deleteDepartment(id).subscribe({
      next: () => {
        this.snackBar.open('Department deleted successfully.', 'Close', { duration: 3000 });
        this.loadDepartments();
      },
      error: () => this.snackBar.open('Failed to delete department.', 'Close', { duration: 3000 })
    });
  }
}
