import { Component, inject, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { InstructorService } from '../../../instructors/services/instructor.service';
import { InstructorPrivateResponse } from '../../../instructors/models/instructor.models';
import { PaginatedResultModel } from '../../../../shared/models/paginated-result.model';
import { InstructorQueryParams, createInstructorQueryParams } from '../../../../shared/models/query-params.model';

@Component({
  selector: 'app-instructors-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './instructors-list.component.html',
  styleUrl: './instructors-list.component.scss'
})
export class InstructorsListComponent implements OnInit {
  private instructorService = inject(InstructorService);
  private router = inject(Router);

  instructorsResult: PaginatedResultModel<InstructorPrivateResponse> = new PaginatedResultModel<InstructorPrivateResponse>();
  params: InstructorQueryParams = createInstructorQueryParams({ pageSize: 10 });
  searchQuery = '';
  statusFilter = '';
  sortBy = 'name';
  sortDescending = false;
  isFilterDropdownOpen = false;

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    const target = event.target as HTMLElement;
    const dropdown = target.closest('.dropdown');
    
    if (!dropdown && this.isFilterDropdownOpen) {
      this.isFilterDropdownOpen = false;
    }
  }

  ngOnInit() {
    this.loadInstructors();
  }

  toggleFilterDropdown() {
    this.isFilterDropdownOpen = !this.isFilterDropdownOpen;
    console.log('Dropdown open:', this.isFilterDropdownOpen);
  }

  loadInstructors() {
    // Build params object, only including properties with values
    const params: InstructorQueryParams = {
      pageNumber: this.params.pageNumber,
      pageSize: this.params.pageSize,
      sortDescending: this.sortDescending
    };

    // Only add optional params if they have values
    if (this.searchQuery) {
      params.searchTerm = this.searchQuery;
    }
    
    if (this.statusFilter) {
      params.status = this.statusFilter;
    }
    
    if (this.sortBy) {
      params.sortBy = this.sortBy;
    }
    
    this.instructorService.getAllInstructors(params).subscribe({
      next: (res: PaginatedResultModel<InstructorPrivateResponse>) => {
        this.instructorsResult = res;
      }
    });
  }

  onSearch() {
    this.params.pageNumber = 1;
    this.loadInstructors();
  }

  clearFilters() {
    this.searchQuery = '';
    this.statusFilter = '';
    this.sortBy = '';
    this.sortDescending = false;
    this.onSearch();
    this.isFilterDropdownOpen = false;
  }

  onPageChange(page: number) {
    if (page < 1 || page > this.instructorsResult.totalPages) return;
    this.params.pageNumber = page;
    this.loadInstructors();
  }

  changeStatus(instructorId: string, newStatus: string) {
    this.instructorService.changeInstructorStatus(instructorId, newStatus).subscribe({
      next: () => {
        this.loadInstructors();
      }
    });
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Verfied':
        return 'status-verified';
      case 'Pending':
        return 'status-pending';
      case 'Unverfied':
        return 'status-unverified';
      default:
        return '';
    }
  }

  getPagesArray(): number[] {
    return Array.from({ length: this.instructorsResult.totalPages }, (_, i) => i + 1);
  }

  viewInstructorDetails(instructorId: string) {
    this.router.navigate(['/admin/dashboard/instructors', instructorId]);
  }
}
