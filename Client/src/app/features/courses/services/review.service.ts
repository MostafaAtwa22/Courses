import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { ReviewResponse, ReviewCreateRequest } from '../models/course.models';
import { PaginatedResultModel } from '../../../shared/models/paginated-result.model';
import { QueryParams } from '../../../shared/models/query-params.model';
import { catchError, map, Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ReviewService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/reviews`;

  getByCourseId(courseId: string, params: QueryParams): Observable<PaginatedResultModel<ReviewResponse>> {
    let httpParams = new HttpParams();

    if (params.pageNumber) {
      httpParams = httpParams.set('pageNumber', params.pageNumber.toString());
    }
    if (params.pageSize) {
      httpParams = httpParams.set('pageSize', params.pageSize.toString());
    }
    if (params.sortBy) {
      httpParams = httpParams.set('sortBy', params.sortBy);
    }
    if (params.sortDescending !== undefined) {
      httpParams = httpParams.set('sortDescending', params.sortDescending.toString());
    }

    return this.http
      .get<unknown>(`${this.apiUrl}/course/${courseId}`, { params: httpParams })
      .pipe(map((res) => PaginatedResultModel.fromApi<ReviewResponse>(res)));
  }

  getById(id: string): Observable<ReviewResponse> {
    return this.http.get<ReviewResponse>(`${this.apiUrl}/${id}`);
  }

  hasUserReviewed(courseId: string): Observable<boolean> {
    return this.http.get<ReviewResponse>(`${this.apiUrl}/course/${courseId}/user-review`).pipe(
      map(() => true),
      catchError((error: any) => {
        if (error.status === 404) return of(false);
        throw error;
      })
    );
  }

  getUserReview(courseId: string): Observable<ReviewResponse | null> {
    return this.http.get<ReviewResponse>(`${this.apiUrl}/course/${courseId}/user-review`).pipe(
      catchError((error: any) => {
        if (error.status === 404) return of(null);
        if (error.status === 401) return of(null); // Handle unauthorized errors gracefully
        throw error;
      })
    );
  }

  createReview(request: ReviewCreateRequest): Observable<string> {
    return this.http.post<{ id: string }>(`${this.apiUrl}`, request).pipe(
      map(response => response.id)
    );
  }

  updateReview(id: string, request: import('../models/course.models').ReviewUpdateRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, request);
  }

  deleteReview(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
