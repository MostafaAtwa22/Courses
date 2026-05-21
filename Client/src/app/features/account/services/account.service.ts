import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { PaginatedResultModel } from '../../../shared/models/paginated-result.model';
import { 
  UserResponseDto, 
  LockUserDto, 
  ForgetPasswordDto, 
  ResetPasswordDto, 
  UserQueryParams 
} from '../models/account.models';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/account`;

  getUsers(params: UserQueryParams): Observable<PaginatedResultModel<UserResponseDto>> {
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
    if (params.gender) {
      httpParams = httpParams.set('gender', params.gender);
    }
    if (params.role) {
      httpParams = httpParams.set('role', params.role);
    }

    return this.http
      .get<unknown>(`${this.apiUrl}/users`, { params: httpParams })
      .pipe(map((res) => PaginatedResultModel.fromApi<UserResponseDto>(res)));
  }

  getUserById(id: string): Observable<UserResponseDto> {
    return this.http.get<UserResponseDto>(`${this.apiUrl}/users/${id}`);
  }

  lockUser(id: string, request: LockUserDto): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/users/${id}/lock`, request);
  }

  unlockUser(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/users/${id}/unlock`, {});
  }

  forgetPassword(request: ForgetPasswordDto): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/forget-password`, request);
  }

  resetPassword(request: ResetPasswordDto): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/reset-password`, request);
  }
}
