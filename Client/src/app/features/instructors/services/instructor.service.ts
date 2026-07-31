import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { InstructorPrivateResponse, InstructorPublicResponse } from '../models/instructor.models';
import { PaginatedResultModel } from '../../../shared/models/paginated-result.model';
import { InstructorQueryParams } from '../../../shared/models/query-params.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class InstructorService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/instructors`;
  
  createInstructor(formData: FormData): Observable<string> {
    return this.http.post<string>(this.apiUrl, formData);
  }

  getInstructorById(id: string): Observable<InstructorPrivateResponse> {
    return this.http.get<InstructorPrivateResponse>(`${this.apiUrl}/private/${id}`);
  }

  getPublicInstructorById(id: string): Observable<InstructorPublicResponse> {
    return this.http.get<InstructorPublicResponse>(`${this.apiUrl}/public/${id}`);
  }

  getPublicInstructorByCourseId(courseId: string): Observable<InstructorPublicResponse> {
    return this.http.get<InstructorPublicResponse>(`${this.apiUrl}/public/by-course/${courseId}`);
  }

  updateInstructor(id: string, formData: FormData): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, formData);
  }

  getAllInstructors(params: InstructorQueryParams): Observable<PaginatedResultModel<InstructorPrivateResponse>> {
    return this.http.get<PaginatedResultModel<InstructorPrivateResponse>>(`${this.apiUrl}/admin/all`, { params: params as any });
  }

  changeInstructorStatus(instructorId: string, status: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/admin/${instructorId}/status`, { status });
  }
}
