import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CourseService } from '../../courses/services/course.service';
import { CourseSummary } from '../../courses/models/course.models';
import { PaginatedResultModel } from '../../../shared/models/paginated-result.model';
import { CourseQueryParams, createCourseQueryParams } from '../../../shared/models/query-params.model';
import { RouterLink } from '@angular/router';
import { SessionService } from '../../auth/services/session.service';
import { CourseCardComponent } from '../../courses/components/course-card/course-card';

@Component({
  selector: 'app-user-courses',
  standalone: true,
  imports: [CommonModule, RouterLink, CourseCardComponent],
  templateUrl: './user-courses.component.html',
  styleUrl: './user-courses.component.scss'
})
export class UserCoursesComponent implements OnInit {
  private courseService = inject(CourseService);
  private sessionService = inject(SessionService);

  coursesResult: PaginatedResultModel<CourseSummary> = new PaginatedResultModel<CourseSummary>();
  params: CourseQueryParams = createCourseQueryParams({ pageSize: 6, pageNumber: 1 });
  isLoading = false;
  errorMessage = '';

  ngOnInit() {
    this.loadCourses();
  }

  loadCourses() {
    this.isLoading = true;
    this.errorMessage = '';

    const currentUser = this.sessionService.currentUser();
    if (!currentUser) {
      this.errorMessage = 'User not authenticated';
      this.isLoading = false;
      return;
    }

    if (currentUser.roles?.includes('Instructor')) {
      this.courseService.getCoursesByInstructorId(this.params).subscribe({
        next: (res) => {
          this.coursesResult = res;
          this.isLoading = false;
        },
        error: (err) => {
          this.errorMessage = 'Failed to load instructor courses';
          this.isLoading = false;
          console.error('Error loading instructor courses:', err);
        }
      });
    } else {
      this.courseService.getCoursesByStudentId(this.params).subscribe({
        next: (res) => {
          this.coursesResult = res;
          this.isLoading = false;
        },
        error: (err) => {
          this.errorMessage = 'Failed to load student courses';
          this.isLoading = false;
          console.error('Error loading student courses:', err);
        }
      });
    }
  }

  onPageChange(page: number) {
    if (page < 1 || page > this.coursesResult.totalPages) return;
    this.params.pageNumber = page;
    this.loadCourses();
  }

  get currentPage(): number {
    return this.params.pageNumber || 1;
  }

  getPagesArray(): number[] {
    return Array.from({ length: this.coursesResult.totalPages }, (_, i) => i + 1);
  }

  get userRole(): string {
    const currentUser = this.sessionService.currentUser();
    return currentUser?.roles?.[0] || 'Student';
  }

  get isInstructor(): boolean {
    const currentUser = this.sessionService.currentUser();
    return currentUser?.roles?.includes('Instructor') || false;
  }

  get isStudent(): boolean {
    const currentUser = this.sessionService.currentUser();
    return currentUser?.roles?.includes('Student') || false;
  }

  get coursesRange(): string {
    if (!this.coursesResult.totalCount || this.coursesResult.totalCount === 0) return '0-0';
    const pageSize = this.params.pageSize ?? 6;
    const start = (this.currentPage - 1) * pageSize + 1;
    const end = Math.min(this.currentPage * pageSize, this.coursesResult.totalCount);
    return `${start}-${end}`;
  }
}
