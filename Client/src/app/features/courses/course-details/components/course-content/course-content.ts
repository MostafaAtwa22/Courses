import { Component, EventEmitter, Input, Output, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SectionResponse } from '../../../models/course.models';

@Component({
  selector: 'app-course-content',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './course-content.html',
  styleUrl: './course-content.scss'
})
export class CourseContentComponent implements OnChanges {
  @Input() sections: SectionResponse[] = [];
  @Input() totalSections = 0;
  @Input() loading = false;
  @Input() hasMore = false;
  @Input() loadingContentSectionIds = new Set<string>();
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

  getContentIcon(type: number): string {
    switch (type) {
      case 0: return 'fa-play-circle'; // Video
      case 1: return 'fa-file-alt'; // Document
      case 2: return 'fa-question-circle'; // Quiz
      default: return 'fa-play-circle';
    }
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
    this.contentSelected.emit(content);
  }
}
