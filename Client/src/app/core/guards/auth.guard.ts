import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../features/auth/services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn()) {
    // Check if user needs to select a role
    const user = authService.currentUser();
    if (user && user.roles.length > 1) {
      const selectedRole = authService.getSelectedRole();
      if (!selectedRole || !user.roles.includes(selectedRole)) {
        // No valid role selected, redirect to role selection page
        if (state.url !== '/auth/role-selection-after-login') {
          router.navigate(['/auth/role-selection-after-login']);
          return false;
        }
      }
    }
    return true;
  }

  router.navigate(['/auth/login'], { queryParams: { returnUrl: state.url } });
  return false;
};
