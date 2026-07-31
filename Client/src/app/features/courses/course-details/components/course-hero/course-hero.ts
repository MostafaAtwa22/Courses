import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CourseResponse } from '../../../models/course.models';

@Component({
  selector: 'app-course-hero',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './course-hero.html',
  styleUrl: './course-hero.scss'
})
export class CourseHeroComponent {
  @Input() course?: CourseResponse;
  lastUpdated = new Date();
}
