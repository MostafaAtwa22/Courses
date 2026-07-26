import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { ProfileService } from '../../profiles/services/profile.service';
import { DeleteProfileDto } from '../../profiles/models/profile.models';
import { Router } from '@angular/router';
import { AuthService } from '../../auth/services/auth.service';

@Component({
  selector: 'app-delete-account',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './delete-account.component.html',
  styleUrl: './delete-account.component.scss'
})
export class DeleteAccountComponent {
  deleteAccountForm: FormGroup;
  isDeleting = false;
  showConfirmModal = false;
  confirmText = '';

  constructor(
    private fb: FormBuilder,
    private profileService: ProfileService,
    private router: Router,
    private authService: AuthService
  ) {
    this.deleteAccountForm = this.fb.group({
      password: ['', [Validators.required]],
      confirmDelete: [false, [Validators.requiredTrue]]
    });
  }

  openConfirmModal(): void {
    if (this.deleteAccountForm.invalid) {
      this.deleteAccountForm.markAllAsTouched();
      return;
    }
    this.showConfirmModal = true;
    this.confirmText = '';
  }

  closeConfirmModal(): void {
    this.showConfirmModal = false;
    this.confirmText = '';
  }

  confirmDeleteAccount(): void {
    if (this.confirmText !== 'DELETE') {
      alert('Please type DELETE to confirm');
      return;
    }

    this.isDeleting = true;
    const deleteData: DeleteProfileDto = {
      password: this.deleteAccountForm.get('password')?.value
    };

    this.profileService.deleteProfile(deleteData).subscribe({
      next: () => {
        this.isDeleting = false;
        this.authService.clearSession();
        this.router.navigate(['/auth/login']);
      },
      error: (error) => {
        console.error('Error deleting account:', error);
        this.isDeleting = false;
        this.closeConfirmModal();
        alert('Failed to delete account. Please check your password and try again.');
      }
    });
  }

  cancelDelete(): void {
    this.closeConfirmModal();
  }
}
