import { Routes } from '@angular/router';

export const TEACHER_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/teacher-list-page.component')
      .then(m => m.TeacherListPageComponent),
    title: 'Teachers | EduTrack'
  },
  {
    path: 'create',
    loadComponent: () => import('./pages/teacher-create-page.component')
      .then(m => m.TeacherCreatePageComponent),
    title: 'Create Teacher | EduTrack'
  },
  {
    path: ':id',
    loadComponent: () => import('./pages/teacher-detail-page.component')
      .then(m => m.TeacherDetailPageComponent),
    title: 'Teacher Details | EduTrack'
  },
  {
    path: ':id/edit',
    loadComponent: () => import('./pages/teacher-create-page.component')
      .then(m => m.TeacherCreatePageComponent),
    title: 'Edit Teacher | EduTrack'
  }
];
