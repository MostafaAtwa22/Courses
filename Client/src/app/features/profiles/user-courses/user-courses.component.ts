import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CourseService } from '../../courses/services/course.service';
import { CourseSummary } from '../../courses/models/course.models';
import { PaginatedResultModel } from '../../../shared/models/paginated-result.model';
import { CourseQueryParams, createCourseQueryParams } from '../../../shared/models/query-params.model';
import { RouterLink } from '@angular/router';
import { SessionService } from '../../auth/services/session.service';
import { CourseCardComponent } from '../../courses/components/course-card/course-card';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-user-courses',
  standalone: true,
  imports: [CommonModule, RouterLink, CourseCardComponent, PaginationComponent],
  templateUrl: './user-courses.component.html',
  styleUrl: './user-courses.component.scss'
})
export class UserCoursesComponent implements OnInit {
  private courseService = inject(CourseService);
  private sessionService = inject(SessionService);

  coursesResult: PaginatedResultModel<CourseSummary> = new PaginatedResultModel<CourseSummary>();
  params: CourseQueryParams = createCourseQueryParams({ pageSize: 3, pageNumber: 1 });
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
    this.params.pageNumber = page;
    this.loadCourses();
  }

  get currentPage(): number {
    return this.params.pageNumber || 1;
  }

  get isInstructor(): boolean {
    const currentUser = this.sessionService.currentUser();
    return currentUser?.roles?.includes('Instructor') || false;
  }

  get isStudent(): boolean {
    const currentUser = this.sessionService.currentUser();
    return currentUser?.roles?.includes('Student') || false;
  }
}
