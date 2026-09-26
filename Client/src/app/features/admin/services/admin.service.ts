import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { AdminResponse, AdminQueryParams, AdminCreateDto } from '../models/admin.models';
import { PaginatedResultModel } from '../../../shared/models/paginated-result.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/admins`;

  getAllAdmins(params: AdminQueryParams): Observable<PaginatedResultModel<AdminResponse>> {
    return this.http.get<PaginatedResultModel<AdminResponse>>(this.apiUrl, { params: params as any });
  }

  getAdminById(id: string): Observable<AdminResponse> {
    return this.http.get<AdminResponse>(`${this.apiUrl}/${id}`);
  }

  deleteAdmin(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  createAdmin(dto: AdminCreateDto): Observable<void> {
    return this.http.post<void>(this.apiUrl, dto);
  }
}