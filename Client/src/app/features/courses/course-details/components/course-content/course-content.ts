import { Component, EventEmitter, Input, Output, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SectionResponse, CourseProgress } from '../../../models/course.models';
import { ProgressBarComponent } from '../../../../../shared/components/progress-bar/progress-bar';

@Component({
  selector: 'app-course-content',
  standalone: true,
  imports: [CommonModule, ProgressBarComponent],
  templateUrl: './course-content.html',
  styleUrl: './course-content.scss'
})
export class CourseContentComponent implements OnChanges {
  @Input() sections: SectionResponse[] = [];
  @Input() totalSections = 0;
  @Input() loading = false;
  @Input() hasMore = false;
  @Input() loadingContentSectionIds = new Set<string>();
  @Input() progress?: CourseProgress;
  @Output() loadMore = new EventEmitter<void>();
  @Output() sectionOpened = new EventEmitter<string>();
  @Output() contentSelected = new EventEmitter<any>();

  expandedSections = new Set<string>();
  expandAll = false;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['sections'] && !changes['sections'].firstChange && this.expandAll) {
      const currentSections = changes['sections'].currentValue as SectionResponse[];
      const previousSections = changes['sections'].previousValue as SectionResponse[];
      
      const newSections = currentSections.filter(cs => !previousSections.some(ps => ps.id === cs.id));
      
      newSections.forEach(section => {
        this.sectionOpened.emit(section.id);
      });
    }
  }

  getTotalLectures(): number {
    return this.sections?.reduce((acc, s) => acc + (s.contentsCount || 0), 0) || 0;
  }

  formatDuration(seconds: number): string {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = Math.floor(seconds % 60);
    return `${minutes.toString().padStart(2, '0')}:${remainingSeconds.toString().padStart(2, '0')}`;
  }

  toggleSection(section: SectionResponse): void {
    if (this.expandedSections.has(section.id)) {
      this.expandedSections.delete(section.id);
    } else {
      this.expandedSections.add(section.id);
      this.sectionOpened.emit(section.id);
    }
  }

  isSectionExpanded(sectionId: string): boolean {
    return this.expandAll || this.expandedSections.has(sectionId);
  }

  isSectionLoading(sectionId: string): boolean {
    return this.loadingContentSectionIds.has(sectionId);
  }

  toggleExpandAll(): void {
    this.expandAll = !this.expandAll;
    if (this.expandAll) {
      // Load contents for all sections when expanding all
      this.sections.forEach(section => {
        this.sectionOpened.emit(section.id);
      });
    }
  }

  onLoadMore(): void {
    this.loadMore.emit();
  }
  
  onContentClick(content: any): void {
    if (this.isContentRestricted(content)) {
      return; // Don't emit event for restricted content
    }
    this.contentSelected.emit(content);
  }

  isContentRestricted(content: any): boolean {
    return !content.isPreview && !content.contentUrl;
  }

  isContentCompleted(contentId: string): boolean {
    return this.progress?.completedContentIds?.includes(contentId) ?? false;
  }

  getCompletedCountForSection(section: SectionResponse): number {
    if (!this.progress || !section.contents) return 0;
    return section.contents.filter(c => this.isContentCompleted(c.id)).length;
  }
}
