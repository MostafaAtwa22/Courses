import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ScheduleItem } from '../../models/dashboard.model';

@Component({
  selector: 'app-schedule-timeline',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './schedule-timeline.html',
  styleUrl: './schedule-timeline.scss'
})
export class ScheduleTimelineComponent {
  @Input() schedule: ScheduleItem[] = [];
}
