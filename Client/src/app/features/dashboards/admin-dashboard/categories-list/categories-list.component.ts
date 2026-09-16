import { Component, inject, OnInit, HostListener, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CategoryService } from '../../../categories/services/category.service';
import { CategoryResponse } from '../../../categories/models/category.models';
import { PaginatedResultModel } from '../../../../shared/models/paginated-result.model';
import { QueryParams } from '../../../../shared/models/query-params.model';
import Swal from 'sweetalert2';
import { ToastrService } from 'ngx-toastr';
import { CategoryModalComponent } from './category-modal/category-modal.component';

@Component({
  selector: 'app-categories-list',
  standalone: true,
  imports: [CommonModule, FormsModule, CategoryModalComponent],
  templateUrl: './categories-list.component.html',
  styleUrl: './categories-list.component.scss'
})
export class CategoriesListComponent implements OnInit {
  private categoryService = inject(CategoryService);
  private toastr = inject(ToastrService);
  private cdr = inject(ChangeDetectorRef);

  categoriesResult: PaginatedResultModel<CategoryResponse> = new PaginatedResultModel<CategoryResponse>();
  params: QueryParams = { pageNumber: 1, pageSize: 10 };
  searchQuery = '';
  sortBy = 'name';
  sortDescending = false;
  isFilterDropdownOpen = false;
  isModalOpen = false;
  editingCategory: CategoryResponse | null = null;

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    const target = event.target as HTMLElement;
    const dropdown = target.closest('.dropdown');
    
    if (!dropdown && this.isFilterDropdownOpen) {
      this.isFilterDropdownOpen = false;
    }
  }

  ngOnInit() {
    this.loadCategories();
  }

  toggleFilterDropdown() {
    this.isFilterDropdownOpen = !this.isFilterDropdownOpen;
  }

  loadCategories() {
    const params: QueryParams = {
      pageNumber: this.params.pageNumber,
      pageSize: this.params.pageSize,
      sortDescending: this.sortDescending
    };

    if (this.searchQuery) {
      params.searchTerm = this.searchQuery;
    }
    
    if (this.sortBy) {
      params.sortBy = this.sortBy;
    }
    
    this.categoryService.getAll(params).subscribe({
      next: (res: PaginatedResultModel<CategoryResponse>) => {
        this.categoriesResult = res;
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.toastr.error('Failed to load categories', 'Error');
      }
    });
  }

  onSearch() {
    this.params.pageNumber = 1;
    this.loadCategories();
  }

  clearFilters() {
    this.searchQuery = '';
    this.sortBy = '';
    this.sortDescending = false;
    this.onSearch();
    this.isFilterDropdownOpen = false;
  }

  onPageChange(page: number) {
    if (page < 1 || page > this.categoriesResult.totalPages) return;
    this.params.pageNumber = page;
    this.loadCategories();
  }

  openCreateModal() {
    this.editingCategory = null;
    this.isModalOpen = true;
  }

  openEditModal(category: CategoryResponse) {
    this.editingCategory = category;
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
    this.editingCategory = null;
  }

  onModalSubmit() {
    this.closeModal();
    // Use setTimeout to ensure modal is fully closed before reloading data
    setTimeout(() => {
      this.loadCategories();
    }, 100);
  }

  deleteCategory(categoryId: string, categoryName: string) {
    Swal.fire({
      title: 'Are you sure?',
      text: `You are about to delete the category "${categoryName}". This action cannot be undone.`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#dc2626',
      cancelButtonColor: '#64748b',
      confirmButtonText: 'Yes, delete it!',
      cancelButtonText: 'Cancel',
      customClass: {
        popup: 'custom-swal-popup',
        title: 'custom-swal-title',
        htmlContainer: 'custom-swal-content',
        confirmButton: 'custom-swal-confirm',
        cancelButton: 'custom-swal-cancel'
      }
    }).then((result) => {
      if (result.isConfirmed) {
        this.categoryService.delete(categoryId).subscribe({
          next: () => {
            this.toastr.success('Category deleted successfully', 'Success');
            this.loadCategories();
            this.cdr.detectChanges();
          },
          error: (error) => {
            this.toastr.error('Failed to delete category', 'Error');
          }
        });
      }
    });
  }

  getPagesArray(): number[] {
    return Array.from({ length: this.categoriesResult.totalPages }, (_, i) => i + 1);
  }
}
