import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from '../../../shared/components/header/header';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { StatCardComponent } from '../../../shared/components/stat-card/stat-card';
import { ScheduleTimelineComponent } from '../components/schedule-timeline/schedule-timeline';
import { RecentActivityLogComponent } from '../components/recent-activity-log/recent-activity-log';
import { DashboardService, StudentSubmission } from '../services/dashboard.service';
import { ToastService } from '../../../core/services/toast.service';
import {
  DashboardMetric,
  ScheduleItem,
  ActivityLogItem
} from '../models/dashboard.model';

@Component({
  selector: 'app-instructor-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    HeaderComponent,
    SidebarComponent,
    StatCardComponent,
    ScheduleTimelineComponent,
    RecentActivityLogComponent
  ],
  templateUrl: './instructor-dashboard.html',
  styleUrl: './instructor-dashboard.scss'
})
export class InstructorDashboardComponent implements OnInit {
  private dashboardService = inject(DashboardService);
  private toastService = inject(ToastService);

  isSidebarCollapsed = false;
  isDarkMode = false;

  metrics: DashboardMetric[] = [];
  schedule: ScheduleItem[] = [];
  activities: ActivityLogItem[] = [];
  submissions: StudentSubmission[] = [];

  ngOnInit() {
    this.loadInstructorData();
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

  loadInstructorData() {
    this.dashboardService.getInstructorMetrics().subscribe(m => (this.metrics = m));
    this.dashboardService.getScheduleTimeline().subscribe(s => (this.schedule = s));
    this.dashboardService.getRecentActivity().subscribe(act => (this.activities = act));
    this.dashboardService.getStudentSubmissions().subscribe(sub => (this.submissions = sub));
  }

  gradeSubmission(id: string, studentName: string) {
    this.submissions = this.submissions.filter(s => s.id !== id);
    this.toastService.success(`Graded assignment for ${studentName}`);
  }

  onSidebarToggle(collapsed: boolean) {
    this.isSidebarCollapsed = collapsed;
  }

  createNewLecture() {
    this.toastService.info('Opening Lecture Composer...');
  }
}
