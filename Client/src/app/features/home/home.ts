import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HeaderComponent } from '../../shared/components/header/header';
import { HeroComponent } from './components/hero/hero';
import { CategoriesComponent } from './components/categories/categories';
import { FeaturedCoursesComponent } from './components/featured-courses/featured-courses';
import { WhyUsComponent } from './components/why-us/why-us';
import { FaqComponent } from './components/faq/faq';
import { FooterComponent } from '../../shared/components/footer/footer';

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

  categories = [
    { name: 'Programming', icon: 'fa-solid fa-code', count: '1,200+ Courses' },
    { name: 'UI/UX Design', icon: 'fa-solid fa-palette', count: '850+ Courses' },
    { name: 'Marketing', icon: 'fa-solid fa-chart-line', count: '540+ Courses' },
    { name: 'AI & Data Science', icon: 'fa-solid fa-robot', count: '420+ Courses' },
    { name: 'Business', icon: 'fa-solid fa-briefcase', count: '960+ Courses' }
  ];

  featuredCourses = [
    {
      image: 'https://images.unsplash.com/photo-1587620962725-abab7fe55159?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80',
      tag: 'Development',
      title: 'Mastering Full-Stack Web Development',
      instructor: 'Alex Johnson',
      rating: 4.9,
      reviews: 2450,
      price: 89.99,
      originalPrice: 129.99
    },
    {
      image: 'https://images.unsplash.com/photo-1586717791821-3f44a563eb4c?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80',
      tag: 'Design',
      title: 'UI/UX Design Principles & Figma Mastery',
      instructor: 'Sarah Miller',
      rating: 4.8,
      reviews: 1820,
      price: 74.99,
      originalPrice: 99.99
    },
    {
      image: 'https://images.unsplash.com/photo-1551288049-bebda4e38f71?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80',
      tag: 'Data Science',
      title: 'Python for Data Science and Machine Learning',
      instructor: 'Dr. Michael Chen',
      rating: 4.9,
      reviews: 3100,
      price: 94.99,
      originalPrice: 149.99
    },
    {
      image: 'https://images.unsplash.com/photo-1460925895917-afdab827c52f?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80',
      tag: 'Business',
      title: 'Digital Marketing Strategy 2024',
      instructor: 'Emma Wilson',
      rating: 4.7,
      reviews: 1240,
      price: 59.99,
      originalPrice: 79.99
    }
  ];

  faqs = [
    { question: 'Is there a free trial?', answer: 'Yes, you can sample the first few lessons of any course for free to see if it matches your learning style.', isOpen: false },
    { question: 'Do I get a certificate of completion?', answer: 'Absolutely! Every course includes a verified certificate of completion that you can share on LinkedIn or with employers.', isOpen: false },
    { question: 'Can I download the lessons?', answer: 'Yes, our mobile app allows you to download lessons for offline viewing anytime, anywhere.', isOpen: false },
    { question: 'What is the lifetime access policy?', answer: 'Once you enroll in a course, you have permanent access to all its content and future updates at no extra cost.', isOpen: false }
  ];

  ngOnInit() {
    this.startSlider();
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark') {
      this.isDarkMode = true;
      document.body.classList.add('dark');
    }
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
