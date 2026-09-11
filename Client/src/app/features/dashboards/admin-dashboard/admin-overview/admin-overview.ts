import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StatCardComponent } from '../../../../shared/components/stat-card/stat-card';
import { CourseAnalyticsComponent } from '../../components/course-analytics/course-analytics';
import { PendingInstructorsCardComponent } from './pending-instructors-card/pending-instructors-card.component';
import { CategoryHistogramCardComponent } from './category-histogram-card/category-histogram-card.component';
import { AdminDashboardService, PendingInstructor } from '../../services/admin-dashboard.service';
import { DashboardService } from '../../services/dashboard.service';
import {
  DashboardMetric,
  CourseAnalytics,
  CategoryHistogramData
} from '../../models/dashboard.model';

@Component({
  selector: 'app-admin-overview',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    StatCardComponent,
    CourseAnalyticsComponent,
    CategoryHistogramCardComponent,
    PendingInstructorsCardComponent
  ],
  templateUrl: './admin-overview.html',
  styleUrl: './admin-overview.scss'
})
export class AdminOverviewComponent implements OnInit {
  private dashboardService = inject(DashboardService);
  private adminDashboardService = inject(AdminDashboardService);

  metrics: DashboardMetric[] = [];
  roleMetrics: DashboardMetric[] = [];
  courses: CourseAnalytics[] = [];
  pendingInstructors: PendingInstructor[] = [];
  categoryHistogramData: CategoryHistogramData[] = [];

  ngOnInit() {
    this.loadAdminData();
  }

  loadAdminData() {
    this.dashboardService.getAdminMetrics().subscribe(m => (this.metrics = m));
    this.adminDashboardService.getRoleStatistics().subscribe(rm => (this.roleMetrics = rm));
    this.adminDashboardService.getTopPerformingCourses().subscribe(c => (this.courses = c));
    this.adminDashboardService.getPendingInstructors().subscribe(pi => (this.pendingInstructors = pi));
    this.adminDashboardService.getTopCategoriesByCourseCount().subscribe(ch => (this.categoryHistogramData = ch));
  }
}
