import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { CourseService } from '../services/course.service';
import { CourseResponse, SectionResponse, ReviewResponse } from '../models/course.models';
import { CourseHeroComponent } from './components/course-hero/course-hero';
import { CourseSidebarComponent } from './components/course-sidebar/course-sidebar';
import { CourseContentComponent } from './components/course-content/course-content';
import { CourseInstructorComponent } from './components/course-instructor/course-instructor';
import { CourseReviewsComponent } from './components/course-reviews/course-reviews';
import { HeaderComponent } from '../../../shared/components/header/header';
import { FooterComponent } from '../../../shared/components/footer/footer';

@Component({
  selector: 'app-course-details',
  standalone: true,
  imports: [
    CommonModule,
    CourseHeroComponent,
    CourseSidebarComponent,
    CourseContentComponent,
    CourseInstructorComponent,
    CourseReviewsComponent,
    HeaderComponent,
    FooterComponent
  ],
  templateUrl: './course-details.html',
  styleUrl: './course-details.scss'
})
export class CourseDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private courseService = inject(CourseService);

  course?: CourseResponse;
  isDarkMode = false;

  learningPoints: string[] = [
    'Build enterprise-level applications with modern technologies',
    'Understand and implement advanced design patterns',
    'Optimize performance and accessibility',
    'Master complex state management strategies',
    'Deploy and scale applications to the cloud',
    'Implement robust security and authentication systems'
  ];

  requirements: string[] = [
    'Solid understanding of JavaScript/TypeScript',
    'Basic knowledge of web frameworks',
    'Experience with HTML and CSS',
    'A computer with administrative privileges'
  ];

  defaultDescription = `
    <p>This comprehensive bootcamp is designed to take you from a basic understanding of web development to being a proficient full-stack engineer. We focus on real-world applications and industry-standard practices.</p>
    <p>Throughout the course, you will build multiple projects that demonstrate your ability to handle both frontend and backend challenges, including state management, API design, and database optimization.</p>
  `;

  dummySections: SectionResponse[] = [
    {
      id: '1',
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
      title: 'Introduction and Setup',
      order: 1,
      contents: [
        { id: '1-1', createdAt: new Date().toISOString(), updatedAt: new Date().toISOString(), title: 'Course Overview', type: 0, contentUrl: '', order: 1, isPreview: true },
        { id: '1-2', createdAt: new Date().toISOString(), updatedAt: new Date().toISOString(), title: 'Installing Tools', type: 0, contentUrl: '', order: 2, isPreview: false },
        { id: '1-3', createdAt: new Date().toISOString(), updatedAt: new Date().toISOString(), title: 'Setting up the Environment', type: 1, contentUrl: '', order: 3, isPreview: false }
      ]
    },
    {
      id: '2',
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
      title: 'Deep Dive into Architecture',
      order: 2,
      contents: [
        { id: '2-1', createdAt: new Date().toISOString(), updatedAt: new Date().toISOString(), title: 'Understanding Patterns', type: 0, contentUrl: '', order: 1, isPreview: true },
        { id: '2-2', createdAt: new Date().toISOString(), updatedAt: new Date().toISOString(), title: 'Domain Driven Design', type: 0, contentUrl: '', order: 2, isPreview: false },
        { id: '2-3', createdAt: new Date().toISOString(), updatedAt: new Date().toISOString(), title: 'Clean Architecture Principles', type: 0, contentUrl: '', order: 3, isPreview: false },
        { id: '2-4', createdAt: new Date().toISOString(), updatedAt: new Date().toISOString(), title: 'Module Quiz', type: 2, contentUrl: '', order: 4, isPreview: false }
      ]
    }
  ];

  dummyReviews: ReviewResponse[] = [
    {
      id: 'r1',
      createdAt: new Date('2024-03-15').toISOString(),
      updatedAt: new Date('2024-03-15').toISOString(),
      headline: 'Excellent Course!',
      comment: 'This course is truly transformative. The instructor explains complex concepts with ease and the project work is very relevant to real-world scenarios.',
      rating: 5,
      studentName: 'Michael Chen',
      studentProfilePicture: ''
    },
    {
      id: 'r2',
      createdAt: new Date('2024-03-20').toISOString(),
      updatedAt: new Date('2024-03-20').toISOString(),
      headline: 'Highly recommended',
      comment: 'I learned more in this course than I did in my entire computer science degree. The focus on architecture is exactly what I was looking for.',
      rating: 5,
      studentName: 'Emma Watson',
      studentProfilePicture: ''
    }
  ];

  ngOnInit(): void {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark') {
      this.isDarkMode = true;
      document.body.classList.add('dark');
    }

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.courseService.getById(id).subscribe({
        next: (course) => {
          this.course = course;
          if (!this.course.sections || this.course.sections.length === 0) {
            this.course.sections = this.dummySections;
          }
          if (!this.course.reviews || this.course.reviews.length === 0) {
            this.course.reviews = this.dummyReviews;
          }
        },
        error: (err) => {
          console.error('Error fetching course:', err);
          // For demo, just use dummy data even if fetch fails
          this.course = {
            id: id,
            title: 'Advanced Full-Stack Web Development Bootcamp 2024',
            description: this.defaultDescription,
            pictureUrl: 'https://images.unsplash.com/photo-1498050108023-c5249f4df085',
            status: 0,
            cost: 89.99,
            studentCount: 85000,
            totalReviews: 12450,
            averageRate: 4.8,
            category: 'Development',
            instructorName: 'Prof. Sarah Jenkins',
            instructorProfilePicture: '',
            instructorTitle: 'Expert Web Developer',
            sections: this.dummySections,
            reviews: this.dummyReviews
          } as CourseResponse;
        }
      });
    }
  }

  toggleTheme() {
    this.isDarkMode = !this.isDarkMode;
    if (this.isDarkMode) {
      document.body.classList.add('dark');
      localStorage.setItem('theme', 'dark');
    } else {
      document.body.classList.remove('dark');
      localStorage.setItem('theme', 'light');
    }
  }
}
