import { Component, Input, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CourseSummary } from '../../models/course.models';
import { AuthService } from '../../../auth/services/auth.service';
import { SectionService } from '../../services/section.service';
import { ContentService } from '../../services/content.service';
import { ProgressService } from '../../services/progress.service';

@Component({
  selector: 'app-course-card',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './course-card.html',
  styleUrl: './course-card.scss'
})
export class CourseCardComponent implements OnInit {
  @Input() course!: CourseSummary;

  private authService = inject(AuthService);
  private sectionService = inject(SectionService);
  private contentService = inject(ContentService);
  private progressService = inject(ProgressService);

  isEnrolled = false;
  isCheckingEnrollment = false;
  firstContentId?: string;
  isInstructorOrAdmin = false;

  ngOnInit(): void {
    this.isInstructorOrAdmin = this.authService.isInstructorOrAdmin();
    this.checkEnrollment();
  }

  private checkEnrollment(): void {
    if (!this.authService.isLoggedIn()) {
      this.isEnrolled = false;
      return;
    }

    if (!this.authService.isStudent()) {
      this.isEnrolled = false;
      return;
    }

    this.isCheckingEnrollment = true;
    // Use progress service to check enrollment - it will fail for non-enrolled users
    this.progressService.checkEnrollment(this.course.id).subscribe({
      next: (progress) => {
        this.isEnrolled = true;
        this.isCheckingEnrollment = false;
        this.loadFirstContent();
      },
      error: (err) => {
        // User is not enrolled
        this.isEnrolled = false;
        this.isCheckingEnrollment = false;
      }
    });
  }

  private loadFirstContent(): void {
    // Load first section and its first content
    this.sectionService.getByCourseId(this.course.id, { pageSize: 1, sortBy: 'order' }).subscribe({
      next: (result) => {
        if (result.items && result.items.length > 0) {
          const firstSection = result.items[0];
          this.contentService.getBySection(firstSection.id, this.course.id).subscribe({
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

  getPlayerRoute(): string[] {
    if (this.firstContentId) {
      return ['/courses', this.course.id, 'content', this.firstContentId];
    }
    return ['/courses', this.course.id];
  }
}
