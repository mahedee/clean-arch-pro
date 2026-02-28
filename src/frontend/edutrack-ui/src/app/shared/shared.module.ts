import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MaterialModule } from './material.module';

// Shared components
import { ConfirmationDialogComponent } from './components/confirmation-dialog/confirmation-dialog.component';
import { LoadingSpinnerComponent } from './components/loading-spinner/loading-spinner.component';
import { PageHeaderComponent } from './components/page-header/page-header.component';

const SHARED_COMPONENTS = [
  ConfirmationDialogComponent,
  LoadingSpinnerComponent,
  PageHeaderComponent
];

const SHARED_MODULES = [
  CommonModule,
  ReactiveFormsModule,
  FormsModule,
  RouterModule,
  MaterialModule
];

@NgModule({
  declarations: [
    // Shared components are now standalone, so no declarations needed
  ],
  imports: [
    ...SHARED_MODULES,
    ...SHARED_COMPONENTS
  ],
  exports: [
    ...SHARED_MODULES,
    ...SHARED_COMPONENTS
  ]
})
export class SharedModule { }