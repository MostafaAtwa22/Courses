import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { InstructorService } from '../services/instructor.service';
import { InstructorPublicResponse } from '../models/instructor.models';
import { CourseService } from '../../courses/services/course.service';
import { CourseSummary } from '../../courses/models/course.models';
import { CourseQueryParams } from '../../../shared/models/query-params.model';
import { HeaderComponent } from '../../../shared/components/header/header';
import { FooterComponent } from '../../../shared/components/footer/footer';
import { ThemeService } from '../../../core/services/theme.service';
import { CourseCardComponent } from '../../courses/components/course-card/course-card';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-instructor-public-profile',
  standalone: true,
  imports: [CommonModule, HeaderComponent, FooterComponent, CourseCardComponent, PaginationComponent],
  templateUrl: './public-profile.component.html',
  styleUrl: './public-profile.component.scss'
})
export class InstructorPublicProfileComponent implements OnInit {
  instructor?: InstructorPublicResponse;
  courses: CourseSummary[] = [];
  isLoading = true;
  isLoadingCourses = true;
  error: string | null = null;
  isBioExpanded = false;
  private themeService = inject(ThemeService);
  
  currentPage = 1;
  pageSize = 3;
  totalCourses = 0;
  totalPages = 0;
  hasPreviousPage = false;
  hasNextPage = false;

  constructor(
    private route: ActivatedRoute,
    public router: Router,
    private instructorService: InstructorService,
    private courseService: CourseService
  ) {}

  ngOnInit() {
    const instructorId = this.route.snapshot.paramMap.get('id');
    if (instructorId) {
      this.currentPage = 1;
      this.loadInstructor(instructorId);
    } else {
      this.error = 'Instructor ID not provided';
      this.isLoading = false;
    }
  }

  private loadInstructor(id: string) {
    this.instructorService.getPublicInstructorById(id).subscribe({
      next: (data) => {
        this.instructor = data;
        this.isLoading = false;
        this.loadCourses(data.id);
      },
      error: (err) => {
        console.error('Failed to load instructor:', err);
        this.error = 'Failed to load instructor profile';
        this.isLoading = false;
      }
    });
  }

  private loadCourses(instructorId: string) {
    const params: CourseQueryParams = {
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      sortBy: 'createdOn',
      sortDescending: true
    };

    this.courseService.getCoursesByInstructorIdPublic(instructorId, params).subscribe({
      next: (result) => {
        this.courses = result.items || [];
        this.totalCourses = result.totalCount || 0;
        this.totalPages = Math.ceil(this.totalCourses / this.pageSize);
        this.hasPreviousPage = result.hasPreviousPage || false;
        this.hasNextPage = result.hasNextPage || false;
        this.isLoadingCourses = false;
      },
      error: (err) => {
        console.error('Failed to load courses:', err);
        this.isLoadingCourses = false;
      }
    });
  }

  onPageChange(page: number) {
    this.currentPage = page;
    this.isLoadingCourses = true;
    if (this.instructor) {
      this.loadCourses(this.instructor.id);
    }
  }

  get instructorName(): string {
    return this.instructor ? `${this.instructor.firstName} ${this.instructor.lastName}` : 'Instructor';
  }

  get instructorProfilePicture(): string {
    if (this.instructor?.profilePicture) {
      return this.instructor.profilePicture;
    }
    return `https://ui-avatars.com/api/?name=${encodeURIComponent(this.instructorName)}&background=random&size=200`;
  }

  get formattedStats() {
    if (!this.instructor) return null;
    return {
      rating: this.instructor.averageRate.toFixed(1),
      reviews: this.instructor.totalReviews.toLocaleString(),
      students: this.instructor.totalStudents.toLocaleString(),
      courses: this.instructor.totalCourses.toLocaleString()
    };
  }

  toggleTheme() {
    this.themeService.toggleTheme();
  }
}
