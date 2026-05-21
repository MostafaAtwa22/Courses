import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { Role } from '../../../shared/models/identity.models';

@Component({
  selector: 'app-role-select',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './role-select.component.html',
  styleUrl: './role-select.component.scss'
})
export class RoleSelectComponent {
  Role = Role;
  selectedRole: Role | null = null;

  constructor(private router: Router) {}

  selectRole(role: Role) {
    this.selectedRole = role;
  }

  continue() {
    if (!this.selectedRole) return;
    localStorage.setItem('selected_role', this.selectedRole);
    this.router.navigate(['/auth/register']);
  }
}
