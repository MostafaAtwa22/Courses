import { Component, Input, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AdminDashboardService, PendingInstructor } from '../../../services/admin-dashboard.service';
import { ToastService } from '../../../../../core/services/toast.service';

@Component({
  selector: 'app-pending-instructors-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pending-instructors-card.component.html',
  styleUrl: './pending-instructors-card.component.scss'
})
export class PendingInstructorsCardComponent {
  @Input() pendingInstructors: PendingInstructor[] = [];
  
  private adminDashboardService = inject(AdminDashboardService);
  private toastService = inject(ToastService);
  private router = inject(Router);

  approveInstructor(id: string, name: string) {
    this.adminDashboardService.changeInstructorStatus(id, 'Verfied').subscribe({
      next: () => {
        this.pendingInstructors = this.pendingInstructors.filter(i => i.id !== id);
        this.toastService.success(`Approved verification for ${name}`);
      },
      error: (error) => {
        this.toastService.error(`Failed to approve instructor: ${error.message}`);
      }
    });
  }

  rejectInstructor(id: string, name: string) {
    this.adminDashboardService.changeInstructorStatus(id, 'Unverfied').subscribe({
      next: () => {
        this.pendingInstructors = this.pendingInstructors.filter(i => i.id !== id);
        this.toastService.info(`Verification declined for ${name}`);
      },
      error: (error) => {
        this.toastService.error(`Failed to reject instructor: ${error.message}`);
      }
    });
  }

  viewAllPending() {
    this.router.navigate(['/admin/dashboard/instructors'], { queryParams: { status: 'Pending' } });
  }
}