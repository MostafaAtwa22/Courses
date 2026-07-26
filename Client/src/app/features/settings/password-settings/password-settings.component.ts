import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ProfileService } from '../../profiles/services/profile.service';
import { SessionService } from '../../auth/services/session.service';
import { ChangePasswordDto, SetPasswordDto } from '../../profiles/models/profile.models';

@Component({
  selector: 'app-password-settings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './password-settings.component.html',
  styleUrl: './password-settings.component.scss'
})
export class PasswordSettingsComponent implements OnInit {
  changePasswordForm: FormGroup;
  setPasswordForm: FormGroup;
  isChangingPassword = false;
  isSettingPassword = false;
  hasPassword = true; // This should be determined from user data

  constructor(private fb: FormBuilder, private profileService: ProfileService, private sessionService: SessionService) {
    this.changePasswordForm = this.fb.group({
      oldPassword: ['', [Validators.required]],
      newPassword: ['', [Validators.required, Validators.minLength(8)]],
      confirmNewPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });

    this.setPasswordForm = this.fb.group({
      newPassword: ['', [Validators.required, Validators.minLength(8)]],
      confirmNewPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  ngOnInit(): void {
    const user = this.sessionService.currentUser();
    this.hasPassword = user?.hasPassword ?? false;
  }

  passwordMatchValidator(form: FormGroup): { [key: string]: boolean } | null {
    const newPassword = form.get('newPassword')?.value;
    const confirmNewPassword = form.get('confirmNewPassword')?.value;
    
    if (newPassword !== confirmNewPassword) {
      return { passwordMismatch: true };
    }
    return null;
  }

  onChangePassword(): void {
    if (this.changePasswordForm.invalid) {
      this.changePasswordForm.markAllAsTouched();
      return;
    }

    this.isChangingPassword = true;
    const changePasswordData: ChangePasswordDto = this.changePasswordForm.value;

    this.profileService.changePassword(changePasswordData).subscribe({
      next: () => {
        this.isChangingPassword = false;
        this.changePasswordForm.reset();
        alert('Password changed successfully!');
      },
      error: (error) => {
        console.error('Error changing password:', error);
        this.isChangingPassword = false;
        alert('Failed to change password. Please check your old password and try again.');
      }
    });
  }

  onSetPassword(): void {
    if (this.setPasswordForm.invalid) {
      this.setPasswordForm.markAllAsTouched();
      return;
    }

    this.isSettingPassword = true;
    const setPasswordData: SetPasswordDto = this.setPasswordForm.value;

    this.profileService.setPassword(setPasswordData).subscribe({
      next: () => {
        this.isSettingPassword = false;
        this.setPasswordForm.reset();
        this.hasPassword = true;
        
        // Update session
        const currentUser = this.sessionService.currentUser();
        const token = this.sessionService.getToken();
        if (currentUser && token) {
          const updatedUser = { ...currentUser, hasPassword: true };
          this.sessionService.saveSession(token, updatedUser);
        }
        
        alert('Password set successfully!');
      },
      error: (error) => {
        console.error('Error setting password:', error);
        this.isSettingPassword = false;
        alert('Failed to set password. Please try again.');
      }
    });
  }

  get changePasswordError(): string {
    if (this.changePasswordForm.hasError('passwordMismatch')) {
      return 'New passwords do not match';
    }
    return '';
  }

  get setPasswordError(): string {
    if (this.setPasswordForm.hasError('passwordMismatch')) {
      return 'Passwords do not match';
    }
    return '';
  }
}
