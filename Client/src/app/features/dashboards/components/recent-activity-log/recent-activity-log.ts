import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivityLogItem } from '../../models/dashboard.model';

@Component({
  selector: 'app-recent-activity-log',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './recent-activity-log.html',
  styleUrl: './recent-activity-log.scss'
})
export class RecentActivityLogComponent {
  @Input() activities: ActivityLogItem[] = [];
}
