import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { UserResponse, UserRolesManage, RolesResponse, LockUserDto } from '../models/user.models';
import { PaginatedResultModel } from '../../../shared/models/paginated-result.model';
import { QueryParams, UserQueryParams } from '../../../shared/models/query-params.model';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private http = inject(HttpClient);
  private accountApiUrl = `${environment.apiUrl}/account`;
  private authApiUrl = `${environment.apiUrl}/authorization`;

  getAll(params: QueryParams | UserQueryParams): Observable<PaginatedResultModel<UserResponse>> {
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
    if ('role' in params && params.role) {
      httpParams = httpParams.set('role', params.role);
    }

    return this.http
      .get<unknown>(`${this.accountApiUrl}/users`, { params: httpParams })
      .pipe(map((res) => PaginatedResultModel.fromApi<UserResponse>(res)));
  }

  getById(id: string): Observable<UserResponse> {
    return this.http.get<UserResponse>(`${this.accountApiUrl}/users/${id}`);
  }

  lockUser(id: string, dto: LockUserDto): Observable<void> {
    return this.http.post<void>(`${this.accountApiUrl}/users/${id}/lock`, dto);
  }

  unlockUser(id: string): Observable<void> {
    return this.http.post<void>(`${this.accountApiUrl}/users/${id}/unlock`, {});
  }

  getAllRoles(): Observable<RolesResponse[]> {
    return this.http.get<RolesResponse[]>(`${this.authApiUrl}/roles`);
  }

  getUserRoles(userId: string): Observable<UserRolesManage> {
    return this.http.get<UserRolesManage>(`${this.authApiUrl}/users/${userId}/roles`);
  }

  updateUserRoles(userId: string, dto: UserRolesManage): Observable<void> {
    return this.http.patch<void>(`${this.authApiUrl}/users/${userId}/roles`, dto);
  }
}
