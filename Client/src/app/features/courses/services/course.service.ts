import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { CourseResponse } from '../models/course.models';
import { PaginatedResultModel } from '../../../shared/models/paginated-result.model';
import { QueryParams } from '../../../shared/models/query-params.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CourseService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/courses`;

  getAll(params: QueryParams): Observable<PaginatedResultModel<CourseResponse>> {
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
    if (params.sortBy) {
      httpParams = httpParams.set('sortBy', params.sortBy);
    }
    if (params.sortDescending !== undefined) {
      httpParams = httpParams.set('sortDescending', params.sortDescending.toString());
    }

    return this.http.get<PaginatedResultModel<CourseResponse>>(this.apiUrl, {
      params: httpParams,
    });
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
