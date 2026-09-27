import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { EnrolledCourseWithProgress } from '../models';

@Component({
  selector: 'app-continue-learning',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './continue-learning.component.html',
  styleUrl: './continue-learning.component.scss'
})
export class ContinueLearningComponent {
  @Input() recentCourses: EnrolledCourseWithProgress[] = [];

  getProgressColor(percentage: number): string {
    if (percentage >= 75) return 'bg-success';
    if (percentage >= 50) return 'bg-primary';
    if (percentage >= 25) return 'bg-warning';
    return 'bg-danger';
  }

  getProgressBadgeClass(percentage: number): string {
    if (percentage === 100) return 'badge-completed';
    if (percentage > 0) return 'badge-in-progress';
    return 'badge-not-started';
  }

  getProgressLabel(percentage: number): string {
    if (percentage === 100) return 'Completed';
    if (percentage > 0) return 'In Progress';
    return 'Not Started';
  }
}
