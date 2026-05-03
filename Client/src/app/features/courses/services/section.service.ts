import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { SectionCreateRequest, SectionResponse, SectionUpdateRequest } from '../models/course.models';
import { PaginatedResultModel } from '../../../shared/models/paginated-result.model';
import { QueryParams } from '../../../shared/models/query-params.model';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SectionService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/sections`;

  private buildHttpParams(params: QueryParams): HttpParams {
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

    return httpParams;
  }

  getAll(params: QueryParams): Observable<PaginatedResultModel<SectionResponse>> {
    return this.http
      .get<unknown>(this.apiUrl, { params: this.buildHttpParams(params) })
      .pipe(map((res) => PaginatedResultModel.fromApi<SectionResponse>(res)));
  }

  getByCourseId(courseId: string, params: QueryParams): Observable<PaginatedResultModel<SectionResponse>> {
    return this.http
      .get<unknown>(`${this.apiUrl}/course/${courseId}`, { params: this.buildHttpParams(params) })
      .pipe(map((res) => PaginatedResultModel.fromApi<SectionResponse>(res)));
  }

  getById(id: string): Observable<SectionResponse> {
    return this.http.get<SectionResponse>(`${this.apiUrl}/${id}`);
  }

  create(request: SectionCreateRequest): Observable<SectionResponse> {
    return this.http.post<SectionResponse>(this.apiUrl, request);
  }

  update(id: string, request: SectionUpdateRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
