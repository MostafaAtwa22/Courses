import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from '../../shared/components/header/header';
import { FooterComponent } from '../../shared/components/footer/footer';
import { CourseCardComponent } from './components/course-card/course-card';
import { CourseService } from './services/course.service';
import { CategoryService } from '../categories/services/category.service';
import { createCourseQueryParams, createQueryParams, CourseQueryParams } from '../../shared/models/query-params.model';
import { PaginatedResultModel } from '../../shared/models/paginated-result.model';
import { CourseResponse } from './models/course.models';

@Component({
  selector: 'app-courses-list',
  standalone: true,
  imports: [
    CommonModule,
    HeaderComponent,
    FooterComponent,
    CourseCardComponent
  ],
  templateUrl: './courses-list.html',
  styleUrl: './courses-list.scss'
})
export class CoursesListComponent implements OnInit {
  private courseService = inject(CourseService);
  private categoryService = inject(CategoryService);

  isDarkMode = false;
  isFilterOpen = false;

  categories: string[] = ['All'];
  selectedCategory = 'All';

  coursesResult: PaginatedResultModel<CourseResponse> = new PaginatedResultModel<CourseResponse>();
  params: CourseQueryParams = createCourseQueryParams({ pageSize: 9 });

  ngOnInit() {
    this.initTheme();
    this.loadCategories();
    this.loadCourses();
  }

  initTheme() {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark') {
      this.isDarkMode = true;
      document.body.classList.add('dark');
    }
  }

  loadCategories() {
    this.categoryService.getAll(createQueryParams({ pageSize: 100 })).subscribe({
      next: (res) => {
        this.categories = ['All', ...res.items.map(c => c.name)];
      }
    });
  }

  loadCourses() {
    this.courseService.getAll(this.params).subscribe({
      next: (res) => {
        this.coursesResult = PaginatedResultModel.fromApi<CourseResponse>(res);
      }
    });
  }

  toggleTheme() {
    this.isDarkMode = !this.isDarkMode;
    if (this.isDarkMode) {
      document.body.classList.add('dark');
      localStorage.setItem('theme', 'dark');
    } else {
      document.body.classList.remove('dark');
      localStorage.setItem('theme', 'light');
    }
  }

  setCategory(cat: string) {
    this.selectedCategory = cat;
    this.params.category = cat === 'All' ? '' : cat;
    this.params.pageNumber = 1;
    this.loadCourses();
  }

  setRatingFilter(min?: number, max?: number) {
    this.params.minRating = min;
    this.params.maxRating = max;
    this.params.pageNumber = 1;
    this.loadCourses();
  }

  onSearch(term: string) {
    this.params.searchTerm = term;
    this.params.pageNumber = 1;
    this.loadCourses();
  }

  onSortChange(event: Event) {
    const sortBy = (event.target as HTMLSelectElement).value;
    switch (sortBy) {
      case 'Newest':
        this.params.sortBy = 'created_at';
        this.params.sortDescending = true;
        break;
      case 'Price: Low to High':
        this.params.sortBy = 'cost';
        this.params.sortDescending = false;
        break;
      case 'Price: High to Low':
        this.params.sortBy = 'cost';
        this.params.sortDescending = true;
        break;
      default:
        this.params.sortBy = 'created_at';
        this.params.sortDescending = true;
    }
    this.params.pageNumber = 1;
    this.loadCourses();
  }

  onPageChange(page: number) {
    if (page < 1 || page > this.coursesResult.totalPages) return;
    this.params.pageNumber = page;
    this.loadCourses();
  }

  getPagesArray(): number[] {
    return Array.from({ length: this.coursesResult.totalPages }, (_, i) => i + 1);
  }

  resetFilters() {
    this.selectedCategory = 'All';
    this.params = createCourseQueryParams({ pageSize: 6 });
    this.loadCourses();
  }
}
