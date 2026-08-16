import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CourseService } from '../services/course.service';
import { SectionService } from '../services/section.service';
import { ContentService } from '../services/content.service';
import { ReviewService } from '../services/review.service';
import { ProgressService } from '../services/progress.service';
import { CourseResponse, SectionResponse, ReviewResponse, ContentResponse, CourseProgress, ReviewCreateRequest } from '../models/course.models';
import { CourseHeroComponent } from './components/course-hero/course-hero';
import { CourseSidebarComponent } from './components/course-sidebar/course-sidebar';
import { CourseContentComponent } from './components/course-content/course-content';
import { CourseInstructorComponent } from './components/course-instructor/course-instructor';
import { CourseReviewsComponent } from './components/course-reviews/course-reviews';
import { HeaderComponent } from '../../../shared/components/header/header';
import { FooterComponent } from '../../../shared/components/footer/footer';
import { ThemeService } from '../../../core/services/theme.service';
import { AuthService } from '../../auth/services/auth.service';

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
  private progressService = inject(ProgressService);
  private themeService = inject(ThemeService);
  private authService = inject(AuthService);

  course?: CourseResponse;
  sections: SectionResponse[] = [];
  reviews: ReviewResponse[] = [];
  progress?: CourseProgress;
  isEnrolled = false;
  hasReviewed = false;
  sectionsLoading = false;
  hasMoreSections = false;
  totalSections = 0;
  currentPage = 1;
  pageSize = 5;

  // Track which sections have had their contents loaded
  private loadedSectionIds = new Set<string>();
  // Track which sections are currently loading contents
  loadingContentSectionIds = new Set<string>();

  learningPoints: string[] = [];
  requirements: string[] = [];

  defaultDescription = `
    <p>This comprehensive bootcamp is designed to take you from a basic understanding of web development to being a proficient full-stack engineer. We focus on real-world applications and industry-standard practices.</p>
    <p>Throughout the course, you will build multiple projects that demonstrate your ability to handle both frontend and backend challenges, including state management, API design, and database optimization.</p>
  `;

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.courseService.getById(id).subscribe({
        next: (course) => {
          this.course = course;
          this.learningPoints = course.whatYouWillLearn || [];
          this.requirements = course.requirements || [];
          this.loadSections(course.id);
          this.loadReviews(course.id);
          this.loadProgress(course.id); // will trigger checkUserReviewStatus after enrollment is confirmed
        },
        error: (err) => {
          console.error('Error fetching course:', err);
        }
      });
    }
  }

  loadProgress(courseId: string): void {
    // Only load progress if user is authenticated
    if (!this.authService.isLoggedIn()) {
      this.isEnrolled = false;
      this.hasReviewed = false;
      return;
    }

    this.progressService.checkEnrollment(courseId).subscribe({
      next: (progress) => {
        this.progress = progress;
        this.isEnrolled = true;
        // Now that we know the user is enrolled, check if they've reviewed
        this.checkUserReviewStatus(courseId);
      },
      error: (err) => {
        // User might not be enrolled - that's fine
        this.isEnrolled = false;
        this.hasReviewed = false;
        console.log('Could not load progress (user may not be enrolled)');
      }
    });
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

    this.contentService.getBySection(sectionId, this.course?.id || '').subscribe({
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

  checkUserReviewStatus(courseId: string): void {
    // Only check if user is authenticated
    if (!this.authService.isLoggedIn()) {
      this.hasReviewed = false;
      return;
    }

    this.reviewService.hasUserReviewed(courseId).subscribe({
      next: (hasReviewed) => {
        this.hasReviewed = hasReviewed;
      },
      error: (err) => {
        // If error, assume not reviewed
        this.hasReviewed = false;
        console.log('Could not check review status');
      }
    });
  }

  loadMoreSections(): void {
    if (!this.course || this.sectionsLoading || !this.hasMoreSections) return;
    this.currentPage++;
    this.loadSections(this.course.id);
  }

  handleContentSelected(content: ContentResponse): void {
    if (!this.course) return;
    this.router.navigate(['content', content.id], { relativeTo: this.route });
  }

  onReviewAdded(): void {
    if (!this.course) return;
    // Reload reviews to show the new/updated review
    this.loadReviews(this.course.id);
    // Re-check if user has reviewed (handles add, edit, delete)
    this.checkUserReviewStatus(this.course.id);
  }
}
