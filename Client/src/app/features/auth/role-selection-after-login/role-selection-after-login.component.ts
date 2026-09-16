import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { Role } from '../../../shared/models/identity.models';
import { AuthService } from '../services/auth.service';
import { SessionService } from '../services/session.service';

@Component({
  selector: 'app-role-selection-after-login',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './role-selection-after-login.component.html',
  styleUrl: './role-selection-after-login.component.scss'
})
export class RoleSelectionAfterLoginComponent implements OnInit {
  Role = Role;
  selectedRole: Role | null = null;
  availableRoles: Role[] = [];

  private authService = inject(AuthService);
  private sessionService = inject(SessionService);
  private router = inject(Router);

  ngOnInit() {
    const user = this.sessionService.currentUser();
    if (!user || user.roles.length === 0) {
      this.router.navigate(['/auth/login']);
      return;
    }

    // Filter valid roles and convert to Role enum
    this.availableRoles = user.roles
      .filter(role => Object.values(Role).includes(role as Role))
      .map(role => role as Role);

    // If only one role, auto-select and redirect
    if (this.availableRoles.length === 1) {
      this.sessionService.setSelectedRole(this.availableRoles[0]);
      this.router.navigate(['/']);
      return;
    }
  }

  selectRole(role: Role) {
    this.selectedRole = role;
  }

  continue() {
    if (!this.selectedRole) return;
    this.sessionService.setSelectedRole(this.selectedRole);
    this.router.navigate(['/']);
  }

  getRoleLabel(role: Role): string {
    switch (role) {
      case Role.Student:
        return 'Student';
      case Role.Instructor:
        return 'Instructor';
      case Role.Admin:
        return 'Admin';
      case Role.SuperAdmin:
        return 'Super Admin';
      default:
        return role;
    }
  }

  getRoleDescription(role: Role): string {
    switch (role) {
      case Role.Student:
        return 'Browse courses, learn new skills, and earn certificates.';
      case Role.Instructor:
        return 'Create courses, share knowledge, and grow your audience.';
      case Role.Admin:
        return 'Manage users, courses, and platform settings.';
      case Role.SuperAdmin:
        return 'Full platform administration and control.';
      default:
        return '';
    }
  }

  getRoleIcon(role: Role): string {
    switch (role) {
      case Role.Student:
        return 'fa-graduation-cap';
      case Role.Instructor:
        return 'fa-chalkboard-user';
      case Role.Admin:
        return 'fa-chart-line';
      case Role.SuperAdmin:
        return 'fa-shield-halved';
      default:
        return 'fa-user';
    }
  }
}
