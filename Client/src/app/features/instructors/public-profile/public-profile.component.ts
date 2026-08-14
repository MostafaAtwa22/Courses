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

@Component({
  selector: 'app-instructor-public-profile',
  standalone: true,
  imports: [CommonModule, HeaderComponent, FooterComponent, CourseCardComponent],
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
  isDarkMode = this.themeService.isDarkModeSignal();
  
  currentPage = 1;
  pageSize = 3;
  totalCourses = 0;
  totalPages = 0;

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
        this.isLoadingCourses = false;
      },
      error: (err) => {
        console.error('Failed to load courses:', err);
        this.isLoadingCourses = false;
      }
    });
  }

  onPageChange(page: number) {
    if (page < 1 || page > this.totalPages || page === this.currentPage) return;
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

  get coursesRange(): string {
    if (this.totalCourses === 0) return '0-0';
    const start = (this.currentPage - 1) * this.pageSize + 1;
    const end = Math.min(this.currentPage * this.pageSize, this.totalCourses);
    return `${start}-${end}`;
  }

  getPageNumbers(): number[] {
    const pages: number[] = [];
    const maxVisiblePages = 5;
    
    if (this.totalPages <= maxVisiblePages) {
      for (let i = 1; i <= this.totalPages; i++) {
        pages.push(i);
      }
    } else {
      if (this.currentPage <= 3) {
        for (let i = 1; i <= 4; i++) {
          pages.push(i);
        }
        pages.push(-1);
        pages.push(this.totalPages);
      } else if (this.currentPage >= this.totalPages - 2) {
        pages.push(1);
        pages.push(-1);
        for (let i = this.totalPages - 3; i <= this.totalPages; i++) {
          pages.push(i);
        }
      } else {
        pages.push(1);
        pages.push(-1);
        pages.push(this.currentPage - 1);
        pages.push(this.currentPage);
        pages.push(this.currentPage + 1);
        pages.push(-1);
        pages.push(this.totalPages);
      }
    }
    
    return pages;
  }

  toggleTheme() {
    this.themeService.toggleTheme();
  }
}
