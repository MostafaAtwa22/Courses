import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators, FormGroup } from '@angular/forms';
import { InstructorService } from '../../../instructors/services/instructor.service';
import { SessionService } from '../../../auth/services/session.service';
import { AuthService } from '../../../auth/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { AlertService } from '../../../../core/services/alert.service';

@Component({
  selector: 'app-instructor-update-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './instructor-update-profile.component.html',
  styleUrl: './instructor-update-profile.component.scss'
})
export class InstructorUpdateProfileComponent implements OnInit {
  private fb = inject(FormBuilder);
  private instructorService = inject(InstructorService);
  private sessionService = inject(SessionService);
  private router = inject(Router);
  private authService = inject(AuthService);
  private toastService = inject(ToastService);
  private alertService = inject(AlertService);
  currentUser = this.sessionService.currentUser;

  instructorForm = this.fb.group({
    bio: ['', [Validators.required, Validators.minLength(50), Validators.maxLength(1000)]],
    title: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
    linkedInProfileUrl: ['', [Validators.required, Validators.pattern(/^https?:\/\/(www\.)?linkedin\.com\/.*$/)]],
    gitHubProfileUrl: ['', [Validators.required, Validators.pattern(/^https?:\/\/(www\.)?github\.com\/.*$/)]],
    cvUrl: [null as File | null]
  });

  isLoading = false;
  isLoadingData = true;
  errorMessage = '';
  successMessage = '';
  cvFileName = '';
  cvFileSize = '';
  selectedFile: File | null = null;
  currentCvUrl = '';
  instructorId = '';

  ngOnInit() {
    this.loadCurrentInstructorData();
  }

  loadCurrentInstructorData() {
    this.isLoadingData = true;
    this.errorMessage = '';

    this.instructorService.getCurrentInstructor().subscribe({
      next: (instructor) => {
        this.instructorId = instructor.id || this.currentUser()?.id || '';
        this.currentCvUrl = instructor.cvUrl || '';
        
        this.instructorForm.patchValue({
          bio: instructor.bio || '',
          title: instructor.title || '',
          linkedInProfileUrl: instructor.linkedInProfileUrl || '',
          gitHubProfileUrl: instructor.gitHubProfileUrl || ''
        });

        this.isLoadingData = false;
      },
      error: (err) => {
        this.isLoadingData = false;
        this.errorMessage = err.error?.detail || err.error?.title || 'Failed to load your profile data. Please try again.';
      }
    });
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      const allowedTypes = ['application/pdf', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'];
      if (!allowedTypes.includes(file.type)) {
        this.errorMessage = 'Only PDF and DOCX files are allowed.';
        this.instructorForm.patchValue({ cvUrl: null });
        this.selectedFile = null;
        return;
      }

      const maxSize = 5 * 1024 * 1024;
      if (file.size > maxSize) {
        this.errorMessage = 'File size must be less than 5MB.';
        this.instructorForm.patchValue({ cvUrl: null });
        this.selectedFile = null;
        return;
      }

      this.cvFileName = file.name;
      this.cvFileSize = this.formatFileSize(file.size);
      this.instructorForm.patchValue({ cvUrl: file });
      this.selectedFile = file;
      this.errorMessage = '';
    }
  }

  removeSelectedFile() {
    this.cvFileName = '';
    this.cvFileSize = '';
    this.instructorForm.patchValue({ cvUrl: null });
    this.selectedFile = null;
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  }

  onSubmit() {
    if (this.instructorForm.invalid) {
      this.instructorForm.markAllAsTouched();
      return;
    }

    // Show confirmation dialog before applying changes
    this.alertService.confirm(
      'Update Profile?',
      'Are you sure you want to update your instructor profile information?',
      'Yes, Update',
      'Cancel'
    ).then((result) => {
      if (result.isConfirmed) {
        this.isLoading = true;
        this.errorMessage = '';
        this.successMessage = '';

        const formValue = this.instructorForm.value;
        const formData = new FormData();

        formData.append('Bio', formValue.bio || '');
        formData.append('Title', formValue.title || '');
        formData.append('LinkedInProfileUrl', formValue.linkedInProfileUrl || '');
        formData.append('GitHubProfileUrl', formValue.gitHubProfileUrl || '');
        
        // Only append CV if a new file was selected
        if (this.selectedFile) {
          formData.append('CvUrl', this.selectedFile, this.selectedFile.name);
        }

        this.instructorService.updateInstructor(this.instructorId, formData).subscribe({
          next: () => {
            this.isLoading = false;
            
            this.toastService.success('Your profile has been updated successfully.', 'Success');
            
            this.loadCurrentInstructorData();
          },
          error: (err) => {
            this.isLoading = false;
            this.errorMessage = err.error?.detail || err.error?.title || 'Failed to update instructor profile. Please try again.';
          }
        });
      }
    });
  }

  onCancel() {
    this.router.navigate(['/instructor/dashboard']);
  }

  get bio() { return this.instructorForm.get('bio'); }
  get title() { return this.instructorForm.get('title'); }
  get linkedInProfileUrl() { return this.instructorForm.get('linkedInProfileUrl'); }
  get gitHubProfileUrl() { return this.instructorForm.get('gitHubProfileUrl'); }
  get cvUrl() { return this.instructorForm.get('cvUrl'); }
}