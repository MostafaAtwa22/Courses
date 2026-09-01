import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { debounceTime, distinctUntilChanged, Subject, takeUntil } from 'rxjs';
import { HeaderComponent } from '../../shared/components/header/header';
import { FooterComponent } from '../../shared/components/footer/footer';
import { CourseCardComponent } from './components/course-card/course-card';
import { CourseService } from './services/course.service';
import { CategoryService } from '../categories/services/category.service';
import { createCourseQueryParams, createQueryParams, CourseQueryParams } from '../../shared/models/query-params.model';
import { PaginatedResultModel } from '../../shared/models/paginated-result.model';
import { CourseSummary } from './models/course.models';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-courses-list',
  standalone: true,
  imports: [
    CommonModule,
    HeaderComponent,
    FooterComponent,
    CourseCardComponent,
    PaginationComponent
  ],
  templateUrl: './courses-list.html',
  styleUrl: './courses-list.scss'
})
export class CoursesListComponent implements OnInit, OnDestroy {
  private courseService = inject(CourseService);
  private categoryService = inject(CategoryService);
  private destroy$ = new Subject<void>();
  private searchSubject = new Subject<string>();

  isDarkMode = false; // kept for template bindings if any
  isFilterOpen = false;

  categories: string[] = ['All'];
  selectedCategory = 'All';

  coursesResult: PaginatedResultModel<CourseSummary> = new PaginatedResultModel<CourseSummary>();
  params: CourseQueryParams = createCourseQueryParams({ pageSize: 9 });

  suggestions: string[] = [];
  showSuggestions = false;

  ngOnInit() {
    this.loadCategories();
    this.loadCourses();

    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(term => {
      if (term.length > 1) {
        this.courseService.getSuggestions(term).subscribe(res => {
          this.suggestions = res;
          this.showSuggestions = true;
        });
      } else {
        this.suggestions = [];
        this.showSuggestions = false;
      }
    });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
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
        this.coursesResult = res;
      }
    });
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
    this.showSuggestions = false;
    this.loadCourses();
  }

  onSearchInput(event: Event) {
    const term = (event.target as HTMLInputElement).value;
    this.searchSubject.next(term);
  }

  selectSuggestion(suggestion: string) {
    this.params.searchTerm = suggestion;
    this.params.pageNumber = 1;
    this.showSuggestions = false;
    this.loadCourses();
  }

  clearSearch() {
    this.params.searchTerm = '';
    this.params.pageNumber = 1;
    this.showSuggestions = false;
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
    this.params.pageNumber = page;
    this.loadCourses();
  }

  resetFilters() {
    this.selectedCategory = 'All';
    this.params = createCourseQueryParams({ pageSize: 9 });
    this.loadCourses();
  }
}
