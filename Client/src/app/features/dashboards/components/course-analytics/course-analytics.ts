import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
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

  constructor(private router: Router) {}

  get topCourses(): CourseAnalytics[] {
    return this.courses.slice(0, 5);
  }

  viewAllCourses() {
    this.router.navigate(['/admin/dashboard/courses']);
  }
}
