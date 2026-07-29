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
  // For now, we'll check if the current route is the instructor creation page
  // If they're trying to access other pages, redirect to instructor creation
  const isInstructorCreationRoute = state.url === '/instructor/create';
  
  if (!isInstructorCreationRoute) {
    // In a real implementation, you would check if the user has an instructor profile
    // by making an API call or checking a flag in the user object
    // For now, we'll redirect to instructor creation if they don't have the profile
    // This is a simplified version - you may need to add instructorId to the user model
    // or create a service to check instructor profile status
    
    // TODO: Add actual check for instructor profile completion
    // For now, we'll assume instructors need to complete their profile
    router.navigate(['/instructor/create']);
    return false;
  }

  return true;
};
