import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CourseService } from '../../../courses/services/course.service';
import { ProgressService } from '../../../courses/services/progress.service';
import { CourseSummary, CourseProgressSummary } from '../../../courses/models/course.models';
import { CourseQueryParams } from '../../../../shared/models/query-params.model';
import { forkJoin } from 'rxjs';

export interface EnrolledCourseWithProgress extends CourseSummary {
  progress?: CourseProgressSummary;
  progressPercentage?: number;
  lastAccessed?: Date;
}

@Component({
  selector: 'app-student-courses',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './student-courses.html',
  styleUrl: './student-courses.scss'
})
export class StudentCoursesComponent implements OnInit {
  private courseService = inject(CourseService);
  private progressService = inject(ProgressService);

  isLoading = true;
  allCourses: EnrolledCourseWithProgress[] = [];
  enrolledCourses: EnrolledCourseWithProgress[] = [];
  filterStatus: 'all' | 'in-progress' | 'completed' | 'not-started' = 'all';
  sortBy: 'recent' | 'progress' | 'title' = 'recent';

  ngOnInit() {
    this.loadStudentCourses();
  }

  loadStudentCourses() {
    const params: CourseQueryParams = {
      pageNumber: 1,
      pageSize: 100,
      sortBy: 'createdAt',
      sortDescending: true
    };

    forkJoin({
      courses: this.courseService.getCoursesByStudentId(params),
      progress: this.progressService.getMyCoursesProgress()
    }).subscribe({
      next: ({ courses, progress }) => {
        this.allCourses = this.mergeCoursesWithProgress(courses.items || [], progress);
        this.applyFilters();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading student courses:', err);
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

  applyFilters() {
    let filtered = [...this.allCourses];

    // Apply status filter
    if (this.filterStatus !== 'all') {
      filtered = filtered.filter(course => {
        const progress = course.progressPercentage || 0;
        switch (this.filterStatus) {
          case 'completed':
            return progress === 100;
          case 'in-progress':
            return progress > 0 && progress < 100;
          case 'not-started':
            return progress === 0;
          default:
            return true;
        }
      });
    }

    // Apply sorting
    switch (this.sortBy) {
      case 'recent':
        filtered.sort((a, b) => (b.lastAccessed?.getTime() || 0) - (a.lastAccessed?.getTime() || 0));
        break;
      case 'progress':
        filtered.sort((a, b) => (b.progressPercentage || 0) - (a.progressPercentage || 0));
        break;
      case 'title':
        filtered.sort((a, b) => a.title.localeCompare(b.title));
        break;
    }

    this.enrolledCourses = filtered;
  }

  onFilterChange(status: 'all' | 'in-progress' | 'completed' | 'not-started') {
    this.filterStatus = status;
    this.applyFilters();
  }

  onSortChange(sortBy: 'recent' | 'progress' | 'title') {
    this.sortBy = sortBy;
    this.applyFilters();
  }

  getProgressColor(percentage: number): string {
    if (percentage >= 75) return 'bg-success';
    if (percentage >= 50) return 'bg-primary';
    if (percentage >= 25) return 'bg-warning';
    return 'bg-danger';
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

  getFilteredCount(): number {
    return this.enrolledCourses.length;
  }

  getCountByStatus(status: 'in-progress' | 'completed' | 'not-started'): number {
    return this.allCourses.filter(course => {
      const progress = course.progressPercentage || 0;
      switch (status) {
        case 'completed':
          return progress === 100;
        case 'in-progress':
          return progress > 0 && progress < 100;
        case 'not-started':
          return progress === 0;
        default:
          return true;
      }
    }).length;
  }
}
