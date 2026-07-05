import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CourseDiscountResponse, CreateCourseDiscountRequest } from '../models/course.models';

export interface UpdateCourseDiscountRequest {
  percentage: number;
  startTime: string;
  endTime: string;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class DiscountService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/discounts`;

  addDiscount(courseId: string, discount: CreateCourseDiscountRequest): Observable<string> {
    return this.http.post<string>(`${this.apiUrl}/${courseId}`, discount);
  }

  getDiscounts(courseId: string): Observable<CourseDiscountResponse[]> {
    return this.http.get<CourseDiscountResponse[]>(`${this.apiUrl}/${courseId}`);
  }

  updateDiscount(id: string, request: UpdateCourseDiscountRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, request);
  }

  deleteDiscount(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
