import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ContentService } from '../../../services/content.service';
import { CourseService } from '../../../services/course.service';
import { SectionService } from '../../../services/section.service';
import { ContentResponse, ContentType, CourseResponse, SectionResponse } from '../../../models/course.models';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-content-player',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './content-player.html',
  styleUrl: './content-player.scss'
})
export class ContentPlayerComponent implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private contentService = inject(ContentService);
  private courseService = inject(CourseService);
  private sectionService = inject(SectionService);

  course?: CourseResponse;
  content?: ContentResponse;
  sections: SectionResponse[] = [];
  
  loading = true;
  error: string | null = null;
  ContentType = ContentType;

  expandedSections = new Set<string>();
  
  // Flattened list of all contents for navigation
  allContents: ContentResponse[] = [];
  nextContent?: ContentResponse;
  previousContent?: ContentResponse;

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

  loadCurrentContent(id: string): void {
    this.loading = true;
    this.contentService.getById(id).subscribe({
      next: (content) => {
        this.content = content;
        this.loading = false;
        this.expandedSections.add(content.sectionId);
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

    // 2. Get Sections
    this.sectionService.getByCourseId(courseId, { pageSize: 100 }).subscribe({
      next: (result) => {
        this.sections = result.items || [];
        
        // 3. For each section, load its contents
        const contentObservables = this.sections.map(s => this.contentService.getBySection(s.id));
        
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
    this.router.navigate(['../', content.id], { relativeTo: this.route });
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
}
