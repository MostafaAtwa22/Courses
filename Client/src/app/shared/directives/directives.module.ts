import { NgModule } from '@angular/core';
import { HasPermissionDirective } from './has-permission.directive';
import { CanCreateDirective } from './can-create.directive';
import { CanReadDirective } from './can-read.directive';
import { CanUpdateDirective } from './can-update.directive';
import { CanDeleteDirective } from './can-delete.directive';

const directives = [
  HasPermissionDirective,
  CanCreateDirective,
  CanReadDirective,
  CanUpdateDirective,
  CanDeleteDirective
];

@NgModule({
  imports: directives,
  exports: directives
})
export class DirectivesModule {}
