import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AccountService } from '../../account/services/account.service';

@Component({
  selector: 'app-forget-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  styleUrl: './forget-password.component.scss',
  templateUrl: './forget-password.component.html'
})
export class ForgetPasswordComponent {
  private fb = inject(FormBuilder);
  private accountService = inject(AccountService);

  forgetForm = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]]
  });

  isLoading = false;
  successMessage = '';
  errorMessage = '';

  onSubmit() {
    if (this.forgetForm.invalid) return;

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.accountService.forgetPassword(this.forgetForm.getRawValue()).subscribe({
      next: () => {
        this.isLoading = false;
        this.successMessage = 'Password reset link has been sent to your email.';
        this.forgetForm.reset();
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.detail || 'Failed to send reset link. Please try again.';
      }
    });
  }
}
