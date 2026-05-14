import { Routes } from '@angular/router';

export const DEPARTMENT_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/department-list-page.component')
      .then(m => m.DepartmentListPageComponent),
    title: 'Departments | EduTrack'
  },
  {
    path: 'create',
    loadComponent: () => import('./pages/department-create-page.component')
      .then(m => m.DepartmentCreatePageComponent),
    title: 'Create Department | EduTrack'
  },
  {
    path: ':id',
    loadComponent: () => import('./pages/department-detail-page.component')
      .then(m => m.DepartmentDetailPageComponent),
    title: 'Department Details | EduTrack'
  },
  {
    path: ':id/edit',
    loadComponent: () => import('./pages/department-create-page.component')
      .then(m => m.DepartmentCreatePageComponent),
    title: 'Edit Department | EduTrack'
  }
];
