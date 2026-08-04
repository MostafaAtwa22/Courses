import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AttendanceSummary } from '../../models/dashboard.model';

@Component({
  selector: 'app-attendance-overview',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './attendance-overview.html',
  styleUrl: './attendance-overview.scss'
})
export class AttendanceOverviewComponent {
  @Input() summary: AttendanceSummary | null = null;
}
