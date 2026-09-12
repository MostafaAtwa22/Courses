import { Component, Input, OnChanges, SimpleChanges, ElementRef, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Chart, ChartConfiguration, ChartData, ChartOptions, registerables } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';

export interface EnrollmentData {
  period: string;
  enrollmentCount: number;
}

@Component({
  selector: 'app-enrollment-chart-card',
  standalone: true,
  imports: [CommonModule, BaseChartDirective],
  templateUrl: './enrollment-chart-card.component.html',
  styleUrl: './enrollment-chart-card.component.scss'
})
export class EnrollmentChartCardComponent implements OnInit, OnChanges {
  @Input() enrollmentData: EnrollmentData[] = [];

  isDarkMode = false;

  public lineChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: false
      },
      tooltip: {
        backgroundColor: 'rgba(0, 0, 0, 0.8)',
        titleFont: {
          size: 14,
          weight: 'bold'
        },
        bodyFont: {
          size: 13
        },
        padding: 20,
        cornerRadius: 8,
        displayColors: false,
        callbacks: {
          label: (context) => `Enrollments: ${context.parsed.y}`
        }
      }
    },
    scales: {
      x: {
        grid: {
          display: false
        },
        ticks: {
          font: {
            size: 11
          },
          color: '#64748b'
        }
      },
      y: {
        beginAtZero: true,
        grid: {
          color: 'rgba(0, 0, 0, 0.05)'
        },
        ticks: {
          font: {
            size: 11
          },
          color: '#64748b'
        }
      }
    }
  };

  public lineChartData: ChartData<'line'> = {
    labels: [],
    datasets: [
      {
        label: 'Enrollments',
        data: [],
        borderColor: '#4f46e5',
        backgroundColor: 'rgba(79, 70, 229, 0.1)',
        borderWidth: 3,
        fill: true,
        tension: 0.4,
        pointBackgroundColor: '#4f46e5',
        pointBorderColor: '#ffffff',
        pointBorderWidth: 2,
        pointRadius: 5,
        pointHoverRadius: 7
      }
    ]
  };

  public lineChartType: ChartConfiguration['type'] = 'line';

  constructor(private elementRef: ElementRef) {}

  ngOnInit(): void {
    Chart.register(...registerables);
    this.checkDarkMode();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['enrollmentData'] && this.enrollmentData) {
      this.updateChartData();
    }
  }

  private checkDarkMode(): void {
    const checkTheme = () => {
      const body = document.body;
      this.isDarkMode = body.classList.contains('dark') || body.classList.contains('dark-theme');
      this.updateChartTheme();
    };

    checkTheme();

    const observer = new MutationObserver(() => {
      checkTheme();
    });

    observer.observe(document.body, {
      attributes: true,
      attributeFilter: ['class']
    });
  }

  private updateChartTheme(): void {
    const isDark = this.isDarkMode;

    const textColor = isDark ? '#cbd5e1' : '#64748b';
    const gridColor = isDark ? 'rgba(255, 255, 255, 0.1)' : 'rgba(0, 0, 0, 0.05)';
    const tooltipBg = isDark ? 'rgba(15, 23, 42, 0.9)' : 'rgba(0, 0, 0, 0.8)';
    const lineColor = isDark ? '#818cf8' : '#4f46e5';
    const fillColor = isDark ? 'rgba(129, 140, 248, 0.15)' : 'rgba(79, 70, 229, 0.1)';
    const pointColor = isDark ? '#818cf8' : '#4f46e5';

    const options = this.lineChartOptions as any;

    if (options.scales?.x?.ticks) {
      options.scales.x.ticks.color = textColor;
    }

    if (options.scales?.y?.ticks) {
      options.scales.y.ticks.color = textColor;
    }

    if (options.scales?.y?.grid) {
      options.scales.y.grid.color = gridColor;
    }

    if (options.plugins?.tooltip) {
      options.plugins.tooltip.backgroundColor = tooltipBg;
    }

    this.lineChartData.datasets[0].borderColor = lineColor;
    this.lineChartData.datasets[0].backgroundColor = fillColor;
    this.lineChartData.datasets[0].pointBackgroundColor = pointColor;
  }

  private updateChartData(): void {
    this.lineChartData.labels = this.enrollmentData.map(item => item.period);
    this.lineChartData.datasets[0].data = this.enrollmentData.map(item => item.enrollmentCount);
  }
}
