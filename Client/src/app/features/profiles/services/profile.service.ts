import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { 
  UpdateProfileDto, 
  DeleteProfileDto, 
  ChangePasswordDto, 
  SetPasswordDto 
} from '../models/profile.models';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/profiles`;

  updateProfile(request: UpdateProfileDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/update-profile`, request);
  }

  deleteProfile(request: DeleteProfileDto): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/delete-profile`, { body: request });
  }

  updateProfileImage(image: File): Observable<void> {
    const formData = new FormData();
    formData.append('Image', image);
    return this.http.patch<void>(`${this.apiUrl}/update-image`, formData);
  }

  deleteProfileImage(): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/delete-image`);
  }

  changePassword(request: ChangePasswordDto): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/change-password`, request);
  }

  setPassword(request: SetPasswordDto): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/set-password`, request);
  }
}
