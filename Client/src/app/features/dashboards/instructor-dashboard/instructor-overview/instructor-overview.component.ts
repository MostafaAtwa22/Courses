import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ScheduleTimelineComponent } from '../../components/schedule-timeline/schedule-timeline';
import { RecentActivityLogComponent } from '../../components/recent-activity-log/recent-activity-log';
import { InstructorBannerComponent } from './instructor-banner/instructor-banner.component';
import { InstructorStatsCardsComponent } from './instructor-stats-cards/instructor-stats-cards.component';
import { InstructorEnrollmentChartCardComponent } from './instructor-enrollment-chart-card/instructor-enrollment-chart-card.component';
import { InstructorEnrollmentSummaryTableComponent } from './instructor-enrollment-summary-table/instructor-enrollment-summary-table.component';
import { DashboardService, StudentSubmission } from '../../services/dashboard.service';
import { InstructorDashboardService, EnrollmentStatistics } from '../../services/instructor-dashboard.service';
import {
  DashboardMetric,
  ScheduleItem,
  ActivityLogItem
} from '../../models/dashboard.model';
import { ToastService } from '../../../../core/services/toast.service';
import { InstructorService } from '../../../../features/instructors/services/instructor.service';
import { InstructorPrivateResponse } from '../../../../features/instructors/models/instructor.models';

@Component({
  selector: 'app-instructor-overview',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    ScheduleTimelineComponent,
    RecentActivityLogComponent,
    InstructorBannerComponent,
    InstructorStatsCardsComponent,
    InstructorEnrollmentChartCardComponent,
    InstructorEnrollmentSummaryTableComponent
  ],
  templateUrl: './instructor-overview.component.html',
  styleUrl: './instructor-overview.component.scss'
})
export class InstructorOverviewComponent implements OnInit {
  private dashboardService = inject(DashboardService);
  private instructorDashboardService = inject(InstructorDashboardService);
  private toastService = inject(ToastService);
  private instructorService = inject(InstructorService);

  metrics: DashboardMetric[] = [];
  schedule: ScheduleItem[] = [];
  activities: ActivityLogItem[] = [];
  submissions: StudentSubmission[] = [];
  enrollmentData: EnrollmentStatistics[] = [];
  
  instructorStatus: string = 'Pending';
  currentInstructor: InstructorPrivateResponse | null = null;
  isLoadingStatus = true;

  ngOnInit() {
    this.loadInstructorStatus();
  }

  loadInstructorStatus() {
    this.instructorService.getCurrentInstructor().subscribe({
      next: (instructor) => {
        if (instructor) {
          this.currentInstructor = instructor;
          this.instructorStatus = instructor.status;
          this.isLoadingStatus = false;
          
          // Only load dashboard data if instructor is verified
          if (this.instructorStatus === 'Verfied') {
            this.loadInstructorData();
          }
        } else {
          this.isLoadingStatus = false;
        }
      },
      error: (error) => {
        console.error('Error loading instructor status:', error);
        this.isLoadingStatus = false;
      }
    });
  }

  loadInstructorData() {
    this.instructorDashboardService.getInstructorStatistics().subscribe(m => (this.metrics = m));
    this.instructorDashboardService.getInstructorEnrollmentStatistics().subscribe(e => (this.enrollmentData = e));
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

  getInstructorProfileUrl(): string {
    return this.currentInstructor ? `/instructors/${this.currentInstructor.id}` : '/profile';
  }
}