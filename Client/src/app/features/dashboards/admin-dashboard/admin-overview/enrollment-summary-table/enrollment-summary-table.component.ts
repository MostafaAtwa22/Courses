import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface EnrollmentData {
  period: string;
  enrollmentCount: number;
}

@Component({
  selector: 'app-enrollment-summary-table',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './enrollment-summary-table.component.html',
  styleUrl: './enrollment-summary-table.component.scss'
})
export class EnrollmentSummaryTableComponent implements OnChanges {
  @Input() enrollmentData: EnrollmentData[] = [];

  totalEnrollments: number = 0;
  averageEnrollments: number = 0;
  highestMonth: string = '';
  highestCount: number = 0;
  growthRate: number = 0;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['enrollmentData'] && this.enrollmentData) {
      this.calculateSummary();
    }
  }

  private calculateSummary(): void {
    if (!this.enrollmentData || this.enrollmentData.length === 0) {
      this.totalEnrollments = 0;
      this.averageEnrollments = 0;
      this.highestMonth = 'N/A';
      this.highestCount = 0;
      this.growthRate = 0;
      return;
    }

    this.totalEnrollments = this.enrollmentData.reduce((sum, item) => sum + item.enrollmentCount, 0);
    this.averageEnrollments = Math.round(this.totalEnrollments / this.enrollmentData.length);

    const highest = this.enrollmentData.reduce((max, item) => 
      item.enrollmentCount > max.enrollmentCount ? item : max
    );
    this.highestMonth = highest.period;
    this.highestCount = highest.enrollmentCount;

    // Calculate growth rate (last month vs first month)
    const firstMonth = this.enrollmentData[0].enrollmentCount;
    const lastMonth = this.enrollmentData[this.enrollmentData.length - 1].enrollmentCount;
    
    if (firstMonth > 0) {
      this.growthRate = Math.round(((lastMonth - firstMonth) / firstMonth) * 100);
    } else {
      this.growthRate = lastMonth > 0 ? 100 : 0;
    }
  }
}
