import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../features/auth/services/auth.service';
import { InstructorService } from '../../features/instructors/services/instructor.service';
import { map, catchError } from 'rxjs/operators';
import { of } from 'rxjs';

export const instructorVerifiedGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const instructorService = inject(InstructorService);

  // If user is not logged in, let authGuard handle it
  if (!authService.isLoggedIn()) {
    return of(true);
  }

  // Check if user's selected role is Instructor
  const selectedRole = authService.getSelectedRole();
  if (selectedRole !== 'Instructor') {
    return of(true);
  }

  // Allow access to update-profile route even if not verified
  if (state.url.includes('/instructor/dashboard/update-profile')) {
    return of(true);
  }

  // Allow access to overview route - it will show pending/rejected status
  if (state.url.includes('/instructor/dashboard/overview')) {
    return of(true);
  }

  // Check instructor verification status via API
  return instructorService.getCurrentInstructor().pipe(
    map((instructor) => {
      if (instructor.status === 'Verfied') {
        return true;
      } else {
        // Not verified, redirect to overview which will show the pending/rejected message
        router.navigate(['/instructor/dashboard/overview']);
        return false;
      }
    }),
    catchError((error) => {
      console.error('Error checking instructor verification status:', error);
      // If we can't verify, redirect to overview for safety
      router.navigate(['/instructor/dashboard/overview']);
      return of(false);
    })
  );
};