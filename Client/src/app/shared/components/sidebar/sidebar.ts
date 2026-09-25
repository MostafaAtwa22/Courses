import { Component, Input, Output, EventEmitter, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../features/auth/services/auth.service';
import { InstructorService } from '../../../features/instructors/services/instructor.service';
import { InstructorPrivateResponse } from '../../../features/instructors/models/instructor.models';

export interface NavItem {
  label: string;
  icon: string;
  route: string;
  badge?: string;
  badgeColor?: string;
  isSection?: false;
  exactMatch?: boolean;
  disabled?: boolean;
  requiresVerification?: boolean;
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
export class SidebarComponent implements OnInit {
  @Input() role: 'admin' | 'instructor' | 'student' | 'general' = 'general';
  @Input() isCollapsed = false;
  @Output() toggleCollapse = new EventEmitter<boolean>();

  private authService = inject(AuthService);
  private instructorService = inject(InstructorService);

  instructorStatus: string = 'Pending';
  isInstructorVerified = false;
  isLoadingStatus = true;

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
    { label: 'Instructor Hub', icon: 'fa-solid fa-chalkboard-user', route: '/instructor/dashboard/overview', exactMatch: true, requiresVerification: true },
    { label: 'Courses', isSection: true },
    { label: 'My Courses', icon: 'fa-solid fa-book-open', route: '/instructor/dashboard/courses', requiresVerification: true },
    { label: 'Teaching', isSection: true },
    { label: 'Courses Schedules', icon: 'fa-solid fa-calendar-days', route: '/instructor/schedule', requiresVerification: true },
    { label: 'Grading & Queue', icon: 'fa-solid fa-check-to-slot', route: '/instructor/grading', badge: '8 New', badgeColor: 'bg-danger', requiresVerification: true },
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

  ngOnInit() {
    if (this.role === 'instructor') {
      this.loadInstructorStatus();
    } else {
      this.isLoadingStatus = false;
    }
  }

  loadInstructorStatus() {
    this.instructorService.getCurrentInstructor().subscribe({
      next: (instructor) => {
        if (instructor) {
          this.instructorStatus = instructor.status;
          this.isInstructorVerified = instructor.status === 'Verfied';
          this.updateNavItemsBasedOnStatus();
        }
        this.isLoadingStatus = false;
      },
      error: (error) => {
        console.error('Error loading instructor status:', error);
        this.isLoadingStatus = false;
      }
    });
  }

  updateNavItemsBasedOnStatus() {
    if (!this.isInstructorVerified) {
      this.instructorNavItems = this.instructorNavItems.map(item => {
        if (!item.isSection && (item as NavItem).requiresVerification) {
          return { ...(item as NavItem), disabled: true };
        }
        return item;
      });
    }
  }

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

  isNavItemDisabled(item: NavItemOrSection): boolean {
    return !item.isSection && ((item as NavItem).disabled === true);
  }

  getNavItemRoute(item: NavItemOrSection): string {
    if (item.isSection) return '';
    const navItem = item as NavItem;
    return navItem.disabled === true ? '' : navItem.route;
  }

  getNavItemTitle(item: NavItemOrSection): string {
    if (item.isSection) return '';
    const navItem = item as NavItem;
    if (this.isCollapsed) return navItem.label;
    return navItem.disabled === true ? 'Requires instructor verification' : navItem.label;
  }

  shouldShowBadge(item: NavItemOrSection): boolean {
    if (item.isSection) return false;
    const navItem = item as NavItem;
    return !!navItem.badge && !this.isCollapsed && navItem.disabled !== true;
  }

  shouldShowGuard(item: NavItemOrSection): boolean {
    if (item.isSection) return false;
    const navItem = item as NavItem;
    return navItem.disabled === true && !this.isCollapsed;
  }
}
