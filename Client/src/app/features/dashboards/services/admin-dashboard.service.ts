import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { DashboardMetric, CategoryHistogramData, CourseAnalytics } from '../models/dashboard.model';
import { CategoryService } from '../../categories/services/category.service';

export interface RoleStatistics {
  superAdminCount: number;
  superAdminChange: number;
  adminCount: number;
  adminChange: number;
  instructorCount: number;
  instructorChange: number;
  studentCount: number;
  studentChange: number;
}

export interface InstructorStatusStatistics {
  verifiedCount: number;
  verifiedChange: number;
  pendingCount: number;
  pendingChange: number;
  unverifiedCount: number;
  unverifiedChange: number;
}

export interface PendingInstructor {
  id: string;
  name: string;
  email: string;
  expertise: string;
  appliedDate: string;
  avatar: string;
}

export interface InstructorApiResponse {
  items: InstructorItem[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface InstructorItem {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  bio: string;
  title: string;
  phoneNumber: string;
  cvUrl: string;
  linkedInProfileUrl: string;
  gitHubProfileUrl: string;
  averageRate: number;
  totalReviews: number;
  totalStudents: number;
  totalCourses: number;
  status: string;
  createdAt: string;
  updatedAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class AdminDashboardService {
  private http = inject(HttpClient);
  private categoryService = inject(CategoryService);
  private apiUrl = `${environment.apiUrl}/api/admin/dashboard`;

  getRoleStatistics(): Observable<DashboardMetric[]> {
    return this.http.get<RoleStatistics>(`${this.apiUrl}/role-statistics`).pipe(
      map((stats) => [
        {
          id: 'rs1',
          title: 'Super Admins',
          value: stats.superAdminCount,
          change: stats.superAdminChange >= 0 ? `+${stats.superAdminChange}%` : `${stats.superAdminChange}%`,
          isPositive: stats.superAdminChange >= 0,
          icon: 'fa-solid fa-user-shield',
          color: 'primary',
          description: 'platform administrators'
        },
        {
          id: 'rs2',
          title: 'Admins',
          value: stats.adminCount,
          change: stats.adminChange >= 0 ? `+${stats.adminChange}%` : `${stats.adminChange}%`,
          isPositive: stats.adminChange >= 0,
          icon: 'fa-solid fa-user-tie',
          color: 'success',
          description: 'system administrators'
        },
        {
          id: 'rs3',
          title: 'Instructors',
          value: stats.instructorCount,
          change: stats.instructorChange >= 0 ? `+${stats.instructorChange}%` : `${stats.instructorChange}%`,
          isPositive: stats.instructorChange >= 0,
          icon: 'fa-solid fa-chalkboard-user',
          color: 'warning',
          description: 'course instructors'
        },
        {
          id: 'rs4',
          title: 'Students',
          value: stats.studentCount,
          change: stats.studentChange >= 0 ? `+${stats.studentChange}%` : `${stats.studentChange}%`,
          isPositive: stats.studentChange >= 0,
          icon: 'fa-solid fa-user-graduate',
          color: 'purple',
          description: 'enrolled students'
        }
      ])
    );
  }

  getPendingInstructors(): Observable<PendingInstructor[]> {
    return this.http.get<InstructorApiResponse>(`${environment.apiUrl}/instructors/admin/all`, {
      params: {
        status: 'Pending',
        pageNumber: '1',
        pageSize: '5'
      }
    }).pipe(
      map((response) => response.items.map((instructor) => ({
        id: instructor.id,
        name: `${instructor.firstName} ${instructor.lastName}`,
        email: instructor.email,
        expertise: instructor.title || 'Not specified',
        appliedDate: this.formatDate(instructor.createdAt),
        avatar: `https://ui-avatars.com/api/?name=${instructor.firstName}+${instructor.lastName}&background=random`
      })))
    );
  }

  private formatDate(dateString: string): string {
    const date = new Date(dateString);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 60) return `${diffMins} mins ago`;
    if (diffHours < 24) return `${diffHours} hours ago`;
    if (diffDays === 1) return '1 day ago';
    return `${diffDays} days ago`;
  }

  changeInstructorStatus(id: string, status: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/instructors/admin/${id}/status`, { status });
  }

  getTopCategoriesByCourseCount(): Observable<CategoryHistogramData[]> {
    return this.categoryService.getAll({ pageNumber: 1, pageSize: 100 }).pipe(
      map((response) => {
        const sortedCategories = response.items
          .sort((a: any, b: any) => b.numberOfCourses - a.numberOfCourses)
          .slice(0, 10);

        return sortedCategories.map((category: any) => ({
          id: category.id,
          categoryName: category.name,
          courseCount: category.numberOfCourses
        }));
      })
    );
  }

  getTopPerformingCourses(limit: number = 5): Observable<CourseAnalytics[]> {
    return this.http.get<any[]>(`${this.apiUrl}/courses/top-performing`, {
      params: { limit: limit.toString() }
    }).pipe(
      map((courses) => courses.map((course) => ({
        id: course.id,
        title: course.title,
        category: course.category,
        instructor: course.instructorName,
        enrolledStudents: course.enrolledStudents,
        completionRate: course.completionRate,
        avgRating: course.averageRating,
        status: course.status,
        progressColor: this.getProgressColor(course.averageRating),
        sectionsCount: course.sectionsCount
      })))
    );
  }

  private getProgressColor(rating: number): string {
    if (rating >= 4.5) return '#10b981';
    if (rating >= 4.0) return '#4f46e5';
    if (rating >= 3.5) return '#f59e0b';
    return '#ef4444';
  }
}
