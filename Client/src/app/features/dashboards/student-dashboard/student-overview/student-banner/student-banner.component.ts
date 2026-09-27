import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-student-banner',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './student-banner.component.html',
  styleUrl: './student-banner.component.scss'
})
export class StudentBannerComponent {
  @Output() refreshData = new EventEmitter<void>();

  onRefreshData() {
    this.refreshData.emit();
  }
}
