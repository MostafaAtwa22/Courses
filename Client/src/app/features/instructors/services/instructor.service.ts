import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { InstructorPrivateResponse, InstructorPublicResponse } from '../models/instructor.models';
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

  updateInstructor(id: string, formData: FormData): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, formData);
  }
}
