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
  private fb             = inject(FormBuilder);
  private accountService = inject(AccountService);
  private router         = inject(Router);
  private route          = inject(ActivatedRoute);

  email        = '';
  token        = '';
  isLoading    = false;
  errorMessage = '';

  showPassword        = false;
  showConfirmPassword = false;

  resetForm = this.fb.nonNullable.group({
    newPassword:        ['', [Validators.required, Validators.minLength(8)]],
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

  // ── Password strength ──────────────────────────────────────────────────────

  get passwordValue(): string {
    return this.resetForm.get('newPassword')?.value ?? '';
  }

  get passwordConditions() {
    const v = this.passwordValue;
    return [
      { label: 'At least 8 characters',       met: v.length >= 8 },
      { label: 'One uppercase letter (A–Z)',   met: /[A-Z]/.test(v) },
      { label: 'One lowercase letter (a–z)',   met: /[a-z]/.test(v) },
      { label: 'One number (0–9)',             met: /\d/.test(v) },
      { label: 'One special character (!@#…)', met: /[^A-Za-z0-9]/.test(v) },
    ];
  }

  get passwordScore(): number {
    return this.passwordConditions.filter(c => c.met).length;
  }

  get passwordStrengthLabel(): string {
    const s = this.passwordScore;
    if (s === 0) return '';
    if (s <= 2)  return 'Weak';
    if (s === 3) return 'Fair';
    if (s === 4) return 'Good';
    return 'Strong';
  }

  get passwordStrengthClass(): string {
    const s = this.passwordScore;
    if (s === 0) return '';
    if (s <= 2)  return 'weak';
    if (s === 3) return 'fair';
    if (s === 4) return 'good';
    return 'strong';
  }

  get showPasswordStrength(): boolean {
    return this.passwordValue.length > 0;
  }

  // ── Validation ─────────────────────────────────────────────────────────────

  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password        = control.get('newPassword');
    const confirmPassword = control.get('confirmNewPassword');

    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    return null;
  }

  // ── Submit ─────────────────────────────────────────────────────────────────

  onSubmit() {
    if (this.resetForm.invalid) {
      this.resetForm.markAllAsTouched();
      return;
    }

    this.isLoading    = true;
    this.errorMessage = '';

    const request = {
      email:              this.email,
      token:              this.token,
      newPassword:        this.resetForm.value.newPassword!,
      confirmNewPassword: this.resetForm.value.confirmNewPassword!
    };

    this.accountService.resetPassword(request).subscribe({
      next: () => {
        this.router.navigate(['/auth/login'], { queryParams: { reset: 'success' } });
      },
      error: (err) => {
        this.isLoading    = false;
        this.errorMessage = err.error?.detail || 'Failed to reset password. The link might be expired.';
      }
    });
  }
}
