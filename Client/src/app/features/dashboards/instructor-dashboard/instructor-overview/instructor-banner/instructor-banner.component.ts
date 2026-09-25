import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-instructor-banner',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './instructor-banner.component.html',
  styleUrl: './instructor-banner.component.scss'
})
export class InstructorBannerComponent {
  @Output() createLecture = new EventEmitter<void>();

  onCreateLecture() {
    this.createLecture.emit();
  }
}