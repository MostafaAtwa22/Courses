import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { ReviewResponse } from '../models/course.models';
import { PaginatedResultModel } from '../../../shared/models/paginated-result.model';
import { QueryParams } from '../../../shared/models/query-params.model';
import { map, Observable } from 'rxjs';

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
}
