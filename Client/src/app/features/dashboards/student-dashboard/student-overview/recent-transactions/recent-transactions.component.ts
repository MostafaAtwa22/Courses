import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Transaction } from '../models';

@Component({
  selector: 'app-recent-transactions',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './recent-transactions.component.html',
  styleUrl: './recent-transactions.component.scss'
})
export class RecentTransactionsComponent {
  @Input() transactions: Transaction[] = [];

  getTransactionStatusClass(status: string): string {
    switch (status) {
      case 'completed': return 'status-completed';
      case 'pending': return 'status-pending';
      case 'failed': return 'status-failed';
      default: return '';
    }
  }

  getTransactionStatusLabel(status: string): string {
    switch (status) {
      case 'completed': return 'Completed';
      case 'pending': return 'Pending';
      case 'failed': return 'Failed';
      default: return status;
    }
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric' 
    });
  }
}
