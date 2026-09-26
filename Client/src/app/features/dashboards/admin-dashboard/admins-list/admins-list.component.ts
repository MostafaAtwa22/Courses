import { Component, inject, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AdminService } from '../../../admin/services/admin.service';
import { AdminResponse, AdminQueryParams } from '../../../admin/models/admin.models';
import { PaginatedResultModel } from '../../../../shared/models/paginated-result.model';

@Component({
  selector: 'app-admins-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admins-list.component.html',
  styleUrl: './admins-list.component.scss'
})
export class AdminsListComponent implements OnInit {
  private adminService = inject(AdminService);
  private router = inject(Router);

  adminsResult: PaginatedResultModel<AdminResponse> = new PaginatedResultModel<AdminResponse>();
  params: AdminQueryParams = { pageNumber: 1, pageSize: 10 };
  searchQuery = '';
  genderFilter = '';
  roleFilter = '';
  sortBy = 'name';
  sortDescending = false;
  isFilterDropdownOpen = false;

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    const target = event.target as HTMLElement;
    const dropdown = target.closest('.dropdown');
    
    if (!dropdown && this.isFilterDropdownOpen) {
      this.isFilterDropdownOpen = false;
    }
  }

  ngOnInit() {
    this.loadAdmins();
  }

  toggleFilterDropdown() {
    this.isFilterDropdownOpen = !this.isFilterDropdownOpen;
  }

  loadAdmins() {
    const params: AdminQueryParams = {
      pageNumber: this.params.pageNumber,
      pageSize: this.params.pageSize,
      sortDescending: this.sortDescending
    };

    if (this.searchQuery?.trim()) {
      params.searchTerm = this.searchQuery.trim();
    }
    
    if (this.genderFilter?.trim()) {
      params.gender = this.genderFilter.trim();
    }

    if (this.roleFilter?.trim()) {
      params.role = this.roleFilter.trim();
    }
    
    if (this.sortBy?.trim()) {
      params.sortBy = this.sortBy.trim();
    }

    console.log('Loading admins with params:', params);
    console.log('Gender filter value:', this.genderFilter);
    
    this.adminService.getAllAdmins(params).subscribe({
      next: (res: PaginatedResultModel<AdminResponse>) => {
        this.adminsResult = res;
        console.log('Received admins:', res.items.length, 'items');
        if (res.items.length > 0) {
          console.log('First admin gender:', res.items[0].gender);
        }
      }
    });
  }

  onFilterChange(filterType: 'gender' | 'role', value: string) {
    if (filterType === 'gender') {
      this.genderFilter = value;
    } else if (filterType === 'role') {
      this.roleFilter = value;
    }
    this.params.pageNumber = 1; // Reset to first page when filtering
    this.loadAdmins();
  }

  onSearch() {
    this.params.pageNumber = 1;
    this.loadAdmins();
  }

  clearFilters() {
    this.searchQuery = '';
    this.genderFilter = '';
    this.roleFilter = '';
    this.sortBy = '';
    this.sortDescending = false;
    this.onSearch();
    this.isFilterDropdownOpen = false;
  }

  onPageChange(page: number) {
    if (page < 1 || page > this.adminsResult.totalPages) return;
    this.params.pageNumber = page;
    this.loadAdmins();
  }

  deleteAdmin(adminId: string) {
    if (confirm('Are you sure you want to delete this admin?')) {
      this.adminService.deleteAdmin(adminId).subscribe({
        next: () => {
          this.loadAdmins();
        }
      });
    }
  }

  getPagesArray(): number[] {
    return Array.from({ length: this.adminsResult.totalPages }, (_, i) => i + 1);
  }

  viewAdminDetails(adminId: string) {
    this.router.navigate(['/admin/dashboard/admins', adminId]);
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