import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AuthResponseDto } from '../../auth/models/auth.models';
import { Disable2FADto, VerifyTwoFactorDto, ConfirmEmailDto } from '../models/security.models';

@Injectable({
  providedIn: 'root'
})
export class SecurityService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/security`;

  generate2FAToken(): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/2fa/generate`, {});
  }

  enable2FA(code: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/2fa/enable`, JSON.stringify(code), {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  disable2FA(request: Disable2FADto): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/2fa/disable`, request);
  }

  verifyTwoFactor(request: VerifyTwoFactorDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/2fa/verify`, request);
  }

  confirmEmail(request: ConfirmEmailDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/confirm-email`, request);
  }
}
