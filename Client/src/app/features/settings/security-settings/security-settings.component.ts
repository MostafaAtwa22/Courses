import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SessionService } from '../../auth/services/session.service';
import { SecurityService } from '../../security/services/security.service';
import { BaseIdentityResponse } from '../../auth/models/auth.models';
import { Disable2FADto } from '../../security/models/security.models';
import { ToastService } from '../../../core/services/toast.service';
import { AlertService } from '../../../core/services/alert.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-security-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './security-settings.component.html',
  styleUrl: './security-settings.component.scss'
})
export class SecuritySettingsComponent implements OnInit {
  currentUser: BaseIdentityResponse | null = null;
  is2FAEnabled = false;
  isProcessing = false;

  constructor(
    private sessionService: SessionService,
    private securityService: SecurityService,
    private toastService: ToastService,
    private alertService: AlertService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.sessionService.currentUser();
    if (this.currentUser) {
      this.is2FAEnabled = this.currentUser.is2FAEnable;
    }
  }

  enable2FA(): void {
    this.alertService.confirm(
      'Enable Two-Factor Authentication?',
      'This will add an extra layer of security to your account. You will need an authenticator app to complete the setup.',
      'Enable',
      'Cancel'
    ).then((result) => {
      if (result.isConfirmed) {
        this.isProcessing = true;

        // First generate the 2FA token
        this.securityService.generate2FAToken().subscribe({
          next: () => {
            this.isProcessing = false;

            // Prompt for verification code
            this.alertService.custom({
              title: 'Enter Verification Code',
              html: `
                <div class="custom-swal-form-container">
                  <p class="custom-swal-form-text">Please enter the 6-digit verification code from your authenticator app:</p>
                  <input id="swal-code" class="swal2-input custom-swal-input" placeholder="6-digit code" type="text" maxlength="6">
                </div>
              `,
              customClass: {
                popup: 'custom-swal-popup',
                title: 'custom-swal-title',
                confirmButton: 'custom-swal-confirm',
                cancelButton: 'custom-swal-cancel'
              },
              focusConfirm: false,
              showCancelButton: true,
              confirmButtonText: 'Verify',
              cancelButtonText: 'Cancel',
              reverseButtons: true,
              preConfirm: () => {
                const code = (document.getElementById('swal-code') as HTMLInputElement).value;

                if (!code || code.length !== 6) {
                  Swal.showValidationMessage('Please enter a valid 6-digit code');
                }

                return code;
              }
            }).then((verifyResult) => {
              if (verifyResult.isConfirmed) {
                this.isProcessing = true;
                const code = verifyResult.value;

                // Call enable2FA with the verification code
                this.securityService.enable2FA(code).subscribe({
                  next: () => {
                    // Update session with new 2FA status
                    if (this.currentUser) {
                      const updatedUser = { ...this.currentUser, is2FAEnable: true };
                      const token = this.sessionService.getToken();
                      if (token) {
                        this.sessionService.saveSession(token, updatedUser);
                      }
                    }
                    this.is2FAEnabled = true;
                    this.isProcessing = false;
                    this.toastService.success('2FA enabled successfully!');
                  },
                  error: (error) => {
                    console.error('Error enabling 2FA:', error);
                    this.isProcessing = false;
                    this.toastService.error('Failed to enable 2FA. Please check your code and try again.');
                  }
                });
              }
            });
          },
          error: (error) => {
            console.error('Error generating 2FA token:', error);
            this.isProcessing = false;
            this.toastService.error('Failed to initiate 2FA setup. Please try again.');
          }
        });
      }
    });
  }

  openDisableModal(): void {
    this.alertService.confirm(
      'Disable Two-Factor Authentication?',
      'Are you sure you want to disable two-factor authentication? This will reduce your account security.',
      'Yes, Disable',
      'Cancel'
    ).then((result) => {
      if (result.isConfirmed) {
        this.isProcessing = true;

        // First generate the 2FA token (sends code to email)
        this.securityService.generate2FAToken().subscribe({
          next: () => {
            this.isProcessing = false;
            this.promptForDisableCode();
          },
          error: (error) => {
            console.error('Error generating disable 2FA token:', error);
            this.isProcessing = false;
            this.toastService.error('Failed to send verification code. Please try again.');
          }
        });
      }
    });
  }

  private promptForDisableCode(): void {
    this.alertService.custom({
      title: 'Disable Two-Factor Authentication',
      html: `
        <div class="custom-swal-form-container">
          <p class="custom-swal-form-text">A verification code has been sent to your email. Please enter your password and the code to disable 2FA:</p>
          <input id="swal-password" class="swal2-input custom-swal-input" placeholder="Password" type="password">
          <input id="swal-code" class="swal2-input custom-swal-input" placeholder="2FA Code" type="text" maxlength="6">
        </div>
      `,
      customClass: {
        popup: 'custom-swal-popup',
        title: 'custom-swal-title',
        confirmButton: 'custom-swal-deny',
        cancelButton: 'custom-swal-cancel'
      },
      focusConfirm: false,
      showCancelButton: true,
      confirmButtonText: 'Disable',
      cancelButtonText: 'Cancel',
      reverseButtons: true,
      preConfirm: () => {
        const password = (document.getElementById('swal-password') as HTMLInputElement).value;
        const code = (document.getElementById('swal-code') as HTMLInputElement).value;

        if (!password || !code) {
          Swal.showValidationMessage('Please enter both password and 2FA code');
        }

        return { password, code };
      }
    }).then((result) => {
      if (result.isConfirmed) {
        const disableRequest: Disable2FADto = {
          password: result.value.password,
          code: result.value.code
        };

        this.isProcessing = true;
        this.securityService.disable2FA(disableRequest).subscribe({
          next: () => {
            // Update session with new 2FA status
            if (this.currentUser) {
              const updatedUser = { ...this.currentUser, is2FAEnable: false };
              const token = this.sessionService.getToken();
              if (token) {
                this.sessionService.saveSession(token, updatedUser);
                this.currentUser = updatedUser;
              }
            }
            this.is2FAEnabled = false;
            this.isProcessing = false;
            this.toastService.success('2FA disabled successfully!');
          },
          error: (error) => {
            console.error('Error disabling 2FA:', error);
            this.isProcessing = false;
            this.toastService.error('Failed to disable 2FA. Please check your password and try again.');
          }
        });
      }
    });
  }
}
