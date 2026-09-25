import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { DashboardMetric } from '../models/dashboard.model';

export interface InstructorStatistics {
  enrolledStudents: number;
  enrolledStudentsChange: number;
  monthlyEarnings: number;
  monthlyEarningsChange: number;
  instructorRating: number;
  instructorRatingChange: number;
  coursesCreated: number;
  coursesCreatedChange: number;
}

export interface EnrollmentStatistics {
  period: string;
  enrollmentCount: number;
}

@Injectable({
  providedIn: 'root'
})
export class InstructorDashboardService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/instructor/dashboard`;

  getInstructorStatistics(): Observable<DashboardMetric[]> {
    return this.http.get<InstructorStatistics>(`${this.apiUrl}/statistics`).pipe(
      map((stats) => [
        {
          id: 'is1',
          title: 'Enrolled Students',
          value: stats.enrolledStudents,
          change: `${stats.enrolledStudentsChange > 0 ? '+' : ''}${stats.enrolledStudentsChange}%`,
          isPositive: stats.enrolledStudentsChange >= 0,
          icon: 'fa-solid fa-user-graduate',
          color: 'primary',
          description: 'across your courses'
        },
        {
          id: 'is2',
          title: 'Monthly Earnings',
          value: `$${stats.monthlyEarnings.toLocaleString()}`,
          change: `${stats.monthlyEarningsChange > 0 ? '+' : ''}${stats.monthlyEarningsChange}%`,
          isPositive: stats.monthlyEarningsChange >= 0,
          icon: 'fa-solid fa-wallet',
          color: 'success',
          description: 'this month'
        },
        {
          id: 'is3',
          title: 'Instructor Rating',
          value: `${stats.instructorRating.toFixed(1)} / 5`,
          change: `${stats.instructorRatingChange > 0 ? '+' : ''}${stats.instructorRatingChange}`,
          isPositive: stats.instructorRatingChange >= 0,
          icon: 'fa-solid fa-star',
          color: 'purple',
          description: 'student reviews'
        },
        {
          id: 'is4',
          title: 'Courses Created',
          value: stats.coursesCreated,
          change: `${stats.coursesCreatedChange > 0 ? '+' : ''}${stats.coursesCreatedChange}%`,
          isPositive: stats.coursesCreatedChange >= 0,
          icon: 'fa-solid fa-graduation-cap',
          color: 'warning',
          description: 'published courses'
        }
      ])
    );
  }

  getInstructorEnrollmentStatistics(): Observable<EnrollmentStatistics[]> {
    return this.http.get<EnrollmentStatistics[]>(`${this.apiUrl}/enrollment-statistics`);
  }
}