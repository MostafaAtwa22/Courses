import { Injectable, signal } from '@angular/core';
import { BaseIdentityResponse } from '../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class SessionService {
  private readonly TOKEN_KEY = 'EduFocus_token';
  private readonly USER_KEY = 'EduFocus_user';
  private readonly SELECTED_ROLE_KEY = 'EduFocus_selected_role';

  currentUser = signal<BaseIdentityResponse | null>(this.getSavedUser());
  selectedRole = signal<string | null>(this.getSelectedRole());

  saveSession(token: string, user: BaseIdentityResponse): void {
    console.log('saveSession called with user:', user);
    localStorage.setItem(this.TOKEN_KEY, token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    this.currentUser.set(user);
    console.log('Session saved, current user roles:', user.roles);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  clearSession(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    localStorage.removeItem(this.SELECTED_ROLE_KEY);
    this.currentUser.set(null);
    this.selectedRole.set(null);
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
}
