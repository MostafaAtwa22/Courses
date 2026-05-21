import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { AccountService } from '../../account/services/account.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  styleUrl: './reset-password.component.scss',
  templateUrl: './reset-password.component.html'
})
export class ResetPasswordComponent implements OnInit {
  private fb = inject(FormBuilder);
  private accountService = inject(AccountService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  email = '';
  token = '';
  isLoading = false;
  errorMessage = '';

  resetForm = this.fb.nonNullable.group({
    newPassword: ['', [Validators.required, Validators.minLength(8)]],
    confirmNewPassword: ['', Validators.required]
  }, { validators: this.passwordMatchValidator });

  ngOnInit() {
    this.email = this.route.snapshot.queryParams['email'] || '';
    this.token = this.route.snapshot.queryParams['token'] || '';

    if (!this.email || !this.token) {
      this.errorMessage = 'Invalid password reset link.';
      this.resetForm.disable();
    }
  }

  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('newPassword');
    const confirmPassword = control.get('confirmNewPassword');
    
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    return null;
  }

  onSubmit() {
    if (this.resetForm.invalid) {
      this.resetForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const request = {
      email: this.email,
      token: this.token,
      newPassword: this.resetForm.value.newPassword!,
      confirmNewPassword: this.resetForm.value.confirmNewPassword!
    };

    this.accountService.resetPassword(request).subscribe({
      next: () => {
        // Redirect to login on success
        this.router.navigate(['/auth/login'], { queryParams: { reset: 'success' } });
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.detail || 'Failed to reset password. The link might be expired.';
      }
    });
  }
}
