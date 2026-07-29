import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  constructor(private toastr: ToastrService) {}

  success(message: string, title?: string) {
    this.toastr.success(message, title, {
      positionClass: 'toast-top-right',
      timeOut: 3000,
      extendedTimeOut: 1000,
      closeButton: true,
      progressBar: true,
      enableHtml: true,
      toastClass: 'custom-toast toast-success',
      titleClass: 'toast-title',
      messageClass: 'toast-message'
    });
  }

  error(message: string, title?: string) {
    this.toastr.error(message, title, {
      positionClass: 'toast-top-right',
      timeOut: 5000,
      extendedTimeOut: 2000,
      closeButton: true,
      progressBar: true,
      enableHtml: true,
      toastClass: 'custom-toast toast-error',
      titleClass: 'toast-title',
      messageClass: 'toast-message'
    });
  }

  warning(message: string, title?: string) {
    this.toastr.warning(message, title, {
      positionClass: 'toast-top-right',
      timeOut: 4000,
      extendedTimeOut: 1500,
      closeButton: true,
      progressBar: true,
      enableHtml: true,
      toastClass: 'custom-toast toast-warning',
      titleClass: 'toast-title',
      messageClass: 'toast-message'
    });
  }

  info(message: string, title?: string) {
    this.toastr.info(message, title, {
      positionClass: 'toast-top-right',
      timeOut: 3000,
      extendedTimeOut: 1000,
      closeButton: true,
      progressBar: true,
      enableHtml: true,
      toastClass: 'custom-toast toast-info',
      titleClass: 'toast-title',
      messageClass: 'toast-message'
    });
  }

  show(message: string, title?: string, options?: any) {
    this.toastr.show(message, title, {
      positionClass: 'toast-top-right',
      timeOut: 3000,
      extendedTimeOut: 1000,
      closeButton: true,
      progressBar: true,
      enableHtml: true,
      toastClass: 'custom-toast',
      titleClass: 'toast-title',
      messageClass: 'toast-message',
      ...options
    });
  }

  clear(toastId?: number) {
    if (toastId) {
      this.toastr.clear(toastId);
    } else {
      this.toastr.clear();
    }
  }
}
