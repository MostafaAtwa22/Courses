import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HeaderComponent } from '../../../shared/components/header/header';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { StatCardComponent } from '../../../shared/components/stat-card/stat-card';
import { CourseAnalyticsComponent } from '../components/course-analytics/course-analytics';
import { AttendanceOverviewComponent } from '../components/attendance-overview/attendance-overview';
import { RecentActivityLogComponent } from '../components/recent-activity-log/recent-activity-log';
import { DashboardService, PendingInstructor } from '../services/dashboard.service';
import { ToastService } from '../../../core/services/toast.service';
import {
  DashboardMetric,
  CourseAnalytics,
  AttendanceSummary,
  ActivityLogItem
} from '../models/dashboard.model';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    HeaderComponent,
    SidebarComponent,
    StatCardComponent,
    CourseAnalyticsComponent,
    AttendanceOverviewComponent,
    RecentActivityLogComponent
  ],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.scss'
})
export class AdminDashboardComponent implements OnInit {
  private dashboardService = inject(DashboardService);
  private toastService = inject(ToastService);

  isSidebarCollapsed = false;
  isDarkMode = false;

  metrics: DashboardMetric[] = [];
  courses: CourseAnalytics[] = [];
  attendance: AttendanceSummary | null = null;
  activities: ActivityLogItem[] = [];
  pendingInstructors: PendingInstructor[] = [];

  ngOnInit() {
    this.loadAdminData();
    this.loadTheme();
  }

  loadTheme() {
    const savedTheme = localStorage.getItem('theme');
    this.isDarkMode = savedTheme === 'dark';
    this.applyTheme();
  }

  toggleTheme() {
    this.isDarkMode = !this.isDarkMode;
    localStorage.setItem('theme', this.isDarkMode ? 'dark' : 'light');
    this.applyTheme();
  }

  applyTheme() {
    const body = document.body;
    if (this.isDarkMode) {
      body.classList.add('dark');
      body.classList.add('dark-theme');
    } else {
      body.classList.remove('dark');
      body.classList.remove('dark-theme');
    }
  }

  loadAdminData() {
    this.dashboardService.getAdminMetrics().subscribe(m => (this.metrics = m));
    this.dashboardService.getCoursesAnalytics().subscribe(c => (this.courses = c));
    this.dashboardService.getAttendanceSummary().subscribe(a => (this.attendance = a));
    this.dashboardService.getRecentActivity().subscribe(act => (this.activities = act));
    this.dashboardService.getPendingInstructors().subscribe(pi => (this.pendingInstructors = pi));
  }

  approveInstructor(id: string, name: string) {
    this.pendingInstructors = this.pendingInstructors.filter(i => i.id !== id);
    this.toastService.success(`Approved verification for ${name}`);
  }

  rejectInstructor(id: string, name: string) {
    this.pendingInstructors = this.pendingInstructors.filter(i => i.id !== id);
    this.toastService.info(`Verification declined for ${name}`);
  }

  onSidebarToggle(collapsed: boolean) {
    this.isSidebarCollapsed = collapsed;
  }
}
