import { Component, inject, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { StudentService } from '../../../students/services/student.service';
import { StudentResponse, StudentQueryParams } from '../../../students/models/student.models';
import { PaginatedResultModel } from '../../../../shared/models/paginated-result.model';

@Component({
  selector: 'app-students-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './students-list.component.html',
  styleUrl: './students-list.component.scss'
})
export class StudentsListComponent implements OnInit {
  private studentService = inject(StudentService);
  private router = inject(Router);

  studentsResult: PaginatedResultModel<StudentResponse> = new PaginatedResultModel<StudentResponse>();
  params: StudentQueryParams = { pageNumber: 1, pageSize: 10 };
  searchQuery = '';
  genderFilter = '';
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
    this.loadStudents();
  }

  toggleFilterDropdown() {
    this.isFilterDropdownOpen = !this.isFilterDropdownOpen;
  }

  loadStudents() {
    const params: StudentQueryParams = {
      pageNumber: this.params.pageNumber,
      pageSize: this.params.pageSize,
      sortDescending: this.sortDescending
    };

    if (this.searchQuery) {
      params.searchTerm = this.searchQuery;
    }
    
    if (this.genderFilter) {
      params.gender = this.genderFilter;
    }
    
    if (this.sortBy) {
      params.sortBy = this.sortBy;
    }
    
    this.studentService.getAllStudents(params).subscribe({
      next: (res: PaginatedResultModel<StudentResponse>) => {
        this.studentsResult = res;
      }
    });
  }

  onSearch() {
    this.params.pageNumber = 1;
    this.loadStudents();
  }

  clearFilters() {
    this.searchQuery = '';
    this.genderFilter = '';
    this.sortBy = '';
    this.sortDescending = false;
    this.onSearch();
    this.isFilterDropdownOpen = false;
  }

  onPageChange(page: number) {
    if (page < 1 || page > this.studentsResult.totalPages) return;
    this.params.pageNumber = page;
    this.loadStudents();
  }

  deleteStudent(studentId: string) {
    if (confirm('Are you sure you want to delete this student?')) {
      this.studentService.deleteStudent(studentId).subscribe({
        next: () => {
          this.loadStudents();
        }
      });
    }
  }

  getPagesArray(): number[] {
    return Array.from({ length: this.studentsResult.totalPages }, (_, i) => i + 1);
  }

  viewStudentDetails(studentId: string) {
    this.router.navigate(['/admin/dashboard/students', studentId]);
  }
}
