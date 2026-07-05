import { Component, Input, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CourseResponse, CourseDiscountResponse } from '../../../models/course.models';

@Component({
  selector: 'app-course-sidebar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './course-sidebar.html',
  styleUrl: './course-sidebar.scss'
})
export class CourseSidebarComponent {
  @Input() course?: CourseResponse;
  isPlayingVideo = false;

  hasDiscount = computed(() => {
    return this.course && this.course.cost > this.course.priceAfterDiscount;
  });

  discountPercentage = computed(() => {
    if (!this.hasDiscount() || !this.course) return 0;
    return Math.round(((this.course.cost - this.course.priceAfterDiscount) / this.course.cost) * 100);
  });

  toggleVideo() {
    this.isPlayingVideo = !this.isPlayingVideo;
  }
}
