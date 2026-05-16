import { Routes } from '@angular/router';
import { CourseListComponent } from './components/course-list.component';
import { CourseFormComponent } from './components/course-form.component';

export const COURSE_ROUTES: Routes = [
  {
    path: '',
    component: CourseListComponent,
    title: 'Courses'
  },
  {
    path: 'create',
    component: CourseFormComponent,
    title: 'Create Course'
  },
  {
    path: ':id/edit',
    component: CourseFormComponent,
    title: 'Edit Course'
  }
];