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

  getBySection(sectionId: string, courseId: string): Observable<ContentResponse[]> {
    return this.http.get<ContentResponse[]>(`${this.apiUrl}/section/${sectionId}/${courseId}`);
  }

  getByCourse(courseId: string, params: QueryParams): Observable<PaginatedResultModel<ContentResponse>> {
    return this.http
      .get<unknown>(`${this.apiUrl}/course/${courseId}`, { params: this.buildHttpParams(params) })
      .pipe(map((res) => PaginatedResultModel.fromApi<ContentResponse>(res)));
  }

  getById(id: string, courseId: string): Observable<ContentResponse> {
    return this.http.get<ContentResponse>(`${this.apiUrl}/${id}/${courseId}`);
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
    formData.append('order', request.order.toString());
    formData.append('isPreview', request.isPreview.toString());
    formData.append('sectionId', request.sectionId);

    if ('courseId' in request) {
      formData.append('courseId', request.courseId);
    }

    if ('videoFile' in request && request.videoFile) {
      formData.append('videoFile', request.videoFile);
    }

    if ('attachments' in request && request.attachments) {
      request.attachments.forEach((file) => {
        formData.append('attachments', file);
      });
    }

    if ('attachmentsToAdd' in request && request.attachmentsToAdd) {
      request.attachmentsToAdd.forEach((file) => {
        formData.append('attachmentsToAdd', file);
      });
    }

    if ('attachmentIdsToRemove' in request && request.attachmentIdsToRemove) {
      request.attachmentIdsToRemove.forEach((id) => {
        formData.append('attachmentIdsToRemove', id);
      });
    }

    return formData;
  }
}
