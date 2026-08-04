import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CourseAnalytics } from '../../models/dashboard.model';

@Component({
  selector: 'app-course-analytics',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './course-analytics.html',
  styleUrl: './course-analytics.scss'
})
export class CourseAnalyticsComponent {
  @Input() courses: CourseAnalytics[] = [];
}
