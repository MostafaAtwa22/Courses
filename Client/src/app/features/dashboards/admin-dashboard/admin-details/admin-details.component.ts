import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminService } from '../../../admin/services/admin.service';
import { AdminResponse } from '../../../admin/models/admin.models';

@Component({
  selector: 'app-admin-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-details.component.html',
  styleUrl: './admin-details.component.scss'
})
export class AdminDetailsComponent implements OnInit {
  private adminService = inject(AdminService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  admin: AdminResponse | null = null;
  isLoading = true;
  error: string | null = null;

  ngOnInit() {
    const adminId = this.route.snapshot.paramMap.get('id');
    if (adminId) {
      this.loadAdminDetails(adminId);
    }
  }

  loadAdminDetails(id: string) {
    this.isLoading = true;
    this.error = null;
    
    this.adminService.getAdminById(id).subscribe({
      next: (admin) => {
        this.admin = admin;
        this.isLoading = false;
      },
      error: (err) => {
        this.error = 'Failed to load admin details';
        this.isLoading = false;
        console.error('Error loading admin details:', err);
      }
    });
  }

  goBack() {
    this.router.navigate(['/admin/dashboard/admins']);
  }

  deleteAdmin() {
    if (!this.admin) return;
    
    if (confirm(`Are you sure you want to delete ${this.admin.firstName} ${this.admin.lastName}?`)) {
      this.adminService.deleteAdmin(this.admin.id).subscribe({
        next: () => {
          this.router.navigate(['/admin/dashboard/admins']);
        },
        error: (err) => {
          this.error = 'Failed to delete admin';
          console.error('Error deleting admin:', err);
        }
      });
    }
  }

  getRoleClass(role: string): string {
    switch (role) {
      case 'SuperAdmin':
        return 'role-superadmin';
      case 'Admin':
        return 'role-admin';
      default:
        return 'role-default';
    }
  }
}
