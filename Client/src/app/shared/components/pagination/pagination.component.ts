import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.scss'
})
export class PaginationComponent {
  @Input() currentPage: number = 1;
  @Input() totalPages: number = 1;
  @Input() totalItems: number = 0;
  @Input() pageSize: number = 10;
  @Input() hasPreviousPage: boolean = false;
  @Input() hasNextPage: boolean = false;
  @Input() showInfo: boolean = true;
  @Input() maxVisiblePages: number = 5;
  @Input() variant: 'default' | 'compact' = 'default'; // For different styles
  
  @Output() pageChange = new EventEmitter<number>();

  onPageChange(page: number) {
    if (page < 1 || page > this.totalPages || page === this.currentPage) {
      return;
    }
    this.pageChange.emit(page);
  }

  get itemsRange(): string {
    if (this.totalItems === 0) return '0-0';
    const start = (this.currentPage - 1) * this.pageSize + 1;
    const end = Math.min(this.currentPage * this.pageSize, this.totalItems);
    return `${start}-${end}`;
  }

  getPageNumbers(): number[] {
    const pages: number[] = [];
    
    if (this.totalPages <= this.maxVisiblePages) {
      // Show all pages if total is less than max visible
      for (let i = 1; i <= this.totalPages; i++) {
        pages.push(i);
      }
    } else {
      // Show ellipsis logic
      if (this.currentPage <= 3) {
        // Near the start
        for (let i = 1; i <= 4; i++) {
          pages.push(i);
        }
        pages.push(-1); // Ellipsis
        pages.push(this.totalPages);
      } else if (this.currentPage >= this.totalPages - 2) {
        // Near the end
        pages.push(1);
        pages.push(-1); // Ellipsis
        for (let i = this.totalPages - 3; i <= this.totalPages; i++) {
          pages.push(i);
        }
      } else {
        // In the middle
        pages.push(1);
        pages.push(-1); // Ellipsis
        pages.push(this.currentPage - 1);
        pages.push(this.currentPage);
        pages.push(this.currentPage + 1);
        pages.push(-1); // Ellipsis
        pages.push(this.totalPages);
      }
    }
    
    return pages;
  }

  get isPreviousDisabled(): boolean {
    return this.currentPage === 1 || !this.hasPreviousPage;
  }

  get isNextDisabled(): boolean {
    return this.currentPage === this.totalPages || !this.hasNextPage;
  }
}
