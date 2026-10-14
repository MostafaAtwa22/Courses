import { Directive, inject, Input, TemplateRef, ViewContainerRef } from '@angular/core';
import { AuthService } from '../../features/auth/services/auth.service';
import { Module, buildPermission, CRUD } from '../constants/permissions.constant';

@Directive({
  selector: '[canUpdate]',
  standalone: true
})
export class CanUpdateDirective {
  private authService = inject(AuthService);

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef
  ) {}

  @Input() set canUpdate(module: Module) {
    const permission = buildPermission(module, CRUD.Update);
    const hasPermission = this.authService.hasPermission(permission);

    if (hasPermission) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainer.clear();
    }
  }
}
