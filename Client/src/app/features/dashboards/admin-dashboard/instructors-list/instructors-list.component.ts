import { Component, inject, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { InstructorService } from '../../../instructors/services/instructor.service';
import { InstructorPrivateResponse } from '../../../instructors/models/instructor.models';
import { PaginatedResultModel } from '../../../../shared/models/paginated-result.model';
import { InstructorQueryParams, createInstructorQueryParams } from '../../../../shared/models/query-params.model';
import { StatCardComponent } from '../../../../shared/components/stat-card/stat-card';
import { DashboardMetric } from '../../../dashboards/models/dashboard.model';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-instructors-list',
  standalone: true,
  imports: [CommonModule, FormsModule, StatCardComponent],
  templateUrl: './instructors-list.component.html',
  styleUrl: './instructors-list.component.scss'
})
export class InstructorsListComponent implements OnInit {
  private instructorService = inject(InstructorService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private http = inject(HttpClient);

  instructorsResult: PaginatedResultModel<InstructorPrivateResponse> = new PaginatedResultModel<InstructorPrivateResponse>();
  instructorStatusMetrics: DashboardMetric[] = [];
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
    // Check for status query parameter
    this.statusFilter = this.route.snapshot.queryParamMap.get('status') || '';
    this.loadInstructors();
    this.loadInstructorStatusMetrics();
  }

  loadInstructorStatusMetrics() {
    this.http.get(`${environment.apiUrl}/instructors/admin/all`, {
      params: { pageNumber: '1', pageSize: '10000' }
    }).subscribe({
      next: (response: any) => {
        const instructors = response.items || [];
        const verified = instructors.filter((i: any) => i.status === 'Verfied').length;
        const pending = instructors.filter((i: any) => i.status === 'Pending').length;
        const unverified = instructors.filter((i: any) => i.status === 'Unverfied').length;

        this.instructorStatusMetrics = [
          {
            id: 'iss1',
            title: 'Verified',
            value: verified,
            change: '+0%',
            isPositive: true,
            icon: 'fa-solid fa-circle-check',
            color: 'success',
            description: 'approved instructors'
          },
          {
            id: 'iss2',
            title: 'Pending',
            value: pending,
            change: pending > 0 ? 'Action Required' : '+0%',
            isPositive: pending === 0,
            icon: 'fa-solid fa-clock',
            color: 'warning',
            description: 'awaiting verification'
          },
          {
            id: 'iss3',
            title: 'Unverified',
            value: unverified,
            change: '+0%',
            isPositive: false,
            icon: 'fa-solid fa-circle-xmark',
            color: 'danger',
            description: 'rejected instructors'
          }
        ];
      },
      error: (error) => {
        console.error('Failed to load instructor status metrics:', error);
        this.instructorStatusMetrics = [];
      }
    });
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
        this.loadInstructorStatusMetrics();
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
