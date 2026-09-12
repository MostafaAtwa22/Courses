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

@Component({
  selector: 'app-courses-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './courses-list.component.html',
  styleUrl: './courses-list.component.scss'
})
export class CoursesListComponent implements OnInit {
  private courseService = inject(CourseService);
  private categoryService = inject(CategoryService);
  private router = inject(Router);

  coursesResult: PaginatedResultModel<CourseSummary> = new PaginatedResultModel<CourseSummary>();
  categories: CategoryResponse[] = [];
  params: CourseQueryParams = { pageNumber: 1, pageSize: 10 };
  searchQuery = '';
  categoryFilter = '';
  sortBy = 'averageRate';
  sortDescending = true;
  isFilterDropdownOpen = false;

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
    
    this.courseService.getAll(params).subscribe({
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

  getStars(rating: number): { full: number, half: boolean, empty: number } {
    const full = Math.floor(rating);
    const half = rating % 1 >= 0.5;
    const empty = 5 - full - (half ? 1 : 0);
    return { full, half, empty };
  }
}
