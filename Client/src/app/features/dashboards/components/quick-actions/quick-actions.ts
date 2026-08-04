import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { QuickAction } from '../../models/dashboard.model';

@Component({
  selector: 'app-quick-actions',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './quick-actions.html',
  styleUrl: './quick-actions.scss'
})
export class QuickActionsComponent {
  @Input() actions: QuickAction[] = [];
  @Output() actionTriggered = new EventEmitter<string>();

  onActionClick(key: string) {
    this.actionTriggered.emit(key);
  }
}
