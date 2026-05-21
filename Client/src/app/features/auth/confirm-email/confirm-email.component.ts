import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { SecurityService } from '../../security/services/security.service';

@Component({
  selector: 'app-confirm-email',
  standalone: true,
  imports: [CommonModule, RouterModule],
  styleUrl: './confirm-email.component.scss',
  templateUrl: './confirm-email.component.html'
})
export class ConfirmEmailComponent implements OnInit {
  private securityService = inject(SecurityService);
  private route = inject(ActivatedRoute);

  isLoading = true;
  isSuccess = false;
  errorMessage = '';

  ngOnInit() {
    const email = this.route.snapshot.queryParams['email'];
    const code = this.route.snapshot.queryParams['code']; // Note: The backend expects 'code' or 'token'

    if (!email || !code) {
      this.isLoading = false;
      this.isSuccess = false;
      this.errorMessage = 'Invalid confirmation link. Missing parameters.';
      return;
    }

    this.securityService.confirmEmail({ email, code }).subscribe({
      next: () => {
        this.isLoading = false;
        this.isSuccess = true;
      },
      error: (err) => {
        this.isLoading = false;
        this.isSuccess = false;
        this.errorMessage = err.error?.detail || 'Failed to confirm email. The link might be expired or invalid.';
      }
    });
  }
}
