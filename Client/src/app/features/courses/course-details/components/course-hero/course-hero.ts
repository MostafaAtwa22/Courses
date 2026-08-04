import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CourseResponse } from '../../../models/course.models';
import { VideoPlayerComponent } from '../../../../../shared/components/video-player/video-player.component';

@Component({
  selector: 'app-course-hero',
  standalone: true,
  imports: [CommonModule, RouterLink, VideoPlayerComponent],
  templateUrl: './course-hero.html',
  styleUrl: './course-hero.scss'
})
export class CourseHeroComponent {
  @Input() course?: CourseResponse;
  lastUpdated = new Date();
  isPlayingVideo = false;

  hasDiscount(): boolean {
    return !!(this.course?.priceAfterDiscount && this.course?.cost && this.course.priceAfterDiscount < this.course.cost);
  }

  discountPercentage(): number {
    if (!this.hasDiscount()) return 0;
    const original = this.course!.cost;
    const discounted = this.course!.priceAfterDiscount!;
    return Math.round(((original - discounted) / original) * 100);
  }

  toggleVideo() {
    this.isPlayingVideo = !this.isPlayingVideo;
  }
}
