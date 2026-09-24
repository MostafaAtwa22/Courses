import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ScheduleTimelineComponent } from '../../components/schedule-timeline/schedule-timeline';
import { RecentActivityLogComponent } from '../../components/recent-activity-log/recent-activity-log';
import { InstructorBannerComponent } from './instructor-banner/instructor-banner.component';
import { InstructorStatsCardsComponent } from './instructor-stats-cards/instructor-stats-cards.component';
import { DashboardService, StudentSubmission } from '../../services/dashboard.service';
import { InstructorDashboardService } from '../../services/instructor-dashboard.service';
import {
  DashboardMetric,
  ScheduleItem,
  ActivityLogItem
} from '../../models/dashboard.model';
import { ToastService } from '../../../../core/services/toast.service';

@Component({
  selector: 'app-instructor-overview',
  standalone: true,
  imports: [
    CommonModule,
    ScheduleTimelineComponent,
    RecentActivityLogComponent,
    InstructorBannerComponent,
    InstructorStatsCardsComponent
  ],
  templateUrl: './instructor-overview.component.html',
  styleUrl: './instructor-overview.component.scss'
})
export class InstructorOverviewComponent implements OnInit {
  private dashboardService = inject(DashboardService);
  private instructorDashboardService = inject(InstructorDashboardService);
  private toastService = inject(ToastService);

  metrics: DashboardMetric[] = [];
  schedule: ScheduleItem[] = [];
  activities: ActivityLogItem[] = [];
  submissions: StudentSubmission[] = [];

  ngOnInit() {
    this.loadInstructorData();
  }

  loadInstructorData() {
    this.instructorDashboardService.getInstructorStatistics().subscribe(m => (this.metrics = m));
    this.dashboardService.getScheduleTimeline().subscribe(s => (this.schedule = s));
    this.dashboardService.getRecentActivity().subscribe(act => (this.activities = act));
    this.dashboardService.getStudentSubmissions().subscribe(sub => (this.submissions = sub));
  }

  gradeSubmission(id: string, studentName: string) {
    this.submissions = this.submissions.filter(s => s.id !== id);
    this.toastService.success(`Graded assignment for ${studentName}`);
  }

  createNewLecture() {
    this.toastService.info('Opening Lecture Composer...');
  }
}