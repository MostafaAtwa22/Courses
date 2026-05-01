import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { CourseResponse } from '../models/course.models';
import { PaginatedResultModel } from '../../../shared/models/paginated-result.model';
import { CourseQueryParams } from '../../../shared/models/query-params.model';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CourseService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/courses`;

  getAll(params: CourseQueryParams): Observable<PaginatedResultModel<CourseResponse>> {
    let httpParams = new HttpParams();

    if (params.pageNumber) {
      httpParams = httpParams.set('pageNumber', params.pageNumber.toString());
    }
    if (params.pageSize) {
      httpParams = httpParams.set('pageSize', params.pageSize.toString());
    }
    if (params.searchTerm) {
      httpParams = httpParams.set('searchTerm', params.searchTerm);
    }
    if (params.category) {
      httpParams = httpParams.set('category', params.category);
    }
    if (params.sortBy) {
      httpParams = httpParams.set('sortBy', params.sortBy);
    }
    if (params.sortDescending !== undefined) {
      httpParams = httpParams.set('sortDescending', params.sortDescending.toString());
    }
    if (params.minRating !== undefined) {
      httpParams = httpParams.set('minRating', params.minRating.toString());
    }
    if (params.maxRating !== undefined) {
      httpParams = httpParams.set('maxRating', params.maxRating.toString());
    }

    return this.http
      .get<unknown>(this.apiUrl, { params: httpParams })
      .pipe(map((res) => PaginatedResultModel.fromApi<CourseResponse>(res)));
  }

  getById(id: string): Observable<CourseResponse> {
    return this.http.get<CourseResponse>(`${this.apiUrl}/${id}`);
  }

  create(course: FormData): Observable<string> {
    return this.http.post<string>(this.apiUrl, course);
  }

  update(id: string, course: FormData): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, course);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
