import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InstructorService } from '../../instructors/services/instructor.service';
import { InstructorPrivateResponse } from '../../instructors/models/instructor.models';
import { PaginatedResultModel } from '../../../shared/models/paginated-result.model';
import { QueryParams, createQueryParams } from '../../../shared/models/query-params.model';

@Component({
  selector: 'app-instructors-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './instructors-list.component.html',
  styleUrl: './instructors-list.component.scss'
})
export class InstructorsListComponent implements OnInit {
  private instructorService = inject(InstructorService);

  instructorsResult: PaginatedResultModel<InstructorPrivateResponse> = new PaginatedResultModel<InstructorPrivateResponse>();
  params: QueryParams = createQueryParams({ pageSize: 10 });
  searchQuery = '';

  ngOnInit() {
    this.loadInstructors();
  }

  loadInstructors() {
    this.params.searchTerm = this.searchQuery || '';
    this.instructorService.getAllInstructors(this.params).subscribe({
      next: (res: PaginatedResultModel<InstructorPrivateResponse>) => {
        this.instructorsResult = res;
      }
    });
  }

  onSearch() {
    this.params.pageNumber = 1;
    this.loadInstructors();
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
}
