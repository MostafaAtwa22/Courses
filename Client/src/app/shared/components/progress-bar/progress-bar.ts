import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CourseProgress } from '../../../features/courses/models/course.models';

@Component({
  selector: 'app-progress-bar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './progress-bar.html',
  styleUrl: './progress-bar.scss'
})
export class ProgressBarComponent {
  @Input() progress?: CourseProgress;
  @Input() showLabel = true;
  @Input() showPercentage = true;
  @Input() compact = false;
  @Input() height = 6;
  @Input() width = 80;
  @Input() label = 'Progress';
}
