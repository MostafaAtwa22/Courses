import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { ContentCreateRequest, ContentResponse, ContentUpdateRequest } from '../models/course.models';
import { map, Observable } from 'rxjs';
import { QueryParams } from '../../../shared/models/query-params.model';
import { PaginatedResultModel } from '../../../shared/models/paginated-result.model';

@Injectable({
  providedIn: 'root',
})
export class ContentService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/contents`;

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

  getBySection(sectionId: string): Observable<ContentResponse[]> {
    return this.http.get<ContentResponse[]>(`${this.apiUrl}/section/${sectionId}`);
  }

  getByCourse(courseId: string, params: QueryParams): Observable<PaginatedResultModel<ContentResponse>> {
    return this.http
      .get<unknown>(`${this.apiUrl}/course/${courseId}`, { params: this.buildHttpParams(params) })
      .pipe(map((res) => PaginatedResultModel.fromApi<ContentResponse>(res)));
  }

  getById(id: string): Observable<ContentResponse> {
    return this.http.get<ContentResponse>(`${this.apiUrl}/${id}`);
  }

  create(request: ContentCreateRequest): Observable<ContentResponse> {
    return this.http.post<ContentResponse>(this.apiUrl, this.toFormData(request));
  }

  update(id: string, request: ContentUpdateRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, this.toFormData(request));
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  private toFormData(request: ContentCreateRequest | ContentUpdateRequest): FormData {
    const formData = new FormData();
    formData.append('title', request.title);
    formData.append('type', request.type.toString());
    formData.append('order', request.order.toString());
    formData.append('isPreview', request.isPreview.toString());
    formData.append('sectionId', request.sectionId);

    if (request.file) {
      formData.append('file', request.file);
    }

    return formData;
  }
}
