import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-instructor-stats',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './instructor-stats.component.html',
  styleUrl: './instructor-stats.component.scss'
})
export class InstructorStatsComponent {
  @Input() totalCourses: number = 0;
  @Input() totalStudents: number = 0;
  @Input() totalReviews: number = 0;
  @Input() averageRate: number = 0;

  get formattedAverageRate(): string {
    return this.averageRate > 0 ? `${this.averageRate.toFixed(1)}/5` : 'N/A';
  }
}
