import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { InstructorService } from '../services/instructor.service';
import { InstructorPublicResponse } from '../models/instructor.models';
import { DecimalPipe } from '../../../shared/pipes/decimal.pipe';
import { HeaderComponent } from '../../../shared/components/header/header';
import { FooterComponent } from '../../../shared/components/footer/footer';
import { ThemeService } from '../../../core/services/theme.service';

@Component({
  selector: 'app-instructor-public-profile',
  standalone: true,
  imports: [CommonModule, DecimalPipe, HeaderComponent, FooterComponent],
  templateUrl: './public-profile.component.html',
  styleUrl: './public-profile.component.scss'
})
export class InstructorPublicProfileComponent implements OnInit {
  instructor?: InstructorPublicResponse;
  isLoading = true;
  error: string | null = null;
  isBioExpanded = false;
  private themeService = inject(ThemeService);
  isDarkMode = this.themeService.isDarkModeSignal();

  constructor(
    private route: ActivatedRoute,
    public router: Router,
    private instructorService: InstructorService
  ) {}

  ngOnInit() {
    const instructorId = this.route.snapshot.paramMap.get('id');
    if (instructorId) {
      this.loadInstructor(instructorId);
    } else {
      this.error = 'Instructor ID not provided';
      this.isLoading = false;
    }
  }

  private loadInstructor(id: string) {
    this.instructorService.getPublicInstructorById(id).subscribe({
      next: (data) => {
        this.instructor = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load instructor:', err);
        this.error = 'Failed to load instructor profile';
        this.isLoading = false;
      }
    });
  }

  get instructorName(): string {
    return this.instructor ? `${this.instructor.firstName} ${this.instructor.lastName}` : 'Instructor';
  }

  get instructorProfilePicture(): string {
    if (this.instructor?.profilePicture) {
      return this.instructor.profilePicture;
    }
    return `https://ui-avatars.com/api/?name=${encodeURIComponent(this.instructorName)}&background=random&size=200`;
  }

  get formattedStats() {
    if (!this.instructor) return null;
    return {
      rating: this.instructor.averageRate.toFixed(1),
      reviews: this.instructor.totalReviews.toLocaleString(),
      students: this.instructor.totalStudents.toLocaleString(),
      courses: this.instructor.totalCourses.toLocaleString()
    };
  }

  toggleTheme() {
    this.themeService.toggleTheme();
  }
}
