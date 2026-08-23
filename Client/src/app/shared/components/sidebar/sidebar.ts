import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

export interface NavItem {
  label: string;
  icon: string;
  route: string;
  badge?: string;
  badgeColor?: string;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss'
})
export class SidebarComponent {
  @Input() role: 'admin' | 'instructor' | 'student' | 'general' = 'general';
  @Input() isCollapsed = false;
  @Output() toggleCollapse = new EventEmitter<boolean>();

  adminNavItems: NavItem[] = [
    { label: 'Admin Overview', icon: 'fa-solid fa-chart-line', route: '/admin/dashboard' },
    { label: 'Instructors', icon: 'fa-solid fa-user-tie', route: '/admin/instructors', badge: '5 Pending', badgeColor: 'bg-warning' },
    { label: 'Courses Catalog', icon: 'fa-solid fa-book-open', route: '/courses' },
    { label: 'User Management', icon: 'fa-solid fa-users-gear', route: '/admin/users' },
    { label: 'System Settings', icon: 'fa-solid fa-sliders', route: '/settings' }
  ];

  instructorNavItems: NavItem[] = [
    { label: 'Instructor Hub', icon: 'fa-solid fa-chalkboard-user', route: '/instructor/dashboard' },
    { label: 'My Courses', icon: 'fa-solid fa-laptop-code', route: '/courses' },
    { label: 'Class Schedules', icon: 'fa-solid fa-calendar-days', route: '/instructor/schedule' },
    { label: 'Grading & Queue', icon: 'fa-solid fa-check-to-slot', route: '/instructor/grading', badge: '8 New', badgeColor: 'bg-danger' },
    { label: 'My Profile', icon: 'fa-solid fa-id-card', route: '/profile' }
  ];

  studentNavItems: NavItem[] = [
    { label: 'My Courses', icon: 'fa-solid fa-laptop-code', route: '/student/dashboard' },
    { label: 'All Courses', icon: 'fa-solid fa-book-open', route: '/courses' },
    { label: 'My Profile', icon: 'fa-solid fa-id-card', route: '/profile' },
    { label: 'Settings', icon: 'fa-solid fa-sliders', route: '/settings' }
  ];

  generalNavItems: NavItem[] = [
    { label: 'Home', icon: 'fa-solid fa-house', route: '/' },
    { label: 'All Courses', icon: 'fa-solid fa-graduation-cap', route: '/courses' },
    { label: 'My Profile', icon: 'fa-solid fa-user', route: '/profile' },
    { label: 'Settings', icon: 'fa-solid fa-gear', route: '/settings' }
  ];

  get navItems(): NavItem[] {
    if (this.role === 'admin') return this.adminNavItems;
    if (this.role === 'instructor') return this.instructorNavItems;
    if (this.role === 'student') return this.studentNavItems;
    return this.generalNavItems;
  }

  onToggle() {
    this.isCollapsed = !this.isCollapsed;
    this.toggleCollapse.emit(this.isCollapsed);
  }
}
