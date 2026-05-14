import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: DashboardComponent },
      { 
        path: 'students', 
        loadChildren: () => import('./features/students/student.routes').then(m => m.STUDENT_ROUTES)
      },
      { 
        path: 'courses', 
        loadChildren: () => import('./features/courses/course.routes').then(m => m.COURSE_ROUTES)
      },
      {
        path: 'teachers',
        loadChildren: () => import('./features/teachers/teacher.routes').then(m => m.TEACHER_ROUTES)
      },
      {
        path: 'departments',
        loadChildren: () => import('./features/departments/department.routes').then(m => m.DEPARTMENT_ROUTES)
      },
    ]
  }
];
