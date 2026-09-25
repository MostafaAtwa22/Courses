import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { InstructorPrivateResponse, InstructorPublicResponse } from '../models/instructor.models';
import { PaginatedResultModel } from '../../../shared/models/paginated-result.model';
import { InstructorQueryParams } from '../../../shared/models/query-params.model';
import { Observable } from 'rxjs';
import { map, tap, catchError } from 'rxjs/operators';
import { of } from 'rxjs';

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

  getCurrentInstructor(): Observable<InstructorPrivateResponse> {
    return this.http.get<InstructorPrivateResponse>(`${this.apiUrl}/private/me`);
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
    console.log('getAllInstructors - Calling API with params:', params);
    console.log('getAllInstructors - URL:', `${this.apiUrl}/admin/all`);
    
    // Remove status parameter from API call since backend doesn't handle it correctly
    // We'll filter on frontend instead
    const { status, ...apiParams } = params;
    console.log('getAllInstructors - API params (without status):', apiParams);
    
    return this.http.get<PaginatedResultModel<InstructorPrivateResponse>>(`${this.apiUrl}/admin/all`, { params: apiParams as any }).pipe(
      tap((response) => {
        console.log('getAllInstructors - API Response:', response);
        console.log('getAllInstructors - Total items:', response.items?.length);
        console.log('getAllInstructors - Total count:', response.totalCount);
        
        // Check if status filtering is requested
        if (status && response.items && response.items.length > 0) {
          const filteredItems = response.items.filter(item => item.status === status);
          console.log('getAllInstructors - Requested status filter:', status);
          console.log('getAllInstructors - Frontend filtered items:', filteredItems.length);
          console.log('getAllInstructors - Item statuses:', response.items.map(item => ({ id: item.id, status: item.status })));
        }
      }),
      map((response) => {
        // Apply frontend filtering as workaround since backend doesn't handle status parameter
        if (status && response.items) {
          const filteredItems = response.items.filter(item => item.status === status);
          console.log('getAllInstructors - Applying frontend filter for status:', status);
          console.log('getAllInstructors - Original count:', response.items.length, 'Filtered count:', filteredItems.length);
          
          return {
            ...response,
            items: filteredItems,
            totalCount: filteredItems.length,
            totalPages: Math.ceil(filteredItems.length / (params.pageSize || 10))
          };
        }
        return response;
      }),
      catchError((error) => {
        console.error('getAllInstructors - API Error:', error);
        console.error('getAllInstructors - Error status:', error.status);
        console.error('getAllInstructors - Error message:', error.message);
        return of(new PaginatedResultModel<InstructorPrivateResponse>());
      })
    );
  }

  changeInstructorStatus(instructorId: string, status: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/admin/${instructorId}/status`, { status });
  }
}
