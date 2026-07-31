import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CourseResponse } from '../../../models/course.models';
import { InstructorService } from '../../../../instructors/services/instructor.service';
import { InstructorPublicResponse } from '../../../../instructors/models/instructor.models';
import { DecimalPipe } from '../../../../../shared/pipes/decimal.pipe';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-course-instructor',
  standalone: true,
  imports: [CommonModule, DecimalPipe, RouterLink],
  templateUrl: './course-instructor.html',
  styleUrl: './course-instructor.scss'
})
export class CourseInstructorComponent implements OnChanges {
  @Input() course?: CourseResponse;
  instructor?: InstructorPublicResponse;
  isLoadingInstructor = false;
  isExpanded = false;
  private loadedCourseId?: string;

  constructor(private instructorService: InstructorService) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['course'] && this.course?.id && this.course.id !== this.loadedCourseId) {
      this.loadInstructor(this.course.id);
    }
  }

  get instructorName(): string {
    if (this.instructor) {
      return `${this.instructor.firstName} ${this.instructor.lastName}`;
    }

    return this.course?.instructorName || 'Instructor';
  }

  get instructorTitle(): string {
    return this.instructor?.title || this.course?.instructorTitle || '';
  }

  get instructorProfilePicture(): string {
    if (this.instructor?.profilePicture) {
      return this.instructor.profilePicture;
    }

    if (this.course?.instructorProfilePicture) {
      return this.course.instructorProfilePicture;
    }

    return `https://ui-avatars.com/api/?name=${encodeURIComponent(this.instructorName)}&background=random`;
  }

  get instructorRatingDisplay(): string {
    return this.formatNumber(this.instructor?.averageRate ?? this.course?.averageRate);
  }

  get totalReviewsDisplay(): string {
    return this.formatNumber(this.instructor?.totalReviews ?? this.course?.totalReviews);
  }

  get totalStudentsDisplay(): string {
    return this.formatNumber(this.instructor?.totalStudents ?? this.course?.studentCount);
  }

  get totalCoursesDisplay(): string {
    return this.formatNumber(this.instructor?.totalCourses);
  }

  get instructorId(): string {
    return this.instructor?.id || '';
  }

  private loadInstructor(courseId: string): void {
    this.loadedCourseId = courseId;
    this.instructor = undefined;
    this.isLoadingInstructor = true;

    this.instructorService.getPublicInstructorByCourseId(courseId).subscribe({
      next: (data) => {
        this.instructor = data;
        this.isLoadingInstructor = false;
      },
      error: (error) => {
        console.error('Failed to load instructor:', error);
        this.isLoadingInstructor = false;
      }
    });
  }

  private formatNumber(value?: number): string {
    return value === undefined || value === null ? '0' : value.toLocaleString();
  }
}
