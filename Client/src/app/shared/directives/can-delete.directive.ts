import { Directive, inject, Input, TemplateRef, ViewContainerRef } from '@angular/core';
import { AuthService } from '../../features/auth/services/auth.service';
import { Module, buildPermission, CRUD } from '../constants/permissions.constant';

@Directive({
  selector: '[canDelete]',
  standalone: true
})
export class CanDeleteDirective {
  private authService = inject(AuthService);

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef
  ) {}

  @Input() set canDelete(module: Module) {
    const permission = buildPermission(module, CRUD.Delete);
    const hasPermission = this.authService.hasPermission(permission);

    if (hasPermission) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainer.clear();
    }
  }
}
