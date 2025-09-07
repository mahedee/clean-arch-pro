import { Routes } from '@angular/router';
import { StudentListComponent } from './components/student-list.component';
import { StudentFormComponent } from './components/student-form.component';
import { StudentDetailComponent } from './components/student-detail.component';

export const STUDENT_ROUTES: Routes = [
  {
    path: '',
    component: StudentListComponent
  },
  {
    path: 'create',
    component: StudentFormComponent
  },
  {
    path: ':id',
    component: StudentDetailComponent
  },
  {
    path: ':id/edit',
    component: StudentFormComponent
  }
];
