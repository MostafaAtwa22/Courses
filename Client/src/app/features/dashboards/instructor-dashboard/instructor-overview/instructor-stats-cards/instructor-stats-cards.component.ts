import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StatCardComponent } from '../../../../../shared/components/stat-card/stat-card';
import { DashboardMetric } from '../../../models/dashboard.model';

@Component({
  selector: 'app-instructor-stats-cards',
  standalone: true,
  imports: [
    CommonModule,
    StatCardComponent
  ],
  templateUrl: './instructor-stats-cards.component.html',
  styleUrl: './instructor-stats-cards.component.scss'
})
export class InstructorStatsCardsComponent {
  @Input() metrics: DashboardMetric[] = [];
}