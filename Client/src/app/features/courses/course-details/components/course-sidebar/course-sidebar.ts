import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CourseResponse } from '../../../models/course.models';

@Component({
  selector: 'app-course-sidebar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './course-sidebar.html',
  styleUrl: './course-sidebar.scss'
})
export class CourseSidebarComponent {
  @Input() course?: CourseResponse;
}
