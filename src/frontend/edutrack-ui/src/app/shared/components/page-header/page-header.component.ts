import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-page-header',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, RouterModule],
  template: `
    <div class="page-header">
      <h1 class="page-title">{{ title }}</h1>
      <div class="page-actions" *ngIf="showActions">
        <ng-content></ng-content>
      </div>
    </div>
  `,
  styles: [`
    .page-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 24px;
      padding-bottom: 16px;
      border-bottom: 1px solid #e0e0e0;
    }
    .page-title {
      margin: 0;
      font-size: 2rem;
      font-weight: 400;
      color: #333;
    }
    .page-actions {
      display: flex;
      gap: 8px;
    }
  `]
})
export class PageHeaderComponent {
  @Input() title: string = '';
  @Input() showActions: boolean = true;
}
