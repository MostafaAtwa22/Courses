import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserResponse, RolesResponse, CheckBoxRoleManage, UserRolesManage } from '../../../../admin/models/user.models';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-role-assignment-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './role-assignment-modal.component.html',
  styleUrl: './role-assignment-modal.component.scss'
})
export class RoleAssignmentModalComponent implements OnInit {
  @Output() save = new EventEmitter<{ dto: UserRolesManage, user: UserResponse }>();
  @Output() cancel = new EventEmitter<void>();

  user: UserResponse | null = null;
  availableRoles: RolesResponse[] = [];
  userRoles: UserRolesManage | null = null;
  selectedRoles: CheckBoxRoleManage[] = [];
  isVisible = false;

  ngOnInit() {
    this.initializeSelectedRoles();
  }

  open(user: UserResponse, availableRoles: RolesResponse[], userRoles: UserRolesManage) {
    this.user = user;
    this.availableRoles = availableRoles;
    this.userRoles = userRoles;
    this.initializeSelectedRoles();
    this.isVisible = true;
  }

  close() {
    this.isVisible = false;
    this.user = null;
    this.userRoles = null;
    this.selectedRoles = [];
  }

  private initializeSelectedRoles() {
    if (!this.userRoles || !this.availableRoles) return;

    const currentRoleNames = new Set(this.userRoles.roles.filter(r => r.isSelected).map(r => r.roleName));
    
    this.selectedRoles = this.availableRoles.map(role => ({
      roleId: role.id,
      roleName: role.name,
      isSelected: currentRoleNames.has(role.name),
      roleClass: this.getRoleClass(role.name)
    }));
  }

  private getRoleClass(roleName: string): string {
    const roleLower = roleName.toLowerCase();
    if (roleLower === 'superadmin') return 'role-superadmin';
    if (roleLower === 'admin') return 'role-admin';
    if (roleLower === 'student') return 'role-student';
    if (roleLower === 'instructor') return 'role-instructor';
    return 'role-default';
  }

  onRoleChange(role: CheckBoxRoleManage) {
    role.isSelected = !role.isSelected;
  }

  onSave() {
    const selectedRoles = this.selectedRoles.filter(r => r.isSelected);
    const dto: UserRolesManage = {
      roles: selectedRoles
    };

    Swal.fire({
      title: 'Confirm Role Assignment',
      text: `Are you sure you want to save the role assignments for ${this.user?.firstName} ${this.user?.lastName}?`,
      icon: 'question',
      showCancelButton: true,
      confirmButtonColor: '#4f46e5',
      cancelButtonColor: '#64748b',
      confirmButtonText: 'Yes, save it!',
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
        this.save.emit({ dto, user: this.user! });
        this.close();
      }
    });
  }

  onCancel() {
    this.cancel.emit();
    this.close();
  }
}
