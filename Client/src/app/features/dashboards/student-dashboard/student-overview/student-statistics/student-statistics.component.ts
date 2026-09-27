import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StudentStatistics } from '../models';

@Component({
  selector: 'app-student-statistics',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './student-statistics.component.html',
  styleUrl: './student-statistics.component.scss'
})
export class StudentStatisticsComponent {
  @Input() statistics: StudentStatistics = {
    totalEnrolled: 0,
    totalPaid: 0,
    completedCourses: 0,
    inProgressCourses: 0
  };
}
