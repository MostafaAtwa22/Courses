import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StatCardComponent } from '../../../../shared/components/stat-card/stat-card';
import { CourseAnalyticsComponent } from '../../components/course-analytics/course-analytics';
import { AttendanceOverviewComponent } from '../../components/attendance-overview/attendance-overview';
import { RecentActivityLogComponent } from '../../components/recent-activity-log/recent-activity-log';
import { DashboardService, PendingInstructor } from '../../services/dashboard.service';
import { ToastService } from '../../../../core/services/toast.service';
import {
  DashboardMetric,
  CourseAnalytics,
  AttendanceSummary,
  ActivityLogItem
} from '../../models/dashboard.model';

@Component({
  selector: 'app-admin-overview',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    StatCardComponent,
    CourseAnalyticsComponent,
    AttendanceOverviewComponent,
    RecentActivityLogComponent
  ],
  templateUrl: './admin-overview.html',
  styleUrl: './admin-overview.scss'
})
export class AdminOverviewComponent implements OnInit {
  private dashboardService = inject(DashboardService);
  private toastService = inject(ToastService);

  metrics: DashboardMetric[] = [];
  courses: CourseAnalytics[] = [];
  attendance: AttendanceSummary | null = null;
  activities: ActivityLogItem[] = [];
  pendingInstructors: PendingInstructor[] = [];
  
  // Pagination for pending instructors
  pendingInstructorsPageSize = 5;
  pendingInstructorsCurrentPage = 1;

  ngOnInit() {
    this.loadAdminData();
  }

  loadAdminData() {
    this.dashboardService.getAdminMetrics().subscribe(m => (this.metrics = m));
    this.dashboardService.getCoursesAnalytics().subscribe(c => (this.courses = c));
    this.dashboardService.getAttendanceSummary().subscribe(a => (this.attendance = a));
    this.dashboardService.getRecentActivity().subscribe(act => (this.activities = act));
    this.dashboardService.getPendingInstructors().subscribe(pi => (this.pendingInstructors = pi));
  }

  get paginatedPendingInstructors(): PendingInstructor[] {
    const startIndex = (this.pendingInstructorsCurrentPage - 1) * this.pendingInstructorsPageSize;
    const endIndex = startIndex + this.pendingInstructorsPageSize;
    return this.pendingInstructors.slice(startIndex, endIndex);
  }

  get pendingInstructorsTotalPages(): number {
    return Math.ceil(this.pendingInstructors.length / this.pendingInstructorsPageSize);
  }

  onPendingInstructorsPageChange(page: number) {
    if (page < 1 || page > this.pendingInstructorsTotalPages) return;
    this.pendingInstructorsCurrentPage = page;
  }

  getPendingInstructorsPagesArray(): number[] {
    return Array.from({ length: this.pendingInstructorsTotalPages }, (_, i) => i + 1);
  }

  approveInstructor(id: string, name: string) {
    this.pendingInstructors = this.pendingInstructors.filter(i => i.id !== id);
    this.toastService.success(`Approved verification for ${name}`);
    
    // Adjust pagination if needed
    if (this.paginatedPendingInstructors.length === 0 && this.pendingInstructorsCurrentPage > 1) {
      this.pendingInstructorsCurrentPage--;
    }
  }

  rejectInstructor(id: string, name: string) {
    this.pendingInstructors = this.pendingInstructors.filter(i => i.id !== id);
    this.toastService.info(`Verification declined for ${name}`);
    
    // Adjust pagination if needed
    if (this.paginatedPendingInstructors.length === 0 && this.pendingInstructorsCurrentPage > 1) {
      this.pendingInstructorsCurrentPage--;
    }
  }
}
