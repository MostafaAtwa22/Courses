import { Injectable, signal } from '@angular/core';
import { Observable, of } from 'rxjs';
import {
  DashboardMetric,
  CourseAnalytics,
  AttendanceSummary,
  ScheduleItem,
  ActivityLogItem,
  QuickAction
} from '../models/dashboard.model';

export interface PendingInstructor {
  id: string;
  name: string;
  email: string;
  expertise: string;
  appliedDate: string;
  avatar: string;
}

export interface StudentSubmission {
  id: string;
  studentName: string;
  courseTitle: string;
  assignmentTitle: string;
  submittedAt: string;
  status: 'Pending' | 'Graded';
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private adminMetrics = signal<DashboardMetric[]>([
    {
      id: 'am1',
      title: 'Total Platform Users',
      value: '18,420',
      change: '+14.2%',
      isPositive: true,
      icon: 'fa-solid fa-users',
      color: 'primary',
      description: 'students & instructors'
    },
    {
      id: 'am2',
      title: 'Total Revenue',
      value: '$148,250',
      change: '+18.5%',
      isPositive: true,
      icon: 'fa-solid fa-dollar-sign',
      color: 'success',
      description: 'gross platform sales'
    },
    {
      id: 'am3',
      title: 'Pending Verification',
      value: '5',
      change: 'Action Required',
      isPositive: false,
      icon: 'fa-solid fa-user-clock',
      color: 'warning',
      description: 'instructor applications'
    },
    {
      id: 'am4',
      title: 'Active Courses',
      value: '342',
      change: '+6.1%',
      isPositive: true,
      icon: 'fa-solid fa-graduation-cap',
      color: 'purple',
      description: 'published platform courses'
    }
  ]);

  private instructorMetrics = signal<DashboardMetric[]>([
    {
      id: 'im1',
      title: 'Enrolled Students',
      value: '1,240',
      change: '+8.4%',
      isPositive: true,
      icon: 'fa-solid fa-user-graduate',
      color: 'primary',
      description: 'across 4 active courses'
    },
    {
      id: 'im2',
      title: 'Monthly Earnings',
      value: '$12,450',
      change: '+15.2%',
      isPositive: true,
      icon: 'fa-solid fa-wallet',
      color: 'success',
      description: 'payout ready'
    },
    {
      id: 'im3',
      title: 'Pending Grading',
      value: '14',
      change: '8 Urgent',
      isPositive: false,
      icon: 'fa-solid fa-file-signature',
      color: 'warning',
      description: 'assignments submitted'
    },
    {
      id: 'im4',
      title: 'Instructor Rating',
      value: '4.92 / 5',
      change: '+0.05',
      isPositive: true,
      icon: 'fa-solid fa-star',
      color: 'purple',
      description: 'from 890 reviews'
    }
  ]);

  private pendingInstructors = signal<PendingInstructor[]>([
    { id: 'pi1', name: 'Dr. Robert Vance', email: 'robert.vance@university.edu', expertise: 'Artificial Intelligence', appliedDate: '2 hours ago', avatar: 'https://i.pravatar.cc/150?img=12' },
    { id: 'pi2', name: 'Emily Watson', email: 'emily.w@designhub.io', expertise: 'UI/UX & Design Systems', appliedDate: '5 hours ago', avatar: 'https://i.pravatar.cc/150?img=24' },
    { id: 'pi3', name: 'Alexander Wright', email: 'alex.wright@tech.org', expertise: 'Cloud Architecture & DevOps', appliedDate: '1 day ago', avatar: 'https://i.pravatar.cc/150?img=33' }
  ]);

  private studentSubmissions = signal<StudentSubmission[]>([
    { id: 'sub1', studentName: 'Lucas Bennett', courseTitle: 'Advanced Web Dev', assignmentTitle: 'REST API & Microservices Project', submittedAt: '10 mins ago', status: 'Pending' },
    { id: 'sub2', studentName: 'Emma Thompson', courseTitle: 'UI/UX Design Systems', assignmentTitle: 'Figma Design System Submission', submittedAt: '45 mins ago', status: 'Pending' },
    { id: 'sub3', studentName: 'Noah Martinez', courseTitle: 'Advanced Web Dev', assignmentTitle: 'Docker & Kubernetes Deployment', submittedAt: '2 hours ago', status: 'Pending' }
  ]);

  getAdminMetrics(): Observable<DashboardMetric[]> {
    return of(this.adminMetrics());
  }

  getInstructorMetrics(): Observable<DashboardMetric[]> {
    return of(this.instructorMetrics());
  }

  getPendingInstructors(): Observable<PendingInstructor[]> {
    return of(this.pendingInstructors());
  }

  getStudentSubmissions(): Observable<StudentSubmission[]> {
    return of(this.studentSubmissions());
  }

  // Common fallbacks
  getMetrics(): Observable<DashboardMetric[]> {
    return of(this.adminMetrics());
  }

  getCoursesAnalytics(): Observable<CourseAnalytics[]> {
    return of([
      { id: 'c1', title: 'Advanced Full-Stack Web Development', category: 'Computer Science', instructor: 'Dr. Sarah Jenkins', enrolledStudents: 412, completionRate: 92, avgRating: 4.9, status: 'Active', progressColor: '#4f46e5' },
      { id: 'c2', title: 'Data Structures & Algorithms in Python', category: 'Software Engineering', instructor: 'Prof. Alex Rivera', enrolledStudents: 380, completionRate: 85, avgRating: 4.8, status: 'Active', progressColor: '#06b6d4' },
      { id: 'c3', title: 'UI/UX Design Systems & Micro-Interactions', category: 'Design & Arts', instructor: 'Elena Rostova', enrolledStudents: 295, completionRate: 78, avgRating: 4.7, status: 'Active', progressColor: '#ec4899' }
    ]);
  }

  getAttendanceSummary(): Observable<AttendanceSummary> {
    return of({
      overallPercentage: 94.8,
      presentCount: 4598,
      absentCount: 184,
      lateCount: 52,
      excusedCount: 18,
      gradeBreakdown: [
        { grade: 'Grade 9', percentage: 96.2 },
        { grade: 'Grade 10', percentage: 95.0 },
        { grade: 'Grade 11', percentage: 93.8 }
      ]
    });
  }

  getScheduleTimeline(): Observable<ScheduleItem[]> {
    return of([
      { id: 's1', title: 'Database Architecture & Indexing', courseName: 'CS-401 System Architecture', time: '09:00 AM - 10:30 AM', duration: '90 min', roomOrLink: 'Hall 3B / Zoom', instructor: 'Prof. David K.', type: 'Lecture', status: 'Live' },
      { id: 's2', title: 'React & Angular Performance Tuning Lab', courseName: 'WEB-302 Frontend Mastery', time: '11:00 AM - 12:30 PM', duration: '90 min', roomOrLink: 'Lab 104', instructor: 'Sarah Jenkins', type: 'Lab', status: 'Upcoming' }
    ]);
  }

  getRecentActivity(): Observable<ActivityLogItem[]> {
    return of([
      { id: 'a1', user: { name: 'Michael Scott', avatar: 'https://i.pravatar.cc/150?img=11', role: 'Student' }, action: 'submitted assignment', target: 'Final Project Architecture Specs', timestamp: '5 mins ago', type: 'submission' },
      { id: 'a2', user: { name: 'Dr. Sarah Jenkins', avatar: 'https://i.pravatar.cc/150?img=5', role: 'Instructor' }, action: 'published new lecture', target: 'Module 4: RxJS & Reactive Patterns', timestamp: '25 mins ago', type: 'announcement' }
    ]);
  }

  getQuickActions(): Observable<QuickAction[]> {
    return of([
      { id: 'qa1', label: 'Add New Student', icon: 'fa-solid fa-user-plus', actionKey: 'add_student', colorClass: 'btn-action-primary' },
      { id: 'qa2', label: 'Create Course', icon: 'fa-solid fa-folder-plus', actionKey: 'create_course', colorClass: 'btn-action-purple' },
      { id: 'qa3', label: 'Post Announcement', icon: 'fa-solid fa-bullhorn', badge: 'New', actionKey: 'post_announcement', colorClass: 'btn-action-info' },
      { id: 'qa4', label: 'Export Analytics', icon: 'fa-solid fa-file-export', actionKey: 'export_report', colorClass: 'btn-action-success' }
    ]);
  }
}
