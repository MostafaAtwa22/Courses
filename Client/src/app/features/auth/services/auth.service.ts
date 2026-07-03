import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { LoginDto, RegisterDto, AuthResponseDto, BaseIdentityResponse } from '../models/auth.models';
import { FacebookLoginDto, GoogleLoginDto, GithubLoginDto } from '../models/external-login.models';
import { SessionService } from './session.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http           = inject(HttpClient);
  private sessionService = inject(SessionService);
  private apiUrl         = `${environment.apiUrl}/authentication`;

  get currentUser()             { return this.sessionService.currentUser; }
  getToken(): string | null     { return this.sessionService.getToken(); }
  isLoggedIn(): boolean         { return this.sessionService.isLoggedIn(); }
  clearSession(): void          { this.sessionService.clearSession(); }
  saveSession(token: string, user: BaseIdentityResponse): void {
    this.sessionService.saveSession(token, user);
  }


  register(request: RegisterDto): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/register`, request);
  }

  login(request: LoginDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/login`, request, { withCredentials: true }).pipe(
      this.saveOnSuccess()
    );
  }

  googleLogin(request: GoogleLoginDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/google-login`, request, { withCredentials: true }).pipe(
      this.saveOnSuccess()
    );
  }

  facebookLogin(request: FacebookLoginDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/facebook-login`, request, { withCredentials: true }).pipe(
      this.saveOnSuccess()
    );
  }

  githubLogin(request: GithubLoginDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/github-login`, request, { withCredentials: true }).pipe(
      this.saveOnSuccess()
    );
  }

  refreshToken(): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/refresh-token`, {}, { withCredentials: true }).pipe(
      this.saveOnSuccess()
    );
  }

  logout(): void {
    this.http.post(`${this.apiUrl}/revoke-token`, {}, { withCredentials: true }).subscribe({
      next:  () => this.sessionService.clearSession(),
      error: () => this.sessionService.clearSession()
    });
  }

  private saveOnSuccess() {
    return tap<AuthResponseDto>(response => {
      if (response.token) {
        this.sessionService.saveSession(response.token, response);
      }
    });
  }
}
