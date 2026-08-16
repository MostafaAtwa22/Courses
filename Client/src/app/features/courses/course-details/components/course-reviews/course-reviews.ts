import { Component, Input, OnChanges, SimpleChanges, Output, EventEmitter, ViewChild, inject, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReviewResponse, CourseResponse, ReviewCreateRequest, ReviewUpdateRequest } from '../../../models/course.models';
import { AddReviewModalComponent } from '../add-review-modal/add-review-modal';
import { ReviewService } from '../../../services/review.service';
import { AuthService } from '../../../../../features/auth/services/auth.service';
import { Gender } from '../../../../../shared/models/identity.models';

@Component({
  selector: 'app-course-reviews',
  standalone: true,
  imports: [CommonModule, FormsModule, AddReviewModalComponent],
  templateUrl: './course-reviews.html',
  styleUrl: './course-reviews.scss'
})
export class CourseReviewsComponent implements OnChanges {
  @Input() reviews: ReviewResponse[] = [];
  @Input() course?: CourseResponse;
  @Input() isEnrolled: boolean = false;
  @Input() hasReviewed: boolean = false;
  @Input() courseId: string = '';
  @Output() reviewAdded = new EventEmitter<void>();

  @ViewChild(AddReviewModalComponent) addReviewModal!: AddReviewModalComponent;
  private reviewService = inject(ReviewService);
  private authService  = inject(AuthService);

  isSubmitting = false;
  userReview: ReviewResponse | null = null;

  // ── Inline write form ──────────────────────────────────────────────────────
  isFormOpen   = false;
  formRating   = 0;
  hoveredStar  = 0;
  formHeadline = '';
  formComment  = '';
  stars = [1, 2, 3, 4, 5];

  // ── Edit form ──────────────────────────────────────────────────────────────
  editingReviewId: string | null = null;
  editRating      = 0;
  editHoveredStar = 0;
  editHeadline    = '';
  editComment     = '';

  // ── Action menu (3-dot) ───────────────────────────────────────────────────
  openMenuId: string | null = null;

  // ── Rating summary ────────────────────────────────────────────────────────
  ratingDistribution: { star: number; percentage: number }[] = [
    { star: 5, percentage: 0 },
    { star: 4, percentage: 0 },
    { star: 3, percentage: 0 },
    { star: 2, percentage: 0 },
    { star: 1, percentage: 0 },
  ];
  averageRating = 0;
  totalCount    = 0;

  // ── Current user helpers ──────────────────────────────────────────────────
  get currentUser() { return this.authService.currentUser(); }

  get currentUserAvatar(): string {
    const u = this.currentUser;
    if (!u) return '';
    if (u.profilePicture) return u.profilePicture;
    return u.gender === Gender.Female
      ? 'assets/users/default-female.png'
      : 'assets/users/default-male.png';
  }

  get currentUserInitials(): string {
    const u = this.currentUser;
    if (!u) return 'U';
    return `${u.firstName?.[0] ?? ''}${u.lastName?.[0] ?? ''}`.toUpperCase() || 'U';
  }

  isOwnReview(review: ReviewResponse): boolean {
    const u = this.currentUser;
    if (!u) return false;
    // match by studentId if API returns it, fallback to name match
    if (review.studentId) return review.studentId === u.id;
    return review.studentName === `${u.firstName} ${u.lastName}`;
  }

  getReviewAvatar(review: ReviewResponse): string {
    if (review.studentProfilePicture) return review.studentProfilePicture;
    const isFemale = review.studentGender === Gender.Female
                  || review.studentGender === 'Female'
                  || review.studentGender === '1';
    return isFemale
      ? 'assets/users/default-female.png'
      : 'assets/users/default-male.png';
  }

  // ── Lifecycle ─────────────────────────────────────────────────────────────
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['reviews'] && this.reviews) {
      this.calculateFeedback();
    }
    if (changes['courseId'] && this.courseId) {
      this.loadUserReview();
    }
  }

  loadUserReview(): void {
    if (!this.courseId) return;
    this.reviewService.getUserReview(this.courseId).subscribe({
      next: (review) => {
        this.userReview = review;
      },
      error: (err) => console.error('Failed to load user review', err)
    });
  }

  calculateFeedback(): void {
    if (!this.reviews || this.reviews.length === 0) {
      this.totalCount   = 0;
      this.averageRating = this.course?.averageRate || 0;
      this.ratingDistribution.forEach(d => (d.percentage = 0));
      return;
    }
    this.totalCount   = this.reviews.length;
    const sum          = this.reviews.reduce((acc, r) => acc + r.rating, 0);
    this.averageRating = sum / this.totalCount;

    const counts = [0, 0, 0, 0, 0];
    this.reviews.forEach(r => {
      const n = Math.round(r.rating);
      if (n >= 1 && n <= 5) counts[n - 1]++;
    });
    this.ratingDistribution = [5, 4, 3, 2, 1].map(star => ({
      star,
      percentage: (counts[star - 1] / this.totalCount) * 100,
    }));
  }

  // ── Star helpers ──────────────────────────────────────────────────────────
  getStars(rating: number): number[]      { return Array(Math.floor(rating)).fill(0); }
  hasHalfStar(rating: number): boolean    { return rating % 1 >= 0.5; }
  getEmptyStars(rating: number): number[] {
    return Array(5 - Math.floor(rating) - (this.hasHalfStar(rating) ? 1 : 0)).fill(0);
  }

  getRatingLabel(rating: number): string {
    return ({ 1: 'Poor', 2: 'Fair', 3: 'Good', 4: 'Very Good', 5: 'Excellent' } as Record<number,string>)[rating] ?? '';
  }

  getInitials(name: string): string {
    if (!name) return 'U';
    return name.split(' ').map(n => n[0]).join('').toUpperCase();
  }

  // ── Write form ────────────────────────────────────────────────────────────
  cancelForm(): void {
    this.isFormOpen   = false;
    this.formRating   = 0;
    this.hoveredStar  = 0;
    this.formHeadline = '';
    this.formComment  = '';
  }

  isInlineFormValid(): boolean {
    return this.formRating > 0
      && this.formHeadline.trim().length > 0
      && this.formComment.trim().length > 0;
  }

  submitInlineReview(): void {
    if (!this.isInlineFormValid()) return;
    this.isSubmitting = true;
    const request: ReviewCreateRequest = {
      courseId: this.courseId,
      headline: this.formHeadline.trim(),
      comment:  this.formComment.trim(),
      rating:   this.formRating,
    };
    this.reviewService.createReview(request).subscribe({
      next: () => { this.isSubmitting = false; this.cancelForm(); this.reviewAdded.emit(); },
      error: (err) => { this.isSubmitting = false; console.error(err); },
    });
  }

  // ── 3-dot menu ────────────────────────────────────────────────────────────
  toggleMenu(reviewId: string, event: Event): void {
    event.stopPropagation();
    this.openMenuId = this.openMenuId === reviewId ? null : reviewId;
    // close edit form if switching
    if (this.editingReviewId && this.editingReviewId !== reviewId) {
      this.cancelEdit();
    }
  }

  @HostListener('document:click')
  closeMenu(): void { this.openMenuId = null; }

  // ── Edit ──────────────────────────────────────────────────────────────────
  startEdit(review: ReviewResponse): void {
    this.openMenuId      = null;
    this.editingReviewId = review.id;
    this.editRating      = review.rating;
    this.editHoveredStar = 0;
    this.editHeadline    = review.headline;
    this.editComment     = review.comment;
  }

  cancelEdit(): void {
    this.editingReviewId = null;
    this.editRating      = 0;
    this.editHoveredStar = 0;
    this.editHeadline    = '';
    this.editComment     = '';
  }

  isEditFormValid(): boolean {
    return this.editRating > 0
      && this.editHeadline.trim().length > 0
      && this.editComment.trim().length > 0;
  }

  saveEdit(review: ReviewResponse): void {
    if (!this.isEditFormValid()) return;
    this.isSubmitting = true;
    const req: ReviewUpdateRequest = {
      headline: this.editHeadline.trim(),
      comment:  this.editComment.trim(),
      rating:   this.editRating,
    };
    this.reviewService.updateReview(review.id, req).subscribe({
      next: () => {
        // update locally
        review.headline = req.headline;
        review.comment  = req.comment;
        review.rating   = req.rating;
        this.userReview = review;
        this.isSubmitting = false;
        this.cancelEdit();
        this.calculateFeedback();
      },
      error: (err) => { this.isSubmitting = false; console.error(err); },
    });
  }

  // ── Delete ────────────────────────────────────────────────────────────────
  deleteReview(review: ReviewResponse): void {
    this.openMenuId = null;
    if (!confirm('Delete your review?')) return;
    this.reviewService.deleteReview(review.id).subscribe({
      next: () => {
        this.userReview = null;
        this.reviewAdded.emit(); /* parent reloads list */
      },
      error: (err) => console.error(err),
    });
  }

  // ── Legacy (keeps ViewChild alive) ────────────────────────────────────────
  openAddReviewModal(): void { this.addReviewModal.openModal(); }
  onReviewSubmitted(request: ReviewCreateRequest): void {
    request.courseId = this.courseId;
    this.reviewService.createReview(request).subscribe({
      next: () => { this.reviewAdded.emit(); this.addReviewModal.closeModal(); },
      error: (err) => console.error(err),
    });
  }
}
