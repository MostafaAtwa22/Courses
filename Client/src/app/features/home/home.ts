import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HeaderComponent } from '../../shared/components/header/header';
import { HeroComponent } from './components/hero/hero';
import { CategoriesComponent } from './components/categories/categories';
import { FeaturedCoursesComponent } from './components/featured-courses/featured-courses';
import { WhyUsComponent } from './components/why-us/why-us';
import { FaqComponent } from './components/faq/faq';
import { FooterComponent } from '../../shared/components/footer/footer';
import { CategoryService } from '../categories/services/category.service';
import { QueryParams } from '../../shared/models/query-params.model';

import { CourseService } from '../courses/services/course.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    HeaderComponent,
    HeroComponent,
    CategoriesComponent,
    FeaturedCoursesComponent,
    WhyUsComponent,
    FaqComponent,
    FooterComponent
  ],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home implements OnInit, OnDestroy {
  private categoryService = inject(CategoryService);
  private courseService = inject(CourseService);
  isDarkMode = false;
  currentSlide = 0;
  slideInterval: any;

  slides = [
    {
      image: 'assets/images/hero1.png',
      title: 'Learn Skills That Shape Your Future',
      description: 'Master the most in-demand skills in programming, design, and business with expert-led courses.',
      cta: 'Browse Courses'
    },
    {
      image: 'assets/images/hero2.png',
      title: 'Expert Instructors, Lifetime Access',
      description: 'Join over 5 million students learning from the best industry experts around the globe.',
      cta: 'Start Learning'
    },
    {
      image: 'assets/images/hero3.png',
      title: 'Get Certified, Level Up Your Career',
      description: 'Complete professional paths and earn certificates recognized by top companies.',
      cta: 'View Paths'
    }
  ];

  categories: any[] = [];
  featuredCourses: any[] = [];

  faqs = [
    { question: 'Is there a free trial?', answer: 'Yes, you can sample the first few lessons of any course for free to see if it matches your learning style.', isOpen: false },
    { question: 'Do I get a certificate of completion?', answer: 'Absolutely! Every course includes a verified certificate of completion that you can share on LinkedIn or with employers.', isOpen: false },
    { question: 'Can I download the lessons?', answer: 'Yes, our mobile app allows you to download lessons for offline viewing anytime, anywhere.', isOpen: false },
    { question: 'What is the lifetime access policy?', answer: 'Once you enroll in a course, you have permanent access to all its content and future updates at no extra cost.', isOpen: false }
  ];

  ngOnInit() {
    this.startSlider();
    this.loadCategories();
    this.loadFeaturedCourses();
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark') {
      this.isDarkMode = true;
      document.body.classList.add('dark');
    }
  }

  loadFeaturedCourses() {
    const params: QueryParams = { pageSize: 10, pageNumber: 1 };
    this.courseService.getAll(params).subscribe(res => {
      this.featuredCourses = res.items;
    });
  }

  loadCategories() {
    const params: QueryParams = { pageSize: 12, pageNumber: 1 };
    this.categoryService.getAll(params).subscribe(res => {
      this.categories = res.items.map(cat => ({
        ...cat,
        icon: this.getCategoryIcon(cat.name),
        countText: `${cat.numberOfCourses}+ Courses`
      }));
    });
  }

  getCategoryIcon(name: string): string {
    const icons: { [key: string]: string } = {
      'Programming': 'fa-solid fa-code',
      'UI/UX Design': 'fa-solid fa-palette',
      'Marketing': 'fa-solid fa-chart-line',
      'AI & Data Science': 'fa-solid fa-robot',
      'Business': 'fa-solid fa-briefcase',
      'Development': 'fa-solid fa-laptop-code',
      'Design': 'fa-solid fa-pen-nib',
      'Data Science': 'fa-solid fa-database',
      'Mobile Development': 'fa-solid fa-mobile-screen'
    };
    return icons[name] || 'fa-solid fa-graduation-cap';
  }

  ngOnDestroy() {
    if (this.slideInterval) {
      clearInterval(this.slideInterval);
    }
  }

  startSlider() {
    this.slideInterval = setInterval(() => {
      this.nextSlide();
    }, 3000);
  }

  nextSlide() {
    this.currentSlide = (this.currentSlide + 1) % this.slides.length;
  }

  prevSlide() {
    this.currentSlide = (this.currentSlide - 1 + this.slides.length) % this.slides.length;
    clearInterval(this.slideInterval);
    this.startSlider();
  }

  setSlide(index: number) {
    this.currentSlide = index;
    clearInterval(this.slideInterval);
    this.startSlider();
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

  toggleFaq(index: number) {
    this.faqs[index].isOpen = !this.faqs[index].isOpen;
  }
}
