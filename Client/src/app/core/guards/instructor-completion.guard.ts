import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../features/auth/services/auth.service';

export const instructorCompletionGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const currentUser = authService.currentUser();
  
  // If user is not logged in, let authGuard handle it
  if (!currentUser) {
    return true;
  }

  // If user is not an instructor, allow access
  if (!currentUser.roles.includes('Instructor')) {
    return true;
  }

  // If user is an instructor, check if they have an instructor profile
  const isInstructorCreationRoute = state.url === '/instructor/create';
  
  if (!isInstructorCreationRoute) {
    // Check from JWT token - no API call needed
    if (!authService.hasInstructorProfile()) {
      router.navigate(['/instructor/create']);
      return false;
    }
  }

  return true;
};
