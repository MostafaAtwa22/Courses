import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { SecurityService } from '../../security/services/security.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-two-factor',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  styleUrl: './two-factor.component.scss',
  templateUrl: './two-factor.component.html'
})
export class TwoFactorComponent implements OnInit {
  private fb = inject(FormBuilder);
  private securityService = inject(SecurityService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  email = '';
  isLoading = false;
  errorMessage = '';

  codeForm = this.fb.nonNullable.group({
    code: ['', [Validators.required, Validators.minLength(6)]]
  });

  ngOnInit() {
    this.email = this.route.snapshot.queryParams['email'] || '';
    if (!this.email) {
      this.router.navigate(['/auth/login']);
    }
  }

  onSubmit() {
    if (this.codeForm.invalid) return;

    this.isLoading = true;
    this.errorMessage = '';

    const request = {
      email: this.email,
      code: this.codeForm.value.code!
    };

    this.securityService.verifyTwoFactor(request).subscribe({
      next: (response) => {
        if (response.token) {
          this.authService.saveSession(response.token, response);
          this.router.navigate(['/']);
        }
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.detail || 'Verification failed. Invalid code.';
      }
    });
  }
}
