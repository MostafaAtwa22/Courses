import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { CourseProgress, CourseProgressSummary, MarkProgressRequest } from '../models/course.models';
import { Observable } from 'rxjs';
import { HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class ProgressService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/progress`;

  getCourseProgress(courseId: string): Observable<CourseProgress> {
    return this.http.get<CourseProgress>(`${this.apiUrl}/course/${courseId}`);
  }

  // Silent enrollment check - bypasses error interceptor
  checkEnrollment(courseId: string): Observable<CourseProgress> {
    const headers = new HttpHeaders().set('X-Silent-Check', 'true');
    return this.http.get<CourseProgress>(`${this.apiUrl}/course/${courseId}`, { headers });
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
