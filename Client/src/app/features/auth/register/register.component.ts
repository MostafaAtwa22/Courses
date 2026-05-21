import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { Gender, Role } from '../../../shared/models/identity.models';

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
  roles = Object.values(Role);
  selectedRoleLabel = '';

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
        this.router.navigate(['/auth/confirm-email']);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.detail || err.error?.title || 'Registration failed. Please try again.';
      }
    });
  }
}
