import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CourseService } from '../../../courses/services/course.service';
import { ProgressService } from '../../../courses/services/progress.service';
import { CourseSummary, CourseProgressSummary } from '../../../courses/models/course.models';
import { CourseQueryParams } from '../../../../shared/models/query-params.model';
import { forkJoin } from 'rxjs';
import { EnrolledCourseWithProgress, Transaction, StudentStatistics } from './models';
import { StudentBannerComponent } from './student-banner/student-banner.component';
import { StudentStatisticsComponent } from './student-statistics/student-statistics.component';
import { ContinueLearningComponent } from './continue-learning/continue-learning.component';
import { RecentTransactionsComponent } from './recent-transactions/recent-transactions.component';

@Component({
  selector: 'app-student-overview',
  standalone: true,
  imports: [
    CommonModule,
    StudentBannerComponent,
    StudentStatisticsComponent,
    ContinueLearningComponent,
    RecentTransactionsComponent
  ],
  templateUrl: './student-overview.html',
  styleUrl: './student-overview.scss'
})
export class StudentOverviewComponent implements OnInit {
  private courseService = inject(CourseService);
  private progressService = inject(ProgressService);

  isLoading = true;
  statistics: StudentStatistics = {
    totalEnrolled: 0,
    totalPaid: 0,
    completedCourses: 0,
    inProgressCourses: 0
  };
  recentCourses: EnrolledCourseWithProgress[] = [];
  transactions: Transaction[] = [];

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
        const enrolledCourses = this.mergeCoursesWithProgress(courses.items || [], progress);
        
        // Calculate statistics
        this.statistics = {
          totalEnrolled: enrolledCourses.length,
          totalPaid: 0, // Mock - payment system not implemented
          completedCourses: enrolledCourses.filter(c => (c.progressPercentage ?? 0) === 100).length,
          inProgressCourses: enrolledCourses.filter(c => (c.progressPercentage ?? 0) > 0 && (c.progressPercentage ?? 0) < 100).length
        };

        // Get last 3 active courses (sorted by progress percentage descending for "active")
        this.recentCourses = enrolledCourses
          .filter(c => (c.progressPercentage ?? 0) > 0)
          .sort((a, b) => (b.progressPercentage || 0) - (a.progressPercentage || 0))
          .slice(0, 3);

        // Mock transaction data
        this.transactions = this.getMockTransactions(enrolledCourses);

        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading student overview data:', err);
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
        progressPercentage: progress ? progress.percentComplete : 0,
        lastAccessed: new Date(course.createdAt) // Use course creation date as fallback
      };
    });
  }

  private getMockTransactions(courses: EnrolledCourseWithProgress[]): Transaction[] {
    // Generate mock transactions based on enrolled courses
    return courses.slice(0, 5).map((course, index) => ({
      id: `TXN-${Date.now()}-${index}`,
      date: new Date(Date.now() - index * 86400000 * (Math.random() * 30 + 1)).toISOString(),
      courseName: course.title,
      instructor: course.instructorName,
      amount: Math.floor(Math.random() * 100 + 50),
      paymentMethod: 'Credit Card',
      status: index === 0 ? 'pending' : 'completed'
    }));
  }
}
