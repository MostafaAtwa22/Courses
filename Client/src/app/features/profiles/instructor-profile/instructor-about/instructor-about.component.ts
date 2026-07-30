import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-instructor-about',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './instructor-about.component.html',
  styleUrl: './instructor-about.component.scss'
})
export class InstructorAboutComponent {
  @Input() instructorTitle: string = '';
  @Input() instructorBio: string = '';
}
