import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SectionResponse } from '../../../models/course.models';

@Component({
  selector: 'app-course-content',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './course-content.html',
  styleUrl: './course-content.scss'
})
export class CourseContentComponent {
  @Input() sections?: SectionResponse[];
  expandAll = false;

  getTotalLectures(): number {
    return this.sections?.reduce((acc, s) => acc + s.contents.length, 0) || 0;
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
