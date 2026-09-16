import { Component, Input, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CategoryService } from '../../../../categories/services/category.service';
import { CategoryResponse, CategoryCreateDto, CategoryUpdateDto } from '../../../../categories/models/category.models';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-category-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './category-modal.component.html',
  styleUrl: './category-modal.component.scss'
})
export class CategoryModalComponent {
  @Input() category: CategoryResponse | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() submit = new EventEmitter<void>();

  private categoryService = inject(CategoryService);
  private toastr = inject(ToastrService);

  formData: CategoryCreateDto | CategoryUpdateDto = {
    name: '',
    slug: ''
  };

  isSubmitting = false;

  ngOnInit() {
    if (this.category) {
      this.formData = {
        name: this.category.name,
        slug: this.category.slug
      };
    }
  }

  generateSlug() {
    if (this.formData.name) {
      this.formData.slug = this.formData.name
        .toLowerCase()
        .replace(/[^a-z0-9]+/g, '-')
        .replace(/(^-|-$)/g, '');
    }
  }

  onSubmit() {
    if (!this.formData.name || !this.formData.slug) {
      this.toastr.error('Please fill in all required fields', 'Validation Error');
      return;
    }

    this.isSubmitting = true;

    if (this.category) {
      // Update existing category
      this.categoryService.update(this.category.id, this.formData as CategoryUpdateDto).subscribe({
        next: () => {
          this.toastr.success('Category updated successfully', 'Success');
          this.submit.emit();
        },
        error: (error: any) => {
          this.toastr.error('Failed to update category', 'Error');
          this.isSubmitting = false;
        }
      });
    } else {
      // Create new category
      this.categoryService.create(this.formData as CategoryCreateDto).subscribe({
        next: () => {
          this.toastr.success('Category created successfully', 'Success');
          this.submit.emit();
        },
        error: (error: any) => {
          this.toastr.error('Failed to create category', 'Error');
          this.isSubmitting = false;
        }
      });
    }
  }

  onClose() {
    this.close.emit();
  }
}
