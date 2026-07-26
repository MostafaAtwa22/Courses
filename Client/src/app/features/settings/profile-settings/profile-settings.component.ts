import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { SessionService } from '../../auth/services/session.service';
import { ProfileService } from '../../profiles/services/profile.service';
import { BaseIdentityResponse } from '../../auth/models/auth.models';
import { UpdateProfileDto } from '../../profiles/models/profile.models';
import { Gender } from '../../../shared/models/identity.models';

@Component({
  selector: 'app-profile-settings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile-settings.component.html',
  styleUrl: './profile-settings.component.scss'
})
export class ProfileSettingsComponent implements OnInit {
  profileForm: FormGroup;
  currentUser: BaseIdentityResponse | null = null;
  selectedImage: File | null = null;
  imagePreview: string | null = null;
  isSubmitting = false;
  isUploadingImage = false;
  isEditing = false;
  genders = Object.values(Gender);
  private readonly defaultMalePic    = 'assets/users/default-male.png';
  private readonly defaultFemalePic  = 'assets/users/default-female.png';

  constructor(
    private fb: FormBuilder,
    private sessionService: SessionService,
    private profileService: ProfileService
  ) {
    this.profileForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      userName: ['', [Validators.required, Validators.minLength(3)]],
      phoneNumber: [''],
      gender: [Gender.Male, [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.currentUser = this.sessionService.currentUser();
    if (this.currentUser) {
      this.seedForm();
    }
  }

  seedForm(): void {
    if (this.currentUser) {
      this.profileForm.patchValue({
        firstName: this.currentUser.firstName || '',
        lastName: this.currentUser.lastName || '',
        userName: this.currentUser.userName || '',
        phoneNumber: this.currentUser.phoneNumber || '',
        gender: this.currentUser.gender || Gender.Male
      });
      this.imagePreview = this.currentUser.profilePicture || null;
    }
  }

  onImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.selectedImage = input.files[0];
      
      // Create preview
      const reader = new FileReader();
      reader.onload = (e) => {
        this.imagePreview = e.target?.result as string;
      };
      reader.readAsDataURL(this.selectedImage);
    }
  }

  uploadImage(): void {
    if (!this.selectedImage) return;

    this.isUploadingImage = true;
    this.profileService.updateProfileImage(this.selectedImage).subscribe({
      next: () => {
        // Update the current user's profile picture in session
        if (this.currentUser) {
          const updatedUser = { ...this.currentUser, profilePicture: this.imagePreview || undefined };
          const token = this.sessionService.getToken();
          if (token) {
            this.sessionService.saveSession(token, updatedUser);
          }
        }
        this.isUploadingImage = false;
        this.selectedImage = null;
      },
      error: (error) => {
        console.error('Error uploading image:', error);
        this.isUploadingImage = false;
      }
    });
  }

  removeImage(): void {
    this.profileService.deleteProfileImage().subscribe({
      next: () => {
        if (this.currentUser) {
          const updatedUser = { ...this.currentUser, profilePicture: undefined };
          const token = this.sessionService.getToken();
          if (token) {
            this.sessionService.saveSession(token, updatedUser);
            this.currentUser = updatedUser;
          }
        }
        this.imagePreview = null;
        this.selectedImage = null;
      },
      error: (error) => {
        console.error('Error removing image:', error);
      }
    });
  }

  onSubmit(): void {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    const updateData: UpdateProfileDto = this.profileForm.value;

    this.profileService.updateProfile(updateData).subscribe({
      next: () => {
        // Update session with new user data
        if (this.currentUser) {
          const updatedUser: BaseIdentityResponse = {
            ...this.currentUser,
            ...updateData
          };
          const token = this.sessionService.getToken();
          if (token) {
            this.sessionService.saveSession(token, updatedUser);
            // We should also update local currentUser so UI updates immediately
            this.currentUser = updatedUser;
          }
        }
        this.isSubmitting = false;
        this.isEditing = false;
        alert('Profile updated successfully!');
      },
      error: (error) => {
        console.error('Error updating profile:', error);
        this.isSubmitting = false;
        alert('Failed to update profile. Please try again.');
      }
    });
  }

  toggleEdit() {
    this.isEditing = !this.isEditing;
    if (!this.isEditing) {
      this.seedForm(); // Reset form to current user data if canceling
    }
  }

  get profilePicture(): string {
    if (this.imagePreview) return this.imagePreview;
    const gender = this.profileForm.get('gender')?.value || this.currentUser?.gender;
    return gender === Gender.Female ? this.defaultFemalePic : this.defaultMalePic;
  }

  get fullName(): string {
    return `${this.profileForm.get('firstName')?.value || ''} ${this.profileForm.get('lastName')?.value || ''}`.trim();
  }
}
