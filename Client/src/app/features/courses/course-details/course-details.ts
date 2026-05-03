import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CourseService } from '../services/course.service';
import { SectionService } from '../services/section.service';
import { ContentService } from '../services/content.service';
import { ReviewService } from '../services/review.service';
import { CourseResponse, SectionResponse, ReviewResponse, ContentResponse } from '../models/course.models';
import { CourseHeroComponent } from './components/course-hero/course-hero';
import { CourseSidebarComponent } from './components/course-sidebar/course-sidebar';
import { CourseContentComponent } from './components/course-content/course-content';
import { CourseInstructorComponent } from './components/course-instructor/course-instructor';
import { CourseReviewsComponent } from './components/course-reviews/course-reviews';
import { HeaderComponent } from '../../../shared/components/header/header';
import { FooterComponent } from '../../../shared/components/footer/footer';

@Component({
  selector: 'app-course-details',
  standalone: true,
  imports: [
    CommonModule,
    CourseHeroComponent,
    CourseSidebarComponent,
    CourseContentComponent,
    CourseInstructorComponent,
    CourseReviewsComponent,
    HeaderComponent,
    FooterComponent,
    RouterModule
  ],
  templateUrl: './course-details.html',
  styleUrl: './course-details.scss'
})
export class CourseDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private courseService = inject(CourseService);
  private sectionService = inject(SectionService);
  private contentService = inject(ContentService);
  private reviewService = inject(ReviewService);

  course?: CourseResponse;
  sections: SectionResponse[] = [];
  reviews: ReviewResponse[] = [];
  sectionsLoading = false;
  hasMoreSections = false;
  totalSections = 0;
  currentPage = 1;
  pageSize = 5;
  isDarkMode = false;

  // Track which sections have had their contents loaded
  private loadedSectionIds = new Set<string>();
  // Track which sections are currently loading contents
  loadingContentSectionIds = new Set<string>();

  learningPoints: string[] = [
    'Build enterprise-level applications with modern technologies',
    'Understand and implement advanced design patterns',
    'Optimize performance and accessibility',
    'Master complex state management strategies',
    'Deploy and scale applications to the cloud',
    'Implement robust security and authentication systems'
  ];

  requirements: string[] = [
    'Solid understanding of JavaScript/TypeScript',
    'Basic knowledge of web frameworks',
    'Experience with HTML and CSS',
    'A computer with administrative privileges'
  ];

  defaultDescription = `
    <p>This comprehensive bootcamp is designed to take you from a basic understanding of web development to being a proficient full-stack engineer. We focus on real-world applications and industry-standard practices.</p>
    <p>Throughout the course, you will build multiple projects that demonstrate your ability to handle both frontend and backend challenges, including state management, API design, and database optimization.</p>
  `;

  ngOnInit(): void {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark') {
      this.isDarkMode = true;
      document.body.classList.add('dark');
    }

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.courseService.getById(id).subscribe({
        next: (course) => {
          this.course = course;
          this.loadSections(course.id);
          this.loadReviews(course.id);
        },
        error: (err) => {
          console.error('Error fetching course:', err);
        }
      });
    }
  }

  loadSections(courseId: string): void {
    this.sectionsLoading = true;
    this.sectionService.getByCourseId(courseId, {
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      sortBy: 'order'
    }).subscribe({
      next: (result) => {
        const newSections = result.items || [];
        this.totalSections = result.totalCount;

        // Initialize contents as empty array — will be loaded on click
        newSections.forEach(section => section.contents = []);
        this.sections = [...this.sections, ...newSections];
        this.hasMoreSections = this.sections.length < this.totalSections;
        this.sectionsLoading = false;
      },
      error: (err) => {
        console.error('Error loading sections:', err);
        this.sectionsLoading = false;
      }
    });
  }

  loadSectionContents(sectionId: string): void {
    // Don't reload if already loaded or currently loading
    if (this.loadedSectionIds.has(sectionId) || this.loadingContentSectionIds.has(sectionId)) {
      return;
    }

    // Update loading state with new Set reference for change detection
    this.loadingContentSectionIds = new Set(this.loadingContentSectionIds).add(sectionId);

    this.contentService.getBySection(sectionId).subscribe({
      next: (contents) => {
        const section = this.sections.find(s => s.id === sectionId);
        if (section) {
          section.contents = Array.isArray(contents) ? contents : [];
          // Trigger change detection for child components by updating reference
          this.sections = [...this.sections];
        }
        this.loadedSectionIds.add(sectionId);
        
        // Remove from loading state with new reference
        const nextLoading = new Set(this.loadingContentSectionIds);
        nextLoading.delete(sectionId);
        this.loadingContentSectionIds = nextLoading;
      },
      error: (err) => {
        console.error(`Error loading contents for section ${sectionId}:`, err);
        const nextLoading = new Set(this.loadingContentSectionIds);
        nextLoading.delete(sectionId);
        this.loadingContentSectionIds = nextLoading;
      }
    });
  }

  isSectionContentLoading(sectionId: string): boolean {
    return this.loadingContentSectionIds.has(sectionId);
  }

  loadReviews(courseId: string): void {
    this.reviewService.getByCourseId(courseId, {
      pageNumber: 1,
      pageSize: 10,
      sortBy: 'createdAt',
      sortDescending: true
    }).subscribe({
      next: (result) => {
        this.reviews = result.items || [];
      },
      error: (err) => {
        console.error('Error loading reviews:', err);
      }
    });
  }

  loadMoreSections(): void {
    if (!this.course || this.sectionsLoading || !this.hasMoreSections) return;
    this.currentPage++;
    this.loadSections(this.course.id);
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

  handleContentSelected(content: ContentResponse): void {
    if (!this.course) return;
    this.router.navigate(['content', content.id], { relativeTo: this.route });
  }
}
