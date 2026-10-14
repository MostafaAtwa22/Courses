import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthorizationService, RoleResponseDto, PermissionRoleDto, PermissionCheckboxDto } from '../services/authorization.service';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../auth/services/auth.service';
import { Module, CRUD, buildPermission } from '../../../shared/constants/permissions.constant';

@Component({
  selector: 'app-permissions-matrix',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './permissions-matrix.component.html',
  styleUrl: './permissions-matrix.component.scss'
})
export class PermissionsMatrixComponent implements OnInit {
  private authorizationService = inject(AuthorizationService);
  private toastr = inject(ToastrService);
  private authService = inject(AuthService);

  roles: RoleResponseDto[] = [];
  selectedRoleId: string = '';
  currentPermissions: PermissionRoleDto | null = null;
  isLoading = false;
  isSaving = false;
  canUpdatePermissions = false;

  // Define modules based on backend enum with unique icons
  modules = [
    { name: 'Role', displayName: 'Roles', icon: 'fa-solid fa-user-tag' },
    { name: 'User', displayName: 'Users', icon: 'fa-solid fa-users' },
    { name: 'Admin', displayName: 'Admins', icon: 'fa-solid fa-user-shield' },
    { name: 'Student', displayName: 'Students', icon: 'fa-solid fa-user-graduate' },
    { name: 'Instructor', displayName: 'Instructors', icon: 'fa-solid fa-chalkboard-user' },
    { name: 'Category', displayName: 'Categories', icon: 'fa-solid fa-tags' }
  ];

  // Define CRUD actions
  actions = ['Create', 'Read', 'Update', 'Delete'];

  ngOnInit() {
    // Check if user has permission to update roles/permissions
    this.canUpdatePermissions = this.authService.hasPermission(buildPermission(Module.Role, CRUD.Update));
    this.loadRoles();
  }

  loadRoles() {
    this.isLoading = true;
    this.authorizationService.getRoles().subscribe({
      next: (roles) => {
        // Filter to only show Admin and SuperAdmin roles
        this.roles = roles.filter(role =>
          role.name === 'Admin' || role.name === 'SuperAdmin'
        );
        if (this.roles.length > 0) {
          this.selectedRoleId = this.roles[0].id;
          this.loadPermissions();
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading roles:', error);
        this.toastr.error('Failed to load roles');
        this.isLoading = false;
      }
    });
  }

  onRoleChange() {
    if (this.selectedRoleId) {
      this.loadPermissions();
    }
  }

  loadPermissions() {
    if (!this.selectedRoleId) return;

    this.isLoading = true;
    this.authorizationService.getRolePermissions(this.selectedRoleId).subscribe({
      next: (permissions) => {
        this.currentPermissions = permissions;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading permissions:', error);
        this.toastr.error('Failed to load permissions');
        this.isLoading = false;
      }
    });
  }

  getPermissionForModuleAction(moduleName: string, action: string): PermissionCheckboxDto | undefined {
    if (!this.currentPermissions) return undefined;
    return this.currentPermissions.permissions.find(
      p => p.module === moduleName && p.action === action
    );
  }

  onPermissionChange(permission: PermissionCheckboxDto) {
    permission.isSelected = !permission.isSelected;
  }

  savePermissions() {
    if (!this.currentPermissions || !this.selectedRoleId) return;

    this.isSaving = true;
    this.authorizationService.updateRolePermissions(this.selectedRoleId, this.currentPermissions.permissions).subscribe({
      next: () => {
        this.toastr.success('Permissions updated successfully');
        this.isSaving = false;
      },
      error: (error) => {
        console.error('Error saving permissions:', error);
        this.toastr.error('Failed to save permissions');
        this.isSaving = false;
      }
    });
  }

  refreshPermissions() {
    this.loadPermissions();
  }
}
