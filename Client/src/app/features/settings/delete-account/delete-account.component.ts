import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { ProfileService } from '../../profiles/services/profile.service';
import { DeleteProfileDto } from '../../profiles/models/profile.models';
import { Router } from '@angular/router';
import { AuthService } from '../../auth/services/auth.service';
import { AlertService } from '../../../core/services/alert.service';
import { ToastService } from '../../../core/services/toast.service';
import Swal from 'sweetalert2';

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

  constructor(
    private fb: FormBuilder,
    private profileService: ProfileService,
    private router: Router,
    private authService: AuthService,
    private alertService: AlertService,
    private toastService: ToastService
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

    this.alertService.custom({
      title: 'Delete Account',
      html: `
        <div style="text-align:	left; margin: 20px 0;">
          <p style="margin-bottom: 15px; color: #dc2626;"><strong>Warning: This action cannot be undone!</strong></p>
          <p style="margin-bottom: 15px;">To confirm deletion, please type <strong>DELETE</strong> below:</p>
          <input id="swal-confirm-text" class="swal2-input" placeholder="Type DELETE" type="text">
        </div>
      `,
      focusConfirm: false,
      showCancelButton: true,
      confirmButtonText: 'Delete Account',
      cancelButtonText: 'Cancel',
      confirmButtonColor: 'var(--error)',
      cancelButtonColor: 'var(--border-color)',
      reverseButtons: true,
      preConfirm: () => {
        const confirmText = (document.getElementById('swal-confirm-text') as HTMLInputElement).value;

        if (confirmText !== 'DELETE') {
          Swal.showValidationMessage('Please type DELETE to confirm');
        }

        return confirmText;
      }
    }).then((result) => {
      if (result.isConfirmed) {
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
            this.toastService.error('Failed to delete account. Please check your password and try again.');
          }
        });
      }
    });
  }
}
