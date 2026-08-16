import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReviewCreateRequest } from '../../../models/course.models';

@Component({
  selector: 'app-add-review-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './add-review-modal.html',
  styleUrl: './add-review-modal.scss'
})
export class AddReviewModalComponent {
  @Output() reviewSubmitted = new EventEmitter<ReviewCreateRequest>();
  @Output() modalClosed = new EventEmitter<void>();

  isOpen = false;
  rating = 0;
  hoveredRating = 0;
  headline = '';
  comment = '';
  isSubmitting = false;
  headlineFocused = false;
  commentFocused = false;

  stars = [1, 2, 3, 4, 5];

  openModal(): void {
    this.isOpen = true;
    this.resetForm();
  }

  closeModal(): void {
    this.isOpen = false;
    this.modalClosed.emit();
  }

  resetForm(): void {
    this.rating = 0;
    this.hoveredRating = 0;
    this.headline = '';
    this.comment = '';
  }

  setRating(rating: number): void {
    this.rating = rating;
  }

  getRatingLabel(rating: number): string {
    const labels: Record<number, string> = {
      1: 'Poor',
      2: 'Fair',
      3: 'Good',
      4: 'Very Good',
      5: 'Excellent'
    };
    return labels[rating] ?? '';
  }

  submitReview(): void {
    if (!this.rating || !this.headline.trim() || !this.comment.trim()) {
      return;
    }

    this.isSubmitting = true;

    const reviewRequest: ReviewCreateRequest = {
      courseId: '',
      headline: this.headline.trim(),
      comment: this.comment.trim(),
      rating: this.rating
    };

    this.reviewSubmitted.emit(reviewRequest);
    this.resetForm();
  }

  isFormValid(): boolean {
    return this.rating > 0 && this.headline.trim().length > 0 && this.comment.trim().length > 0;
  }
}
