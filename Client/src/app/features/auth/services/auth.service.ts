import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { LoginDto, RegisterDto, AuthResponseDto, BaseIdentityResponse } from '../models/auth.models';
import { GoogleLoginDto } from '../models/external-login.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/authentication`;

  currentUser = signal<BaseIdentityResponse | null>(this.getSavedUser());

  register(request: RegisterDto): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/register`, request);
  }

  login(request: LoginDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/login`, request, { withCredentials: true }).pipe(
      tap(response => {
        if (response.token) {
          this.saveSession(response.token, response);
        }
      })
    );
  }

  googleLogin(request: GoogleLoginDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/google-login`, request, { withCredentials: true }).pipe(
      tap(response => {
        if (response.token) {
          this.saveSession(response.token, response);
        }
      })
    );
  }
  
  refreshToken(): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/refresh-token`, {}, { withCredentials: true }).pipe(
      tap(response => {
        if (response.token) {
          this.saveSession(response.token, response);
        }
      })
    );
  }

  saveSession(token: string, user: BaseIdentityResponse): void {
    localStorage.setItem('EduFocus_token', token);
    localStorage.setItem('EduFocus_user', JSON.stringify(user));
    this.currentUser.set(user);
  }

  getToken(): string | null {
    return localStorage.getItem('EduFocus_token');
  }

  logout(): void {
    this.http.post(`${this.apiUrl}/revoke-token`, {}, { withCredentials: true }).subscribe({
      next: () => this.clearSession(),
      error: () => this.clearSession()
    });
  }

  clearSession(): void {
    localStorage.removeItem('EduFocus_token');
    localStorage.removeItem('EduFocus_user');
    this.currentUser.set(null);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  private getSavedUser(): BaseIdentityResponse | null {
    const userJson = localStorage.getItem('EduFocus_user');
    if (!userJson) return null;
    try {
      return JSON.parse(userJson) as BaseIdentityResponse;
    } catch {
      return null;
    }
  }
}
