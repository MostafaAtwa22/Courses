import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';

export interface RoleResponseDto {
  id: string;
  name: string;
}

export interface PermissionCheckboxDto {
  name: string;
  module: string;
  action: string;
  isSelected: boolean;
}

export interface PermissionRoleDto {
  roleId: string;
  roleName: string;
  permissions: PermissionCheckboxDto[];
}

@Injectable({
  providedIn: 'root'
})
export class AuthorizationService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/authorization`;

  getRoles(): Observable<RoleResponseDto[]> {
    return this.http.get<RoleResponseDto[]>(`${this.apiUrl}/roles`);
  }

  getRolePermissions(roleId: string): Observable<PermissionRoleDto> {
    return this.http.get<PermissionRoleDto>(`${this.apiUrl}/roles/${roleId}/permissions`);
  }

  updateRolePermissions(roleId: string, permissions: PermissionCheckboxDto[]): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/roles/${roleId}/permissions`, permissions);
  }
}
