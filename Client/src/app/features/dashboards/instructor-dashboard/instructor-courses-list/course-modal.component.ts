import { Component, inject, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CourseService } from '../../../courses/services/course.service';
import { CourseSummary } from '../../../courses/models/course.models';
import { CategoryResponse } from '../../../categories/models/category.models';
import { CourseResponse } from '../../../courses/models/course.models';
import { ToastService } from '../../../../core/services/toast.service';

interface CourseFormData {
  title: string;
  description: string;
  categoryId: string;
  cost: number;
  language: string;
  status: string;
  whatYouWillLearn: string;
  requirements: string;
  pictureUrl: File | null;
  introVideo: File | null;
}

@Component({
  selector: 'app-course-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './course-modal.component.html',
  styleUrl: './course-modal.component.scss'
})
export class CourseModalComponent {
  private courseService = inject(CourseService);
  private toastService = inject(ToastService);

  @Input() course: CourseSummary | null = null;
  @Input() categories: CategoryResponse[] = [];
  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<void>();

  isSubmitting = false;
  formData: CourseFormData = {
    title: '',
    description: '',
    categoryId: '',
    cost: 0,
    language: 'English',
    status: '0',
    whatYouWillLearn: '',
    requirements: '',
    pictureUrl: null,
    introVideo: null
  };

  ngOnInit() {
    if (this.course) {
      this.loadCourseDetails();
    }
  }

  loadCourseDetails() {
    if (!this.course) return;
    
    this.courseService.getById(this.course.id).subscribe({
      next: (courseDetails: CourseResponse) => {
        // Find category ID by matching category name
        const category = this.categories.find(cat => cat.name === courseDetails.category);
        const categoryId = category ? category.id : '';
        
        this.formData = {
          title: courseDetails.title,
          description: courseDetails.description,
          categoryId: categoryId,
          cost: courseDetails.cost,
          language: courseDetails.language,
          status: courseDetails.status.toString(),
          whatYouWillLearn: courseDetails.whatYouWillLearn.join(', '),
          requirements: courseDetails.requirements.join(', '),
          pictureUrl: null,
          introVideo: null
        };
      },
      error: () => {
        this.toastService.error('Failed to load course details');
      }
    });
  }

  onClose() {
    this.close.emit();
  }

  onSubmit() {
    if (!this.formData.title || !this.formData.description || !this.formData.categoryId) {
      this.toastService.error('Please fill in all required fields');
      return;
    }

    this.isSubmitting = true;

    const formData = new FormData();
    formData.append('Title', this.formData.title);
    formData.append('Description', this.formData.description);
    formData.append('CategoryId', this.formData.categoryId);
    formData.append('Cost', this.formData.cost.toString());
    formData.append('Language', this.formData.language);
    formData.append('Status', this.formData.status);
    formData.append('WhatYouWillLearn', this.parseArrayInput(this.formData.whatYouWillLearn));
    formData.append('Requirements', this.parseArrayInput(this.formData.requirements));

    if (this.formData.pictureUrl) {
      formData.append('PictureUrl', this.formData.pictureUrl);
    }

    if (this.formData.introVideo) {
      formData.append('IntroVideo', this.formData.introVideo);
    }

    if (this.course) {
      // Update existing course
      this.courseService.update(this.course.id, formData).subscribe({
        next: () => {
          this.toastService.success('Course updated successfully');
          this.saved.emit();
          this.isSubmitting = false;
        },
        error: () => {
          this.toastService.error('Failed to update course');
          this.isSubmitting = false;
        }
      });
    } else {
      // Create new course
      if (!this.formData.pictureUrl) {
        this.toastService.error('Course picture is required');
        this.isSubmitting = false;
        return;
      }
      if (!this.formData.introVideo) {
        this.toastService.error('Intro video is required');
        this.isSubmitting = false;
        return;
      }

      this.courseService.create(formData).subscribe({
        next: () => {
          this.toastService.success('Course created successfully');
          this.saved.emit();
          this.isSubmitting = false;
        },
        error: () => {
          this.toastService.error('Failed to create course');
          this.isSubmitting = false;
        }
      });
    }
  }

  parseArrayInput(input: string): string {
    return JSON.stringify(input.split(',').map(item => item.trim()).filter(item => item.length > 0));
  }

  onPictureSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.formData.pictureUrl = input.files[0];
    }
  }

  onVideoSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.formData.introVideo = input.files[0];
    }
  }
}
