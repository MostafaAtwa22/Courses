import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CourseCardComponent } from '../../../courses/components/course-card/course-card';

@Component({
  selector: 'app-featured-courses',
  standalone: true,
  imports: [CommonModule, CourseCardComponent],
  templateUrl: './featured-courses.html',
  styleUrl: './featured-courses.scss'
})
export class FeaturedCoursesComponent {
  @Input() courses: any[] = [];
}
