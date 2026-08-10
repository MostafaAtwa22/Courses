import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { StudentResponse, StudentQueryParams } from '../models/student.models';
import { PaginatedResultModel } from '../../../shared/models/paginated-result.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class StudentService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/students`;

  getAllStudents(params: StudentQueryParams): Observable<PaginatedResultModel<StudentResponse>> {
    return this.http.get<PaginatedResultModel<StudentResponse>>(this.apiUrl, { params: params as any });
  }

  getStudentById(id: string): Observable<StudentResponse> {
    return this.http.get<StudentResponse>(`${this.apiUrl}/${id}`);
  }

  getStudentByUserId(userId: string): Observable<StudentResponse> {
    return this.http.get<StudentResponse>(`${this.apiUrl}/by-user/${userId}`);
  }

  deleteStudent(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
