import { Injectable, signal } from '@angular/core';
import { BaseIdentityResponse } from '../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class SessionService {
  private readonly TOKEN_KEY = 'EduFocus_token';
  private readonly USER_KEY = 'EduFocus_user';
  private readonly SELECTED_ROLE_KEY = 'EduFocus_selected_role';
  private readonly PERMISSIONS_KEY = 'EduFocus_permissions';

  currentUser = signal<BaseIdentityResponse | null>(this.getSavedUser());
  selectedRole = signal<string | null>(this.getSelectedRole());
  permissions = signal<string[]>(this.getSavedPermissions());

  saveSession(token: string, user: BaseIdentityResponse): void {
    console.log('saveSession called with user:', user);
    localStorage.setItem(this.TOKEN_KEY, token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    localStorage.setItem(this.PERMISSIONS_KEY, JSON.stringify(user.permissions || []));
    this.currentUser.set(user);
    this.permissions.set(user.permissions || []);
    console.log('Session saved, current user roles:', user.roles);
    console.log('Session saved, current user permissions:', user.permissions);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  clearSession(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    localStorage.removeItem(this.SELECTED_ROLE_KEY);
    localStorage.removeItem(this.PERMISSIONS_KEY);
    this.currentUser.set(null);
    this.selectedRole.set(null);
    this.permissions.set([]);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  private getSavedUser(): BaseIdentityResponse | null {
    const userJson = localStorage.getItem(this.USER_KEY);
    if (!userJson) return null;
    try {
      return JSON.parse(userJson) as BaseIdentityResponse;
    } catch {
      return null;
    }
  }

  private getSavedPermissions(): string[] {
    const permissionsJson = localStorage.getItem(this.PERMISSIONS_KEY);
    if (!permissionsJson) return [];
    try {
      return JSON.parse(permissionsJson) as string[];
    } catch {
      return [];
    }
  }

  getSelectedRole(): string | null {
    return localStorage.getItem(this.SELECTED_ROLE_KEY);
  }

  setSelectedRole(role: string): void {
    console.log('setSelectedRole called with:', role);
    localStorage.setItem(this.SELECTED_ROLE_KEY, role);
    this.selectedRole.set(role);
    console.log('Selected role saved to localStorage:', localStorage.getItem(this.SELECTED_ROLE_KEY));
  }

  clearSelectedRole(): void {
    localStorage.removeItem(this.SELECTED_ROLE_KEY);
    this.selectedRole.set(null);
  }

  hasPermission(permission: string): boolean {
    return this.permissions().includes(permission);
  }

  hasAnyPermission(permissions: string[]): boolean {
    return permissions.some(permission => this.permissions().includes(permission));
  }
}
