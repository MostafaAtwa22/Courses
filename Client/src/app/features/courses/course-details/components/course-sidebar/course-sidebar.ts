import { Component, Input, computed, inject, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { CourseResponse, CourseDiscountResponse } from '../../../models/course.models';
import { VideoPlayerComponent } from '../../../../../shared/components/video-player/video-player.component';
import { ProgressService } from '../../../services/progress.service';
import { SectionService } from '../../../services/section.service';
import { ContentService } from '../../../services/content.service';
import { AuthService } from '../../../../auth/services/auth.service';

@Component({
  selector: 'app-course-sidebar',
  standalone: true,
  imports: [CommonModule, VideoPlayerComponent],
  templateUrl: './course-sidebar.html',
  styleUrl: './course-sidebar.scss'
})
export class CourseSidebarComponent implements OnInit, OnChanges {
  @Input() course?: CourseResponse;
  
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private progressService = inject(ProgressService);
  private authService = inject(AuthService);
  private sectionService = inject(SectionService);
  private contentService = inject(ContentService);
  
  isPlayingVideo = false;
  isEnrolled = false;
  isCheckingEnrollment = false;
  firstContentId?: string;

  ngOnInit(): void {
    this.checkEnrollment();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['course'] && this.course) {
      this.checkEnrollment();
    }
  }

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

  private checkEnrollment(): void {
    if (!this.course) return;

    if (!this.authService.isLoggedIn()) {
      this.isEnrolled = false;
      return;
    }

    this.isCheckingEnrollment = true;
    this.progressService.getCourseProgress(this.course.id).subscribe({
      next: () => {
        this.isEnrolled = true;
        this.isCheckingEnrollment = false;
        this.loadFirstContent();
      },
      error: () => {
        this.isEnrolled = false;
        this.isCheckingEnrollment = false;
      }
    });
  }

  private loadFirstContent(): void {
    if (!this.course) return;

    // Load first section and its first content
    this.sectionService.getByCourseId(this.course.id, { pageSize: 1, sortBy: 'order' }).subscribe({
      next: (result) => {
        if (result.items && result.items.length > 0) {
          const firstSection = result.items[0];
          this.contentService.getBySection(firstSection.id, this.course!.id).subscribe({
            next: (contents) => {
              if (contents && contents.length > 0) {
                this.firstContentId = contents[0].id;
              }
            }
          });
        }
      }
    });
  }

  goToCourse(): void {
    if (!this.course || !this.firstContentId) return;
    this.router.navigate(['content', this.firstContentId], { relativeTo: this.route });
  }
}
