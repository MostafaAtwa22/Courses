import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { CourseProgress, CourseProgressSummary, MarkProgressRequest } from '../models/course.models';
import { Observable, of, throwError } from 'rxjs';
import { HttpHeaders } from '@angular/common/http';
import { catchError, map, switchMap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class ProgressService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/progress`;

  // Cache for enrolled course IDs
  private enrolledCourseIds = new Set<string>();
  private enrolledCoursesLoaded = false;

  getCourseProgress(courseId: string): Observable<CourseProgress> {
    return this.http.get<CourseProgress>(`${this.apiUrl}/course/${courseId}`);
  }

  // Load enrolled courses once and cache the IDs
  private loadEnrolledCourses(): Observable<void> {
    if (this.enrolledCoursesLoaded) {
      return of(void 0);
    }

    return this.getMyCoursesProgress().pipe(
      map(courses => {
        this.enrolledCourseIds.clear();
        courses.forEach(course => {
          this.enrolledCourseIds.add(course.courseId);
        });
        this.enrolledCoursesLoaded = true;
      }),
      catchError(() => {
        // If failed to load, assume no enrolled courses
        this.enrolledCourseIds.clear();
        this.enrolledCoursesLoaded = true;
        return of(void 0);
      })
    );
  }

  // Check enrollment - only calls API if course might be enrolled
  checkEnrollment(courseId: string): Observable<CourseProgress> {
    return this.loadEnrolledCourses().pipe(
      switchMap(() => {
        if (!this.enrolledCourseIds.has(courseId)) {
          // Not in enrolled courses, don't call API
          return throwError(() => new Error('Not enrolled'));
        }
        // Might be enrolled, make the API call to get progress data
        const headers = new HttpHeaders().set('X-Silent-Check', 'true');
        return this.http.get<CourseProgress>(`${this.apiUrl}/course/${courseId}`, { headers });
      })
    );
  }

  // Clear enrollment cache (call when user enrolls/unenrolls)
  clearEnrollmentCache(courseId?: string): void {
    if (courseId) {
      this.enrolledCourseIds.delete(courseId);
    } else {
      this.enrolledCourseIds.clear();
      this.enrolledCoursesLoaded = false;
    }
  }

  getMyCoursesProgress(): Observable<CourseProgressSummary[]> {
    return this.http.get<CourseProgressSummary[]>(`${this.apiUrl}/my-courses`);
  }

  markComplete(request: MarkProgressRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/complete`, request);
  }

  markIncomplete(request: MarkProgressRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/incomplete`, request);
  }
}
