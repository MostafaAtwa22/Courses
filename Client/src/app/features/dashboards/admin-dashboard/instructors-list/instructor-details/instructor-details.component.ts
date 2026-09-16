import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { InstructorService } from '../../../../instructors/services/instructor.service';
import { InstructorPrivateResponse } from '../../../../instructors/models/instructor.models';
import { ToastService } from '../../../../../core/services/toast.service';

@Component({
  selector: 'app-instructor-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './instructor-details.component.html',
  styleUrl: './instructor-details.component.scss'
})
export class InstructorDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private instructorService = inject(InstructorService);
  private toastService = inject(ToastService);

  instructor: InstructorPrivateResponse | null = null;
  instructorId: string = '';

  ngOnInit() {
    this.instructorId = this.route.snapshot.paramMap.get('id') || '';
    if (this.instructorId) {
      this.loadInstructorDetails();
    } else {
      this.router.navigate(['/admin/dashboard/instructors']);
    }
  }

  loadInstructorDetails() {
    this.instructorService.getInstructorById(this.instructorId).subscribe({
      next: (instructor: InstructorPrivateResponse) => {
        this.instructor = instructor;
      },
      error: () => {
        this.toastService.error('Failed to load instructor details');
        this.router.navigate(['/admin/dashboard/instructors']);
      }
    });
  }

  goBack() {
    this.router.navigate(['/admin/dashboard/instructors']);
  }

  changeStatus(newStatus: string) {
    if (!this.instructor) return;
    
    this.instructorService.changeInstructorStatus(this.instructor.id, newStatus).subscribe({
      next: () => {
        this.toastService.success(`Instructor status changed to ${newStatus}`);
        this.loadInstructorDetails();
      },
      error: () => {
        this.toastService.error('Failed to change instructor status');
      }
    });
  }

  getStatusBadgeClass(status: string): string {
    switch (status) {
      case 'Verfied':
        return 'status-verified';
      case 'Pending':
        return 'status-pending';
      case 'Unverfied':
        return 'status-unverified';
      default:
        return '';
    }
  }
}
