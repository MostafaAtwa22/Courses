import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ContentService } from '../../../services/content.service';
import { CourseService } from '../../../services/course.service';
import { SectionService } from '../../../services/section.service';
import { ProgressService } from '../../../services/progress.service';
import { ContentResponse, ContentType, CourseProgress, CourseResponse, SectionResponse } from '../../../models/course.models';
import { forkJoin } from 'rxjs';
import { VideoPlayerComponent } from '../../../../../shared/components/video-player/video-player.component';
import { ProgressBarComponent } from '../../../../../shared/components/progress-bar/progress-bar';
import { environment } from '../../../../../../environments/environment';
import { AuthService } from '../../../../auth/services/auth.service';

@Component({
  selector: 'app-content-player',
  standalone: true,
  imports: [CommonModule, RouterModule, VideoPlayerComponent, ProgressBarComponent],
  templateUrl: './content-player.html',
  styleUrl: './content-player.scss'
})
export class ContentPlayerComponent implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private contentService = inject(ContentService);
  private courseService = inject(CourseService);
  private sectionService = inject(SectionService);
  private progressService = inject(ProgressService);
  private authService = inject(AuthService);

  course?: CourseResponse;
  content?: ContentResponse;
  sections: SectionResponse[] = [];
  progress?: CourseProgress;
  isEnrolled = false;
  
  loading = true;
  error: string | null = null;
  ContentType = ContentType;

  expandedSections = new Set<string>();
  
  allContents: ContentResponse[] = [];
  nextContent?: ContentResponse;
  previousContent?: ContentResponse;
  isSidebarOpen = false;

  getFullUrl(relativeUrl?: string): string {
    if (!relativeUrl) return '';
    if (relativeUrl.startsWith('http://') || relativeUrl.startsWith('https://')) {
      return relativeUrl;
    }
    return `${environment.apiUrl}/${relativeUrl.replace(/^\//, '')}`;
  }

  ngOnInit(): void {
    document.body.style.overflow = 'hidden';

    // Get courseId and contentId from parent and current route
    const courseId = this.route.snapshot.parent?.paramMap.get('id');
    
    this.route.params.subscribe(params => {
      const contentId = params['contentId'];
      if (contentId) {
        this.loadCurrentContent(contentId);
      }
      
      if (courseId && this.sections.length === 0) {
        this.loadCurriculum(courseId);
      } else if (this.allContents.length > 0) {
        this.updateNavigation(contentId);
      }
    });
  }

  ngOnDestroy(): void {
    document.body.style.overflow = 'auto';
  }

  isVideoContent(content: ContentResponse | undefined): boolean {
    if (!content) return false;
    // Check both numeric type (0) and string type ("Video")
    const typeValue = content.type as any;
    return typeValue === ContentType.Video || typeValue === 'Video' || typeValue === 'video';
  }

  loadCurrentContent(id: string): void {
    this.loading = true;
    const courseId = this.route.snapshot.parent?.paramMap.get('id') || '';
    this.contentService.getById(id, courseId).subscribe({
      next: (content) => {
        this.content = content;
        this.loading = false;
        this.expandedSections.add(content.sectionId);
        this.updateNavigation(id);
      },
      error: (err) => {
        console.error('Error loading content:', err);
        this.error = 'Failed to load content.';
        this.loading = false;
      }
    });
  }

  loadCurriculum(courseId: string): void {
    // 1. Get Course details
    this.courseService.getById(courseId).subscribe(c => this.course = c);

    // 2. Load progress (only if authenticated)
    if (this.authService.isLoggedIn()) {
      this.progressService.checkEnrollment(courseId).subscribe({
        next: (progress) => {
          this.progress = progress;
          this.isEnrolled = true;
        },
        error: (err) => {
          // User might not be enrolled - that's fine
          this.isEnrolled = false;
          console.log('Could not load progress (user may not be enrolled)');
        }
      });
    } else {
      this.isEnrolled = false;
    }

    // 3. Get Sections
    this.sectionService.getByCourseId(courseId, { pageSize: 100 }).subscribe({
      next: (result) => {
        this.sections = result.items || [];

        // 4. For each section, load its contents
        const contentObservables = this.sections.map(s => this.contentService.getBySection(s.id, courseId));

        forkJoin(contentObservables).subscribe({
          next: (allSectionContents) => {
            this.allContents = [];
            allSectionContents.forEach((contents, index) => {
              this.sections[index].contents = contents;
              this.allContents.push(...contents);
            });

            if (this.content) {
              this.updateNavigation(this.content.id);
            }
          }
        });
      }
    });
  }

  updateNavigation(currentId: string): void {
    const currentIndex = this.allContents.findIndex(c => c.id === currentId);
    this.nextContent = currentIndex < this.allContents.length - 1 ? this.allContents[currentIndex + 1] : undefined;
    this.previousContent = currentIndex > 0 ? this.allContents[currentIndex - 1] : undefined;
  }

  toggleSection(sectionId: string): void {
    if (this.expandedSections.has(sectionId)) {
      this.expandedSections.delete(sectionId);
    } else {
      this.expandedSections.add(sectionId);
    }
  }

  isSectionExpanded(sectionId: string): boolean {
    return this.expandedSections.has(sectionId);
  }

  selectContent(content: ContentResponse): void {
    if (this.isContentRestricted(content)) {
      return; // Don't navigate to restricted content
    }
    this.router.navigate(['../', content.id], { relativeTo: this.route });
  }

  isContentRestricted(content: ContentResponse): boolean {
    return !content.isPreview && !content.contentUrl;
  }

  goToNext(): void {
    if (this.nextContent) {
      this.selectContent(this.nextContent);
    }
  }

  goToPrevious(): void {
    if (this.previousContent) {
      this.selectContent(this.previousContent);
    }
  }

  close(): void {
    this.router.navigate(['../../'], { relativeTo: this.route });
  }

  toggleSidebar(): void {
    this.isSidebarOpen = !this.isSidebarOpen;
  }

  openFile(): void {
    if (this.content?.contentUrl) {
      window.open(this.content.contentUrl, '_blank');
    }
  }

  getContentIcon(type: number): string {
    switch (type) {
      case 0: return 'fa-play-circle'; // Video
      case 1: return 'fa-file-alt'; // Document
      case 2: return 'fa-question-circle'; // Quiz
      default: return 'fa-play-circle';
    }
  }

  isContentCompleted(contentId: string): boolean {
    return this.progress?.completedContentIds?.includes(contentId) ?? false;
  }

  toggleContentComplete(event: Event, content: ContentResponse): void {
    event.stopPropagation(); // Prevent selecting the content
    
    if (!this.course?.id || !this.authService.isLoggedIn()) return;

    const isCompleted = this.isContentCompleted(content.id);
    const request = { contentId: content.id, courseId: this.course.id };

    if (isCompleted) {
      this.progressService.markIncomplete(request).subscribe({
        next: () => {
          // Update local state
          if (this.progress) {
            this.progress.completedContentIds = this.progress.completedContentIds.filter(id => id !== content.id);
            this.progress.completedCount = Math.max(0, this.progress.completedCount - 1);
            this.progress.percentComplete = this.progress.totalCount > 0 
              ? Math.round((this.progress.completedCount / this.progress.totalCount) * 100)
              : 0;
          }
        },
        error: (err) => {
          console.error('Failed to mark content as incomplete:', err);
        }
      });
    } else {
      this.progressService.markComplete(request).subscribe({
        next: () => {
          // Update local state
          if (this.progress) {
            if (!this.progress.completedContentIds.includes(content.id)) {
              this.progress.completedContentIds.push(content.id);
              this.progress.completedCount++;
              this.progress.percentComplete = this.progress.totalCount > 0 
                ? Math.round((this.progress.completedCount / this.progress.totalCount) * 100)
                : 0;
            }
          }
        },
        error: (err) => {
          console.error('Failed to mark content as complete:', err);
        }
      });
    }
  }

  markCurrentContentComplete(): void {
    if (this.content && this.course?.id && this.authService.isLoggedIn()) {
      const request = { contentId: this.content.id, courseId: this.course.id };
      this.progressService.markComplete(request).subscribe({
        next: () => {
          if (this.progress && !this.progress.completedContentIds.includes(this.content!.id)) {
            this.progress.completedContentIds.push(this.content!.id);
            this.progress.completedCount++;
            this.progress.percentComplete = this.progress.totalCount > 0 
              ? Math.round((this.progress.completedCount / this.progress.totalCount) * 100)
              : 0;
          }
        },
        error: (err) => {
          console.error('Failed to mark content as complete:', err);
        }
      });
    }
  }

  getCompletedCountForSection(section: SectionResponse): number {
    if (!this.progress || !section.contents) return 0;
    return section.contents.filter(c => this.isContentCompleted(c.id)).length;
  }
}
