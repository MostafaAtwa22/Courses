import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../features/auth/services/auth.service';

export function permissionGuard(permission: string | string[]): CanActivateFn {
  return (route, state) => {
    const authService = inject(AuthService);
    const router = inject(Router);

    const hasPermission = Array.isArray(permission)
      ? authService.hasAnyPermission(permission)
      : authService.hasPermission(permission);

    if (hasPermission) {
      return true;
    }

    // Redirect to admin overview if permission denied
    router.navigate(['/admin/dashboard/overview']);
    return false;
  };
}
