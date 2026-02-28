import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

// Feature routing
import { STUDENT_ROUTES } from './student.routes';

// Shared modules
import { SharedModule } from '../../shared/shared.module';

// Pages
import { StudentListPageComponent } from './pages/student-list-page.component';
import { StudentCreatePageComponent } from './pages/student-create-page.component';
import { StudentDetailPageComponent } from './pages/student-detail-page.component';

// Services
import { StudentService } from './services/student.service';

const STUDENT_COMPONENTS = [
  StudentListPageComponent,
  StudentCreatePageComponent,
  StudentDetailPageComponent
];

@NgModule({
  declarations: [
    // All components are standalone, no declarations needed
  ],
  imports: [
    CommonModule,
    SharedModule,
    RouterModule.forChild(STUDENT_ROUTES),
    ...STUDENT_COMPONENTS
  ],
  providers: [
    StudentService
  ]
})
export class StudentsModule { }
