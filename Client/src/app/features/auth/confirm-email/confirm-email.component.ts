import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SecurityService } from '../../security/services/security.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-confirm-email',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  styleUrl: './confirm-email.component.scss',
  templateUrl: './confirm-email.component.html'
})
export class ConfirmEmailComponent implements OnInit {
  private securityService = inject(SecurityService);
  private authService     = inject(AuthService);
  private route           = inject(ActivatedRoute);
  private router          = inject(Router);
  private fb              = inject(FormBuilder);

  form!: FormGroup;
  isLoading    = false;
  errorMessage = '';
  emailFromRoute = '';

  ngOnInit() {
    this.emailFromRoute = this.route.snapshot.queryParams['email'] ?? '';

    this.form = this.fb.group({
      email: [this.emailFromRoute, [Validators.required, Validators.email]],
      code:  ['', [Validators.required]]
    });
  }

  get emailControl() { return this.form.get('email'); }
  get codeControl()  { return this.form.get('code'); }

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading    = true;
    this.errorMessage = '';

    const { email, code } = this.form.value;

    this.securityService.confirmEmail({ email, code }).subscribe({
      next: (response) => {
        if (response.token) {
          this.authService.saveSession(response.token, response);
        }
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.isLoading    = false;
        this.errorMessage = err.error?.detail || 'Invalid verification code. Please try again.';
      }
    });
  }
}
