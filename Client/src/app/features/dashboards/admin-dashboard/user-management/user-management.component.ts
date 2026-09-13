import { Component, inject, OnInit, HostListener, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../../admin/services/user.service';
import { UserResponse, UserRolesManage, CheckBoxRoleManage, RolesResponse, LockUserDto } from '../../../admin/models/user.models';
import { PaginatedResultModel } from '../../../../shared/models/paginated-result.model';
import { QueryParams } from '../../../../shared/models/query-params.model';
import Swal from 'sweetalert2';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.scss'
})
export class UserManagementComponent implements OnInit {
  private userService = inject(UserService);
  private toastr = inject(ToastrService);
  private cdr = inject(ChangeDetectorRef);

  usersResult: PaginatedResultModel<UserResponse> = new PaginatedResultModel<UserResponse>();
  searchQuery = '';
  sortBy = 'userName';
  sortDescending = false;
  isFilterDropdownOpen = false;
  availableRoles: RolesResponse[] = [];
  currentPage = 1;

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    const target = event.target as HTMLElement;
    const dropdown = target.closest('.dropdown');
    
    if (!dropdown && this.isFilterDropdownOpen) {
      this.isFilterDropdownOpen = false;
    }
  }

  ngOnInit() {
    this.loadUsers();
    this.loadAvailableRoles();
  }

  toggleFilterDropdown() {
    this.isFilterDropdownOpen = !this.isFilterDropdownOpen;
  }

  loadUsers() {
    const params: QueryParams = {
      pageNumber: this.currentPage,
      pageSize: 10,
      sortDescending: this.sortDescending
    };

    if (this.searchQuery) {
      params.searchTerm = this.searchQuery;
    }
    
    if (this.sortBy) {
      params.sortBy = this.sortBy;
    }
    
    this.userService.getAll(params).subscribe({
      next: (res: PaginatedResultModel<UserResponse>) => {
        this.usersResult = res;
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.toastr.error('Failed to load users', 'Error');
      }
    });
  }

  loadAvailableRoles() {
    this.userService.getAllRoles().subscribe({
      next: (roles) => {
        this.availableRoles = roles;
      },
      error: (error) => {
        this.toastr.error('Failed to load roles', 'Error');
      }
    });
  }

  onSearch() {
    this.currentPage = 1;
    this.loadUsers();
  }

  clearFilters() {
    this.searchQuery = '';
    this.sortBy = 'userName';
    this.sortDescending = false;
    this.onSearch();
    this.isFilterDropdownOpen = false;
  }

  onPageChange(page: number) {
    if (page < 1 || page > this.usersResult.totalPages) return;
    this.currentPage = page;
    this.loadUsers();
  }

  lockUser(user: UserResponse) {
    Swal.fire({
      title: 'Lock User',
      text: `Are you sure you want to lock ${user.firstName} ${user.lastName}?`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#dc2626',
      cancelButtonColor: '#64748b',
      confirmButtonText: 'Yes, lock it!',
      cancelButtonText: 'Cancel',
      customClass: {
        popup: 'custom-swal-popup',
        title: 'custom-swal-title',
        htmlContainer: 'custom-swal-content',
        confirmButton: 'custom-swal-confirm',
        cancelButton: 'custom-swal-cancel'
      }
    }).then((result) => {
      if (result.isConfirmed) {
        const lockoutEnd = new Date();
        lockoutEnd.setFullYear(lockoutEnd.getFullYear() + 100);
        
        const dto: LockUserDto = {
          lockoutEnd: lockoutEnd.toISOString()
        };

        this.userService.lockUser(user.id, dto).subscribe({
          next: () => {
            this.toastr.success('User locked successfully', 'Success');
            this.loadUsers();
            this.cdr.detectChanges();
          },
          error: (error) => {
            this.toastr.error('Failed to lock user', 'Error');
          }
        });
      }
    });
  }

  unlockUser(user: UserResponse) {
    Swal.fire({
      title: 'Unlock User',
      text: `Are you sure you want to unlock ${user.firstName} ${user.lastName}?`,
      icon: 'question',
      showCancelButton: true,
      confirmButtonColor: '#22c55e',
      cancelButtonColor: '#64748b',
      confirmButtonText: 'Yes, unlock it!',
      cancelButtonText: 'Cancel',
      customClass: {
        popup: 'custom-swal-popup',
        title: 'custom-swal-title',
        htmlContainer: 'custom-swal-content',
        confirmButton: 'custom-swal-confirm',
        cancelButton: 'custom-swal-cancel'
      }
    }).then((result) => {
      if (result.isConfirmed) {
        this.userService.unlockUser(user.id).subscribe({
          next: () => {
            this.toastr.success('User unlocked successfully', 'Success');
            this.loadUsers();
            this.cdr.detectChanges();
          },
          error: (error) => {
            this.toastr.error('Failed to unlock user', 'Error');
          }
        });
      }
    });
  }

  changeRoles(user: UserResponse) {
    this.userService.getUserRoles(user.id).subscribe({
      next: (userRoles) => {
        this.showRoleChangeModal(user, userRoles);
      },
      error: (error) => {
        this.toastr.error('Failed to load user roles', 'Error');
      }
    });
  }

  showRoleChangeModal(user: UserResponse, userRoles: UserRolesManage) {
    const currentRoles = new Set(userRoles.roles.map(r => r.roleName));
    
    let html = '<div style="display: flex; flex-direction: column; gap: 10px;">';
    
    this.availableRoles.forEach(role => {
      const isChecked = currentRoles.has(role.name) ? 'checked' : '';
      html += `
        <label style="display: flex; align-items: center; gap: 8px; cursor: pointer;">
          <input type="checkbox" value="${role.id}" data-role-name="${role.name}" ${isChecked} style="width: 18px; height: 18px;">
          <span>${role.name}</span>
        </label>
      `;
    });
    
    html += '</div>';

    Swal.fire({
      title: 'Change User Roles',
      text: `Select roles for ${user.firstName} ${user.lastName}`,
      icon: 'info',
      html: html,
      showCancelButton: true,
      confirmButtonColor: '#4f46e5',
      cancelButtonColor: '#64748b',
      confirmButtonText: 'Save Changes',
      cancelButtonText: 'Cancel',
      customClass: {
        popup: 'custom-swal-popup',
        title: 'custom-swal-title',
        htmlContainer: 'custom-swal-content',
        confirmButton: 'custom-swal-confirm',
        cancelButton: 'custom-swal-cancel'
      },
      didOpen: () => {
        const checkboxes = Swal.getPopup()?.querySelectorAll('input[type="checkbox"]');
        checkboxes?.forEach(checkbox => {
          checkbox.addEventListener('change', (e) => {
            const target = e.target as HTMLInputElement;
          });
        });
      },
      preConfirm: () => {
        const checkboxes = Swal.getPopup()?.querySelectorAll('input[type="checkbox"]:checked') as NodeListOf<HTMLInputElement>;
        const selectedRoles: CheckBoxRoleManage[] = [];
        
        checkboxes.forEach(checkbox => {
          selectedRoles.push({
            roleId: checkbox.value,
            roleName: checkbox.getAttribute('data-role-name') || '',
            isSelected: true
          });
        });

        return { roles: selectedRoles };
      }
    }).then((result) => {
      if (result.isConfirmed && result.value) {
        const dto: UserRolesManage = result.value;
        
        this.userService.updateUserRoles(user.id, dto).subscribe({
          next: () => {
            this.toastr.success('User roles updated successfully', 'Success');
            this.loadUsers();
            this.cdr.detectChanges();
          },
          error: (error) => {
            this.toastr.error('Failed to update user roles', 'Error');
          }
        });
      }
    });
  }

  getPagesArray(): number[] {
    return Array.from({ length: this.usersResult.totalPages }, (_, i) => i + 1);
  }

  isUserLocked(user: UserResponse): boolean {
    if (!user.lockoutEnd) return false;
    const lockoutEnd = new Date(user.lockoutEnd);
    return lockoutEnd > new Date();
  }

  isSuperAdmin(user: UserResponse): boolean {
    return user.roles.includes('SuperAdmin');
  }
}
