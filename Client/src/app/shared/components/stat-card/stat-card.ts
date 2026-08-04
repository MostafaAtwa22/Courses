import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardMetric } from '../../../features/dashboards/models/dashboard.model';

@Component({
  selector: 'app-stat-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './stat-card.html',
  styleUrl: './stat-card.scss'
})
export class StatCardComponent {
  @Input({ required: true }) metric!: DashboardMetric;
}
