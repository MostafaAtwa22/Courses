import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { Gender, Role } from '../../../shared/models/identity.models';

export interface PasswordCondition {
  label: string;
  met: boolean;
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent implements OnInit {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  genders = Object.values(Gender);
  Gender = Gender;
  roles = Object.values(Role);
  selectedRoleLabel = '';

  showPassword = false;
  showConfirmPassword = false;

  registerForm = this.fb.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    userName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', Validators.required],
    gender: [Gender.Male, Validators.required],
    role: [Role.Student, Validators.required],
    terms: [false, Validators.requiredTrue]
  }, { validators: this.passwordMatchValidator });

  ngOnInit() {
    const savedRole = localStorage.getItem('selected_role');
    if (!savedRole || (savedRole !== Role.Student && savedRole !== Role.Instructor)) {
      this.router.navigate(['/auth/role-select']);
      return;
    }
    this.registerForm.patchValue({ role: savedRole as Role });
    this.selectedRoleLabel = savedRole === Role.Student ? 'Student' : 'Instructor';
  }

  isLoading = false;
  errorMessage = '';

  // ── Password strength ──────────────────────────────────────────────────────

  get passwordValue(): string {
    return this.registerForm.get('password')?.value ?? '';
  }

  get passwordConditions(): PasswordCondition[] {
    const v = this.passwordValue;
    return [
      { label: 'At least 8 characters',       met: v.length >= 8 },
      { label: 'One uppercase letter (A-Z)',   met: /[A-Z]/.test(v) },
      { label: 'One lowercase letter (a-z)',   met: /[a-z]/.test(v) },
      { label: 'One number (0-9)',             met: /\d/.test(v) },
      { label: 'One special character (!@#…)', met: /[^A-Za-z0-9]/.test(v) },
    ];
  }

  get passwordScore(): number {
    return this.passwordConditions.filter(c => c.met).length;
  }

  get passwordStrengthLabel(): string {
    const score = this.passwordScore;
    if (score === 0) return '';
    if (score <= 2) return 'Weak';
    if (score === 3) return 'Fair';
    if (score === 4) return 'Good';
    return 'Strong';
  }

  get passwordStrengthClass(): string {
    const score = this.passwordScore;
    if (score === 0) return '';
    if (score <= 2) return 'weak';
    if (score === 3) return 'fair';
    if (score === 4) return 'good';
    return 'strong';
  }

  get showPasswordStrength(): boolean {
    return this.passwordValue.length > 0;
  }

  // ── Gender toggle ──────────────────────────────────────────────────────────

  selectGender(gender: Gender) {
    this.registerForm.patchValue({ gender });
  }

  get selectedGender(): Gender {
    return this.registerForm.get('gender')?.value as Gender;
  }

  // ── Form ───────────────────────────────────────────────────────────────────

  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');

    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    return null;
  }

  onSubmit() {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const { terms, ...registerDto } = this.registerForm.getRawValue();

    this.authService.register(registerDto).subscribe({
      next: () => {
        this.router.navigate(['/auth/confirm-email'], {
          queryParams: { email: registerDto.email }
        });
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.detail || err.error?.title || 'Registration failed. Please try again.';
      }
    });
  }
}
