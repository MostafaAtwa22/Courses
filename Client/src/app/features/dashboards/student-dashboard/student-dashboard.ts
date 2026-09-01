import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from '../../../shared/components/header/header';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { CourseService } from '../../courses/services/course.service';
import { ProgressService } from '../../courses/services/progress.service';
import { CourseSummary, CourseProgressSummary } from '../../courses/models/course.models';
import { CourseQueryParams } from '../../../shared/models/query-params.model';
import { Observable, forkJoin, map } from 'rxjs';

export interface EnrolledCourseWithProgress extends CourseSummary {
  progress?: CourseProgressSummary;
  progressPercentage?: number;
}

@Component({
  selector: 'app-student-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    HeaderComponent,
    SidebarComponent
  ],
  templateUrl: './student-dashboard.html',
  styleUrl: './student-dashboard.scss'
})
export class StudentDashboardComponent implements OnInit {
  private courseService = inject(CourseService);
  private progressService = inject(ProgressService);

  isSidebarCollapsed = false;
  enrolledCourses: EnrolledCourseWithProgress[] = [];
  isLoading = true;

  ngOnInit() {
    this.loadStudentData();
  }

  loadStudentData() {
    const params: CourseQueryParams = {
      pageNumber: 1,
      pageSize: 20,
      sortBy: 'createdAt',
      sortDescending: true
    };

    forkJoin({
      courses: this.courseService.getCoursesByStudentId(params),
      progress: this.progressService.getMyCoursesProgress()
    }).subscribe({
      next: ({ courses, progress }) => {
        this.enrolledCourses = this.mergeCoursesWithProgress(courses.items || [], progress);
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading student dashboard data:', err);
        this.isLoading = false;
      }
    });
  }

  private mergeCoursesWithProgress(
    courses: CourseSummary[],
    progressList: CourseProgressSummary[]
  ): EnrolledCourseWithProgress[] {
    return courses.map(course => {
      const progress = progressList.find(p => p.courseId === course.id);
      return {
        ...course,
        progress,
        progressPercentage: progress ? progress.percentComplete : 0
      };
    });
  }

  onSidebarToggle(collapsed: boolean) {
    this.isSidebarCollapsed = collapsed;
  }

  getProgressColor(percentage: number): string {
    if (percentage >= 75) return 'bg-success';
    if (percentage >= 50) return 'bg-primary';
    if (percentage >= 25) return 'bg-warning';
    return 'bg-danger';
  }

  getAverageProgress(): number {
    if (this.enrolledCourses.length === 0) return 0;
    const total = this.enrolledCourses.reduce((sum, course) => sum + (course.progressPercentage || 0), 0);
    return Math.round(total / this.enrolledCourses.length);
  }

  getProgressBadgeClass(percentage: number): string {
    if (percentage === 100) return 'badge-completed';
    if (percentage > 0) return 'badge-in-progress';
    return 'badge-not-started';
  }

  getProgressLabel(percentage: number): string {
    if (percentage === 100) return 'Completed';
    if (percentage > 0) return 'In Progress';
    return 'Not Started';
  }
}
