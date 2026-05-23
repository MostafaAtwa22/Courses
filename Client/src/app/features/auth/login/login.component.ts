import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  private fb          = inject(FormBuilder);
  private authService = inject(AuthService);
  private router      = inject(Router);
  private route       = inject(ActivatedRoute);

  loginForm = this.fb.nonNullable.group({
    email:    ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
  });

  isLoading      = false;
  errorMessage   = '';
  successMessage = '';
  showPassword   = false;

  ngOnInit() {
    if (this.route.snapshot.queryParams['reset'] === 'success') {
      this.successMessage = 'Password reset successfully. You can now sign in.';
    }
  }

  onSubmit() {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading    = true;
    this.errorMessage = '';

    this.authService.login(this.loginForm.getRawValue()).subscribe({
      next: (response) => {
        if (response.requiresTwoFactor) {
          this.router.navigate(['/auth/two-factor'], {
            queryParams: { email: this.loginForm.value.email }
          });
        } else {
          this.router.navigate(['/']);
        }
      },
      error: (err) => {
        this.isLoading    = false;
        this.errorMessage = err.error?.detail || err.error?.title || 'Login failed. Please check your credentials.';
      }
    });
  }
}
