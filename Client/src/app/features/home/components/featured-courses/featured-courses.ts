import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-featured-courses',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './featured-courses.html',
  styleUrl: './featured-courses.scss'
})
export class FeaturedCoursesComponent {
  @Input() courses: any[] = [];
}
