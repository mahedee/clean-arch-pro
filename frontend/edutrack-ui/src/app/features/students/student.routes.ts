import { Routes } from '@angular/router';

export const STUDENT_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/student-list-page.component')
      .then(m => m.StudentListPageComponent),
    title: 'Students | EduTrack'
  },
  {
    path: 'create',
    loadComponent: () => import('./pages/student-create-page.component')
      .then(m => m.StudentCreatePageComponent),
    title: 'Create Student | EduTrack'
  },
  {
    path: ':id',
    loadComponent: () => import('./pages/student-detail-page.component')
      .then(m => m.StudentDetailPageComponent),
    title: 'Student Details | EduTrack'
  },
  {
    path: ':id/edit',
    loadComponent: () => import('./pages/student-create-page.component')
      .then(m => m.StudentCreatePageComponent),
    title: 'Edit Student | EduTrack'
  }
];
