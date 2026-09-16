import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { StudentService } from '../../../../students/services/student.service';
import { StudentResponse } from '../../../../students/models/student.models';
import { ToastService } from '../../../../../core/services/toast.service';

@Component({
  selector: 'app-student-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './student-details.component.html',
  styleUrl: './student-details.component.scss'
})
export class StudentDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private studentService = inject(StudentService);
  private toastService = inject(ToastService);

  student: StudentResponse | null = null;
  studentId: string = '';

  ngOnInit() {
    this.studentId = this.route.snapshot.paramMap.get('id') || '';
    if (this.studentId) {
      this.loadStudentDetails();
    } else {
      this.router.navigate(['/admin/dashboard/students']);
    }
  }

  loadStudentDetails() {
    this.studentService.getStudentById(this.studentId).subscribe({
      next: (student: StudentResponse) => {
        this.student = student;
      },
      error: () => {
        this.toastService.error('Failed to load student details');
        this.router.navigate(['/admin/dashboard/students']);
      }
    });
  }

  goBack() {
    this.router.navigate(['/admin/dashboard/students']);
  }

  deleteStudent() {
    if (!this.student) return;
    
    if (confirm('Are you sure you want to delete this student? This action cannot be undone.')) {
      this.studentService.deleteStudent(this.student.id).subscribe({
        next: () => {
          this.toastService.success('Student deleted successfully');
          this.router.navigate(['/admin/dashboard/students']);
        },
        error: () => {
          this.toastService.error('Failed to delete student');
        }
      });
    }
  }
}
