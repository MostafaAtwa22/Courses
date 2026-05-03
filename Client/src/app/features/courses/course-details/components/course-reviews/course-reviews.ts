import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReviewResponse } from '../../../models/course.models';

@Component({
  selector: 'app-course-reviews',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './course-reviews.html',
  styleUrl: './course-reviews.scss'
})
export class CourseReviewsComponent {
  @Input() reviews?: ReviewResponse[];

  getInitials(name: string): string {
    if (!name) return 'U';
    return name.split(' ').map(n => n[0]).join('').toUpperCase();
  }
}
