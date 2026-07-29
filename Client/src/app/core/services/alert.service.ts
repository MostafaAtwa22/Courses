import { Injectable } from '@angular/core';
import Swal, { SweetAlertOptions, SweetAlertResult } from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class AlertService {
  constructor() {
    // Set default SweetAlert2 options to match website theme
    Swal.mixin({
      customClass: {
        popup: 'custom-swal-popup',
        title: 'custom-swal-title',
        htmlContainer: 'custom-swal-content',
        confirmButton: 'custom-swal-confirm',
        denyButton: 'custom-swal-deny',
        cancelButton: 'custom-swal-cancel',
        input: 'custom-swal-input'
      },
      buttonsStyling: false,
      showClass: {
        popup: 'swal2-show',
        backdrop: 'swal2-backdrop-show'
      },
      hideClass: {
        popup: 'swal2-hide',
        backdrop: 'swal2-backdrop-hide'
      }
    });
  }

  success(title: string, text?: string): Promise<SweetAlertResult> {
    return Swal.fire({
      icon: 'success',
      title: title,
      text: text,
      confirmButtonText: 'OK',
      customClass: {
        confirmButton: 'custom-swal-confirm'
      }
    });
  }

  error(title: string, text?: string): Promise<SweetAlertResult> {
    return Swal.fire({
      icon: 'error',
      title: title,
      text: text,
      confirmButtonText: 'OK',
      customClass: {
        confirmButton: 'custom-swal-deny'
      }
    });
  }

  warning(title: string, text?: string): Promise<SweetAlertResult> {
    return Swal.fire({
      icon: 'warning',
      title: title,
      text: text,
      confirmButtonText: 'OK',
      customClass: {
        confirmButton: 'custom-swal-confirm'
      }
    });
  }

  info(title: string, text?: string): Promise<SweetAlertResult> {
    return Swal.fire({
      icon: 'info',
      title: title,
      text: text,
      confirmButtonText: 'OK',
      customClass: {
        confirmButton: 'custom-swal-confirm'
      }
    });
  }

  confirm(title: string, text?: string, confirmText: string = 'Yes', cancelText: string = 'No'): Promise<SweetAlertResult> {
    return Swal.fire({
      icon: 'question',
      title: title,
      text: text,
      showCancelButton: true,
      confirmButtonText: confirmText,
      cancelButtonText: cancelText,
      customClass: {
        confirmButton: 'custom-swal-confirm',
        cancelButton: 'custom-swal-cancel'
      },
      reverseButtons: true
    });
  }

  confirmDelete(title: string = 'Are you sure?', text?: string): Promise<SweetAlertResult> {
    return Swal.fire({
      icon: 'warning',
      title: title,
      text: text || 'This action cannot be undone.',
      showCancelButton: true,
      confirmButtonText: 'Delete',
      cancelButtonText: 'Cancel',
      customClass: {
        confirmButton: 'custom-swal-deny',
        cancelButton: 'custom-swal-cancel'
      },
      reverseButtons: true
    });
  }

  input(title: string, text?: string, options?: SweetAlertOptions): Promise<SweetAlertResult> {
    return Swal.fire({
      title: title,
      text: text,
      input: 'text',
      inputPlaceholder: 'Enter value...',
      showCancelButton: true,
      confirmButtonText: 'Submit',
      cancelButtonText: 'Cancel',
      customClass: {
        confirmButton: 'custom-swal-confirm',
        cancelButton: 'custom-swal-cancel'
      },
      ...options
    });
  }

  custom(options: SweetAlertOptions): Promise<SweetAlertResult> {
    return Swal.fire(options);
  }

  close() {
    Swal.close();
  }
}
