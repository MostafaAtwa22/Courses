import { Component, Input, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../features/auth/services/auth.service';

export interface NavItem {
  label: string;
  icon: string;
  route: string;
  badge?: string;
  badgeColor?: string;
  isSection?: false;
  exactMatch?: boolean;
}

export interface NavSection {
  label: string;
  isSection: true;
}

export type NavItemOrSection = NavItem | NavSection;

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

  private authService = inject(AuthService);

  adminNavItems: NavItemOrSection[] = [
    { label: 'Overview', isSection: true },
    { label: 'Admin Overview', icon: 'fa-solid fa-chart-line', route: '/admin/dashboard/overview' },
    { label: 'Users', isSection: true },
    { label: 'Admins', icon: 'fa-solid fa-user-shield', route: '/admin/dashboard/admins' },
    { label: 'Instructors', icon: 'fa-solid fa-user-tie', route: '/admin/dashboard/instructors'},
    { label: 'Students', icon: 'fa-solid fa-user-graduate', route: '/admin/dashboard/students' },
    { label: 'System Users', icon: 'fa-solid fa-users-gear', route: '/admin/dashboard/users' },
    { label: 'Content', isSection: true },
    { label: 'Categories', icon: 'fa-solid fa-tags', route: '/admin/dashboard/categories' },
    { label: 'Courses', icon: 'fa-solid fa-graduation-cap', route: '/admin/dashboard/courses' },
    { label: 'System', isSection: true },
    { label: 'System Settings', icon: 'fa-solid fa-sliders', route: '/settings' }
  ];

  instructorNavItems: NavItemOrSection[] = [
    { label: 'Overview', isSection: true },
    { label: 'Instructor Hub', icon: 'fa-solid fa-chalkboard-user', route: '/instructor/dashboard/overview', exactMatch: true },
    { label: 'Courses', isSection: true },
    { label: 'My Courses', icon: 'fa-solid fa-book-open', route: '/instructor/dashboard/courses' },
    { label: 'Teaching', isSection: true },
    { label: 'Courses Schedules', icon: 'fa-solid fa-calendar-days', route: '/instructor/schedule' },
    { label: 'Grading & Queue', icon: 'fa-solid fa-check-to-slot', route: '/instructor/grading', badge: '8 New', badgeColor: 'bg-danger' },
    { label: 'Account', isSection: true },
    { label: 'Update Instructor', icon: 'fa-solid fa-user-pen', route: '/instructor/dashboard/update-profile' },
    { label: 'My Profile', icon: 'fa-solid fa-id-card', route: '/profile' }
  ];

  studentNavItems: NavItemOrSection[] = [
    { label: 'My Courses', icon: 'fa-solid fa-laptop-code', route: '/student/dashboard' },
    { label: 'All Courses', icon: 'fa-solid fa-book-open', route: '/courses' },
    { label: 'My Profile', icon: 'fa-solid fa-id-card', route: '/profile' },
    { label: 'Settings', icon: 'fa-solid fa-sliders', route: '/settings' }
  ];

  generalNavItems: NavItemOrSection[] = [
    { label: 'Home', icon: 'fa-solid fa-house', route: '/' },
    { label: 'All Courses', icon: 'fa-solid fa-graduation-cap', route: '/courses' },
    { label: 'My Profile', icon: 'fa-solid fa-user', route: '/profile' },
    { label: 'Settings', icon: 'fa-solid fa-gear', route: '/settings' }
  ];

  get navItems(): NavItemOrSection[] {
    if (this.role === 'admin') return this.adminNavItems;
    if (this.role === 'instructor') return this.instructorNavItems;
    if (this.role === 'student') return this.studentNavItems;
    return this.generalNavItems;
  }

  onToggle() {
    this.isCollapsed = !this.isCollapsed;
    this.toggleCollapse.emit(this.isCollapsed);
  }

  onLogout() {
    this.authService.logout();
  }
}
