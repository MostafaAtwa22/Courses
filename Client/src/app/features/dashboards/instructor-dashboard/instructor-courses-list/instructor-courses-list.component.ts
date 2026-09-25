import { Component, inject, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CourseService } from '../../../courses/services/course.service';
import { CourseSummary } from '../../../courses/models/course.models';
import { PaginatedResultModel } from '../../../../shared/models/paginated-result.model';
import { CourseQueryParams } from '../../../../shared/models/query-params.model';
import { CategoryService } from '../../../categories/services/category.service';
import { CategoryResponse } from '../../../categories/models/category.models';
import { CourseModalComponent } from './course-modal.component';
import { ToastService } from '../../../../core/services/toast.service';

@Component({
  selector: 'app-instructor-courses-list',
  standalone: true,
  imports: [CommonModule, FormsModule, CourseModalComponent],
  templateUrl: './instructor-courses-list.component.html',
  styleUrl: './instructor-courses-list.component.scss'
})
export class InstructorCoursesListComponent implements OnInit {
  private courseService = inject(CourseService);
  private categoryService = inject(CategoryService);
  private router = inject(Router);
  private toastService = inject(ToastService);

  coursesResult: PaginatedResultModel<CourseSummary> = new PaginatedResultModel<CourseSummary>();
  categories: CategoryResponse[] = [];
  params: CourseQueryParams = { pageNumber: 1, pageSize: 10 };
  searchQuery = '';
  categoryFilter = '';
  sortBy = 'averageRate';
  sortDescending = true;
  isFilterDropdownOpen = false;
  isModalOpen = false;
  selectedCourse: CourseSummary | null = null;
  isSubmitting = false;

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    const target = event.target as HTMLElement;
    const dropdown = target.closest('.dropdown');
    
    if (!dropdown && this.isFilterDropdownOpen) {
      this.isFilterDropdownOpen = false;
    }
  }

  ngOnInit() {
    this.loadCategories();
    this.loadCourses();
  }

  toggleFilterDropdown() {
    this.isFilterDropdownOpen = !this.isFilterDropdownOpen;
  }

  loadCategories() {
    this.categoryService.getAll({ pageNumber: 1, pageSize: 100 }).subscribe({
      next: (res) => {
        this.categories = res.items;
      }
    });
  }

  loadCourses() {
    const params: CourseQueryParams = {
      pageNumber: this.params.pageNumber,
      pageSize: this.params.pageSize,
      sortDescending: this.sortDescending
    };

    if (this.searchQuery) {
      params.searchTerm = this.searchQuery;
    }
    
    if (this.categoryFilter) {
      params.category = this.categoryFilter;
    }
    
    if (this.sortBy) {
      params.sortBy = this.sortBy;
    }
    
    this.courseService.getCoursesByInstructorId(params).subscribe({
      next: (res: PaginatedResultModel<CourseSummary>) => {
        this.coursesResult = res;
      }
    });
  }

  onSearch() {
    this.params.pageNumber = 1;
    this.loadCourses();
  }

  clearFilters() {
    this.searchQuery = '';
    this.categoryFilter = '';
    this.sortBy = 'averageRate';
    this.sortDescending = true;
    this.onSearch();
    this.isFilterDropdownOpen = false;
  }

  onPageChange(page: number) {
    if (page < 1 || page > this.coursesResult.totalPages) return;
    this.params.pageNumber = page;
    this.loadCourses();
  }

  getPagesArray(): number[] {
    return Array.from({ length: this.coursesResult.totalPages }, (_, i) => i + 1);
  }

  viewCourseDetails(courseId: string) {
    this.router.navigate(['/courses', courseId]);
  }

  openCreateModal() {
    this.selectedCourse = null;
    this.isModalOpen = true;
  }

  openEditModal(course: CourseSummary) {
    this.selectedCourse = course;
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
    this.selectedCourse = null;
  }

  onCourseSaved() {
    this.closeModal();
    this.loadCourses();
    this.toastService.success('Course saved successfully');
  }

  deleteCourse(courseId: string) {
    if (confirm('Are you sure you want to delete this course? This action cannot be undone.')) {
      this.isSubmitting = true;
      this.courseService.delete(courseId).subscribe({
        next: () => {
          this.toastService.success('Course deleted successfully');
          this.loadCourses();
          this.isSubmitting = false;
        },
        error: () => {
          this.toastService.error('Failed to delete course');
          this.isSubmitting = false;
        }
      });
    }
  }

  getStars(rating: number): { full: number, half: boolean, empty: number } {
    const full = Math.floor(rating);
    const half = rating % 1 >= 0.5;
    const empty = 5 - full - (half ? 1 : 0);
    return { full, half, empty };
  }
}
