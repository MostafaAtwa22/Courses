import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
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
  private router         = inject(Router);
  private apiUrl         = `${environment.apiUrl}/authentication`;

  get currentUser()             { return this.sessionService.currentUser; }
  getToken(): string | null     { return this.sessionService.getToken(); }
  isLoggedIn(): boolean         { return this.sessionService.isLoggedIn(); }
  clearSession(): void          { this.sessionService.clearSession(); }
  saveSession(token: string, user: BaseIdentityResponse): void {
    this.sessionService.saveSession(token, user);
  }
  getSelectedRole(): string | null { return this.sessionService.getSelectedRole(); }
  setSelectedRole(role: string): void { this.sessionService.setSelectedRole(role); }


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
        // Use setTimeout to ensure session is saved before role selection
        setTimeout(() => {
          this.handleRoleSelection();
        }, 0);
      }
    });
  }

  private decodeToken(token: string): any {
    const payload = token.split('.')[1];
    return JSON.parse(atob(payload));
  }

  hasInstructorProfile(): boolean {
    const token = this.getToken();
    if (!token) return false;
    
    const payload = this.decodeToken(token);
    return payload?.has_instructor_profile === 'true';
  }

  isInstructor(): boolean {
    const selectedRole = this.getSelectedRole();
    return selectedRole === 'Instructor';
  }

  isAdmin(): boolean {
    const selectedRole = this.getSelectedRole();
    return selectedRole === 'Admin' || selectedRole === 'SuperAdmin';
  }

  isStudent(): boolean {
    const selectedRole = this.getSelectedRole();
    return selectedRole === 'Student';
  }

  isInstructorOrAdmin(): boolean {
    return this.isInstructor() || this.isAdmin();
  }

  hasMultipleRoles(): boolean {
    const user = this.currentUser();
    if (!user) return false;
    return user.roles.length > 1;
  }

  private autoSelectRole(): void {
    const user = this.currentUser();
    if (!user) return;

    if (user.roles.length === 1) {
      this.setSelectedRole(user.roles[0]);
    }
  }

  handleRoleSelectionAfterLogin(): void {
    const user = this.currentUser();
    if (!user) return;

    console.log('handleRoleSelectionAfterLogin - User roles:', user.roles);
    console.log('handleRoleSelectionAfterLogin - Selected role:', this.getSelectedRole());

    if (user.roles.length === 1) {
      this.setSelectedRole(user.roles[0]);
      console.log('Auto-selected single role:', user.roles[0]);
    } else if (user.roles.length > 1) {
      const selectedRole = this.getSelectedRole();
      if (!selectedRole || !user.roles.includes(selectedRole)) {
        console.log('Redirecting to role selection page');
        // Use setTimeout to ensure the session is saved before navigation
        setTimeout(() => {
          this.router.navigate(['/auth/role-selection-after-login']);
        }, 100);
      } else {
        console.log('User already has valid selected role:', selectedRole);
      }
    }
  }

  private handleRoleSelection(): void {
    this.handleRoleSelectionAfterLogin();
  }
}
