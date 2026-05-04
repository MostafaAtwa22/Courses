import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReviewResponse, CourseResponse } from '../../../models/course.models';

@Component({
  selector: 'app-course-reviews',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './course-reviews.html',
  styleUrl: './course-reviews.scss'
})
export class CourseReviewsComponent implements OnChanges {
  @Input() reviews: ReviewResponse[] = [];
  @Input() course?: CourseResponse;

  ratingDistribution: { star: number, percentage: number }[] = [
    { star: 5, percentage: 0 },
    { star: 4, percentage: 0 },
    { star: 3, percentage: 0 },
    { star: 2, percentage: 0 },
    { star: 1, percentage: 0 }
  ];

  averageRating = 0;
  totalCount = 0;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['reviews'] && this.reviews) {
      this.calculateFeedback();
    }
  }

  calculateFeedback(): void {
    if (!this.reviews || this.reviews.length === 0) {
      this.totalCount = 0;
      this.averageRating = this.course?.averageRate || 0;
      this.ratingDistribution.forEach(d => d.percentage = 0);
      return;
    }

    this.totalCount = this.reviews.length;
    const sum = this.reviews.reduce((acc, r) => acc + r.rating, 0);
    this.averageRating = sum / this.totalCount;

    // Calculate distribution
    const counts = [0, 0, 0, 0, 0]; // 1 to 5 stars
    this.reviews.forEach(r => {
      const roundedRating = Math.round(r.rating);
      if (roundedRating >= 1 && roundedRating <= 5) {
        counts[roundedRating - 1]++;
      }
    });

    this.ratingDistribution = [5, 4, 3, 2, 1].map(star => ({
      star: star,
      percentage: (counts[star - 1] / this.totalCount) * 100
    }));
  }

  getInitials(name: string): string {
    if (!name) return 'U';
    return name.split(' ').map(n => n[0]).join('').toUpperCase();
  }

  getStars(rating: number): number[] {
    return Array(Math.floor(rating)).fill(0);
  }

  hasHalfStar(rating: number): boolean {
    return rating % 1 >= 0.5;
  }

  getEmptyStars(rating: number): number[] {
    const fullStars = Math.floor(rating);
    const halfStar = this.hasHalfStar(rating) ? 1 : 0;
    return Array(5 - fullStars - halfStar).fill(0);
  }
}
