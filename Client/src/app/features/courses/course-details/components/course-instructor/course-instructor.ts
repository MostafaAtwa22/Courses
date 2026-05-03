import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CourseResponse } from '../../../models/course.models';

@Component({
  selector: 'app-course-instructor',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './course-instructor.html',
  styleUrl: './course-instructor.scss'
})
export class CourseInstructorComponent {
  @Input() course?: CourseResponse;
  isExpanded = false;
}
