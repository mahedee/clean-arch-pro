import { Component, OnInit, OnDestroy } from '@angular/core';
import { RouterOutlet, RouterModule, Router, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../shared/material.module';
import { filter, Subscription } from 'rxjs';

interface NavGroup {
  id: string;
  label: string;
  icon: string;
  children: NavItem[];
}

interface NavItem {
  label: string;
  icon: string;
  route: string;
}

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, RouterModule, MaterialModule, CommonModule],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss'
})
export class MainLayoutComponent implements OnInit, OnDestroy {
  openGroups = new Set<string>();
  private routerSub!: Subscription;

  navGroups: NavGroup[] = [
    {
      id: 'academic',
      label: 'Academic Management',
      icon: 'school',
      children: [
        { label: 'Departments', icon: 'business', route: '/departments' },
        { label: 'Courses',     icon: 'book',     route: '/courses'     },
      ]
    },
    {
      id: 'people',
      label: 'People',
      icon: 'groups',
      children: [
        { label: 'Students', icon: 'person', route: '/students' },
        { label: 'Teachers', icon: 'badge',  route: '/teachers' },
      ]
    },
    {
      id: 'tracking',
      label: 'Tracking',
      icon: 'track_changes',
      children: [
        { label: 'Attendance', icon: 'how_to_reg', route: '/attendance' },
        { label: 'Grades',     icon: 'grade',      route: '/grades'     },
      ]
    }
  ];

  constructor(private router: Router) {}

  ngOnInit(): void {
    this.expandActiveGroup(this.router.url);
    this.routerSub = this.router.events.pipe(
      filter(e => e instanceof NavigationEnd)
    ).subscribe((e: any) => this.expandActiveGroup(e.urlAfterRedirects));
  }

  ngOnDestroy(): void {
    this.routerSub?.unsubscribe();
  }

  toggleGroup(id: string): void {
    if (this.openGroups.has(id)) {
      this.openGroups.delete(id);
    } else {
      this.openGroups.add(id);
    }
  }

  isGroupOpen(id: string): boolean {
    return this.openGroups.has(id);
  }

  private expandActiveGroup(url: string): void {
    for (const group of this.navGroups) {
      if (group.children.some(c => url.startsWith(c.route))) {
        this.openGroups.add(group.id);
      }
    }
  }
}
