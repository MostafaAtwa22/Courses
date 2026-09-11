import { Component, Input, OnChanges, SimpleChanges, ElementRef, Renderer2, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Chart, ChartConfiguration, ChartData, ChartOptions, registerables } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { CategoryHistogramData } from '../../../models/dashboard.model';

@Component({
  selector: 'app-category-histogram-card',
  standalone: true,
  imports: [CommonModule, BaseChartDirective],
  templateUrl: './category-histogram-card.component.html',
  styleUrl: './category-histogram-card.component.scss'
})
export class CategoryHistogramCardComponent implements OnInit, OnChanges {
  @Input() categoryData: CategoryHistogramData[] = [];

  isDarkMode = false;

  public barChartOptions: ChartOptions = {
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
        padding: 12,
        cornerRadius: 8,
        displayColors: false
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
          maxRotation: 45,
          minRotation: 45,
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
          stepSize: 1,
          color: '#64748b'
        }
      }
    }
  };

  public barChartData: ChartData<'bar'> = {
    labels: [],
    datasets: [
      {
        data: [],
        backgroundColor: [
          'rgba(79, 70, 229, 0.8)',
          'rgba(6, 182, 212, 0.8)',
          'rgba(236, 72, 153, 0.8)',
          'rgba(16, 185, 129, 0.8)',
          'rgba(245, 158, 11, 0.8)',
          'rgba(139, 92, 246, 0.8)',
          'rgba(239, 68, 68, 0.8)',
          'rgba(34, 197, 94, 0.8)',
          'rgba(251, 191, 36, 0.8)',
          'rgba(59, 130, 246, 0.8)'
        ],
        borderColor: [
          'rgba(79, 70, 229, 1)',
          'rgba(6, 182, 212, 1)',
          'rgba(236, 72, 153, 1)',
          'rgba(16, 185, 129, 1)',
          'rgba(245, 158, 11, 1)',
          'rgba(139, 92, 246, 1)',
          'rgba(239, 68, 68, 1)',
          'rgba(34, 197, 94, 1)',
          'rgba(251, 191, 36, 1)',
          'rgba(59, 130, 246, 1)'
        ],
        borderWidth: 2,
        borderRadius: 6
      }
    ]
  };

  public barChartType: ChartConfiguration['type'] = 'bar';

  constructor(private elementRef: ElementRef) {}

  ngOnInit(): void {
    // Register Chart.js components
    Chart.register(...registerables);
    this.checkDarkMode();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['categoryData'] && this.categoryData) {
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

    // Listen for theme changes
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

    // Update chart colors based on theme
    const textColor = isDark ? '#cbd5e1' : '#64748b';
    const gridColor = isDark ? 'rgba(255, 255, 255, 0.1)' : 'rgba(0, 0, 0, 0.05)';
    const tooltipBg = isDark ? 'rgba(15, 23, 42, 0.9)' : 'rgba(0, 0, 0, 0.8)';

    // Use bracket notation to avoid TypeScript strict typing issues
    const options = this.barChartOptions as any;

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
  }

  private updateChartData(): void {
    this.barChartData.labels = this.categoryData.map(item => item.categoryName);
    this.barChartData.datasets[0].data = this.categoryData.map(item => item.courseCount);
  }
}