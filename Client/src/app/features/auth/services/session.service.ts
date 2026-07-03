import { Injectable, signal } from '@angular/core';
import { BaseIdentityResponse } from '../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class SessionService {
  private readonly TOKEN_KEY = 'EduFocus_token';
  private readonly USER_KEY = 'EduFocus_user';

  currentUser = signal<BaseIdentityResponse | null>(this.getSavedUser());

  saveSession(token: string, user: BaseIdentityResponse): void {
    localStorage.setItem(this.TOKEN_KEY, token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    this.currentUser.set(user);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  clearSession(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUser.set(null);
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
}
