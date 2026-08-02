import { HttpErrorResponse, HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toastr = inject(ToastrService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'An unexpected error occurred. Please try again.';

      if (error instanceof HttpErrorResponse) {
        switch (error.status) {
          case 400:
            // Handle validation errors
            if (error.error && typeof error.error === 'object') {
              if (error.error.errors) {
                // Extract validation error details
                const errors = error.error.errors;
                if (typeof errors === 'object') {
                  const errorMessages = Object.entries(errors).map(([field, messages]) => {
                    const fieldErrors = Array.isArray(messages) ? messages : [messages];
                    return `${field}: ${fieldErrors.join(', ')}`;
                  });
                  errorMessage = errorMessages.join('<br>');
                } else {
                  errorMessage = error.error.detail || error.error.message || 'Validation failed. Please check your input.';
                }
              } else if (error.error.detail) {
                errorMessage = error.error.detail;
              } else if (error.error.message) {
                errorMessage = error.error.message;
              } else {
                errorMessage = 'Validation failed. Please check your input.';
              }
            } else {
              errorMessage = error.message || 'Validation failed. Please check your input.';
            }
            toastr.error(errorMessage, 'Validation Error', {
              enableHtml: true,
              timeOut: 5000
            });
            break;

          case 404:
            errorMessage = error.error?.detail || error.error?.message || 'The requested resource was not found.';
            toastr.warning(errorMessage, 'Not Found', {
              timeOut: 4000
            });
            break;

          case 500:
            errorMessage = error.error?.detail || error.error?.message || 'A server error occurred. Please try again later.';
            toastr.error(errorMessage, 'Server Error', {
              timeOut: 5000
            });
            break;

          case 401:
            // Don't show toastr for 401 errors - let auth interceptor handle it
            break;

          case 403:
            errorMessage = error.error?.detail || error.error?.message || 'You do not have permission to perform this action.';
            toastr.error(errorMessage, 'Access Denied', {
              timeOut: 4000
            });
            break;

          case 409:
            errorMessage = error.error?.detail || error.error?.message || 'A conflict occurred with the current state of the resource.';
            toastr.warning(errorMessage, 'Conflict', {
              timeOut: 4000
            });
            break;

          default:
            errorMessage = error.error?.detail || error.error?.message || `An error occurred (${error.status}). Please try again.`;
            toastr.error(errorMessage, 'Error', {
              timeOut: 4000
            });
            break;
        }
      }

      return throwError(() => error);
    })
  );
};
