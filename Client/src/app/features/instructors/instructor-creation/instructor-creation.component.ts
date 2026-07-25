import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators, FormGroup } from '@angular/forms';
import { InstructorService } from '../services/instructor.service';
import { AuthService } from '../../auth/services/auth.service';

@Component({
  selector: 'app-instructor-creation',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './instructor-creation.component.html',
  styleUrl: './instructor-creation.component.scss'
})
export class InstructorCreationComponent {
  private fb = inject(FormBuilder);
  private instructorService = inject(InstructorService);
  private authService = inject(AuthService);
  private router = inject(Router);

  instructorForm = this.fb.group({
    bio: ['', [Validators.required, Validators.minLength(50), Validators.maxLength(1000)]],
    title: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
    linkedInProfileUrl: ['', [Validators.pattern(/^https?:\/\/(www\.)?linkedin\.com\/.*$/)]],
    gitHubProfileUrl: ['', [Validators.pattern(/^https?:\/\/(www\.)?github\.com\/.*$/)]],
    cvUrl: [null as File | null, [Validators.required]]
  });

  isLoading = false;
  errorMessage = '';
  cvFileName = '';
  cvFileSize = '';

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      // Validate file type
      const allowedTypes = ['application/pdf', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'];
      if (!allowedTypes.includes(file.type)) {
        this.errorMessage = 'Only PDF and DOCX files are allowed.';
        this.instructorForm.patchValue({ cvUrl: null });
        return;
      }

      // Validate file size (max 5MB)
      const maxSize = 5 * 1024 * 1024;
      if (file.size > maxSize) {
        this.errorMessage = 'File size must be less than 5MB.';
        this.instructorForm.patchValue({ cvUrl: null });
        return;
      }

      this.cvFileName = file.name;
      this.cvFileSize = this.formatFileSize(file.size);
      this.instructorForm.patchValue({ cvUrl: file });
      this.errorMessage = '';
    }
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

    this.isLoading = true;
    this.errorMessage = '';

    const formValue = this.instructorForm.value;
    const formData = new FormData();

    formData.append('Bio', formValue.bio || '');
    formData.append('Title', formValue.title || '');
    formData.append('LinkedInProfileUrl', formValue.linkedInProfileUrl || '');
    formData.append('GitHubProfileUrl', formValue.gitHubProfileUrl || '');
    
    if (formValue.cvUrl) {
      formData.append('CvUrl', formValue.cvUrl);
    }

    this.instructorService.createInstructor(formData).subscribe({
      next: () => {
        this.isLoading = false;
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.detail || err.error?.title || 'Failed to create instructor profile. Please try again.';
      }
    });
  }

  get bio() { return this.instructorForm.get('bio'); }
  get title() { return this.instructorForm.get('title'); }
  get linkedInProfileUrl() { return this.instructorForm.get('linkedInProfileUrl'); }
  get gitHubProfileUrl() { return this.instructorForm.get('gitHubProfileUrl'); }
  get cvUrl() { return this.instructorForm.get('cvUrl'); }
}
