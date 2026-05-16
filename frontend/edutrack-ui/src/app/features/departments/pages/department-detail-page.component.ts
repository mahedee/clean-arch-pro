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

import { DepartmentService } from '../services/department.service';
import { IDepartment } from '../../../shared/interfaces';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-department-detail-page',
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
    <app-page-header [title]="department?.name || 'Department Details'">
      <button mat-button routerLink="/departments">
        <mat-icon>arrow_back</mat-icon>
        Back to Departments
      </button>
      <button mat-raised-button color="primary" [routerLink]="['/departments', departmentId, 'edit']" *ngIf="department && !isLoading">
        <mat-icon>edit</mat-icon>
        Edit Department
      </button>
    </app-page-header>

    <app-loading-spinner *ngIf="isLoading" message="Loading department details..." [diameter]="50"></app-loading-spinner>

    <div *ngIf="!isLoading && department" class="detail-container">
      <mat-card class="info-card">
        <mat-card-header><mat-card-title>Department Information</mat-card-title></mat-card-header>
        <mat-card-content>
          <div class="info-grid">
            <div class="info-item"><label>Name:</label><span>{{department.name}}</span></div>
            <div class="info-item"><label>Code:</label><span>{{department.code}}</span></div>
            <div class="info-item" *ngIf="department.location"><label>Location:</label><span>{{department.location}}</span></div>
            <div class="info-item">
              <label>Status:</label>
              <mat-chip-set><mat-chip>{{department.status}}</mat-chip></mat-chip-set>
            </div>
            <div class="info-item" *ngIf="department.establishedDate"><label>Established:</label><span>{{department.establishedDate | date: 'longDate'}}</span></div>
            <div class="info-item"><label>Faculty Count:</label><span>{{department.facultyCount}}</span></div>
            <div class="info-item"><label>Student Count:</label><span>{{department.studentCount}}</span></div>
            <div class="info-item" *ngIf="department.budget"><label>Budget:</label><span>{{department.budget | currency}}</span></div>
          </div>
          <div *ngIf="department.description" class="description-section">
            <mat-divider></mat-divider>
            <p class="description">{{department.description}}</p>
          </div>
        </mat-card-content>
      </mat-card>

      <mat-card class="info-card" *ngIf="department.contactEmail || department.contactPhone">
        <mat-card-header><mat-card-title>Contact Information</mat-card-title></mat-card-header>
        <mat-card-content>
          <div class="info-grid">
            <div class="info-item" *ngIf="department.contactEmail"><label>Email:</label><span>{{department.contactEmail}}</span></div>
            <div class="info-item" *ngIf="department.contactPhone"><label>Phone:</label><span>{{department.contactPhone}}</span></div>
          </div>
        </mat-card-content>
      </mat-card>
    </div>

    <div *ngIf="!isLoading && !department" class="not-found">
      <mat-icon>error_outline</mat-icon>
      <p>Department not found.</p>
      <button mat-raised-button color="primary" routerLink="/departments">Back to Departments</button>
    </div>
  `,
  styles: [`
    .detail-container { display: flex; flex-direction: column; gap: 16px; }
    .info-card { margin-bottom: 0; }
    .info-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(260px, 1fr)); gap: 16px; padding: 8px 0; }
    .info-item { display: flex; flex-direction: column; gap: 4px; }
    .info-item label { font-weight: 600; color: #666; font-size: 0.85rem; }
    .description-section { margin-top: 16px; }
    .description { margin-top: 12px; color: #444; line-height: 1.6; }
    .not-found { text-align: center; padding: 64px; color: #666; }
    .not-found mat-icon { font-size: 64px; height: 64px; width: 64px; }
  `]
})
export class DepartmentDetailPageComponent implements OnInit {
  department: IDepartment | null = null;
  departmentId = '';
  isLoading = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private departmentService: DepartmentService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.departmentId = this.route.snapshot.paramMap.get('id') || '';
    if (this.departmentId) this.loadDepartment();
  }

  loadDepartment(): void {
    this.isLoading = true;
    this.departmentService.getDepartment(this.departmentId).subscribe({
      next: (d) => { this.department = d; this.isLoading = false; },
      error: () => { this.isLoading = false; this.snackBar.open('Failed to load department.', 'Close', { duration: 3000 }); }
    });
  }
}
