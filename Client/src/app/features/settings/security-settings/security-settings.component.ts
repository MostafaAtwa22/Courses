import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SessionService } from '../../auth/services/session.service';
import { SecurityService } from '../../security/services/security.service';
import { BaseIdentityResponse } from '../../auth/models/auth.models';
import { Disable2FADto } from '../../security/models/security.models';

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
  showDisableModal = false;
  disablePassword = '';
  disableCode = '';

  constructor(
    private sessionService: SessionService,
    private securityService: SecurityService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.sessionService.currentUser();
    if (this.currentUser) {
      this.is2FAEnabled = this.currentUser.is2FAEnable;
    }
  }

  enable2FA(): void {
    this.isProcessing = true;
    
    // First generate the 2FA token
    this.securityService.generate2FAToken().subscribe({
      next: () => {
        // After generating, the user would typically need to verify with a code
        // For now, we'll show a message that they need to complete the setup
        alert('2FA setup initiated. Please complete the setup process with your authenticator app.');
        this.isProcessing = false;
      },
      error: (error) => {
        console.error('Error enabling 2FA:', error);
        this.isProcessing = false;
      }
    });
  }

  openDisableModal(): void {
    this.showDisableModal = true;
    this.disablePassword = '';
    this.disableCode = '';
  }

  closeDisableModal(): void {
    this.showDisableModal = false;
    this.disablePassword = '';
    this.disableCode = '';
  }

  confirmDisable2FA(): void {
    if (!this.disablePassword || !this.disableCode) {
      alert('Please enter your password and 2FA code to disable 2FA');
      return;
    }

    this.isProcessing = true;
    const disableRequest: Disable2FADto = {
      password: this.disablePassword,
      code: this.disableCode
    };

    this.securityService.disable2FA(disableRequest).subscribe({
      next: () => {
        // Update session with new 2FA status
        if (this.currentUser) {
          const updatedUser = { ...this.currentUser, is2FAEnable: false };
          const token = this.sessionService.getToken();
          if (token) {
            this.sessionService.saveSession(token, updatedUser);
          }
        }
        this.is2FAEnabled = false;
        this.isProcessing = false;
        this.closeDisableModal();
      },
      error: (error) => {
        console.error('Error disabling 2FA:', error);
        this.isProcessing = false;
        alert('Failed to disable 2FA. Please check your password and try again.');
      }
    });
  }
}
