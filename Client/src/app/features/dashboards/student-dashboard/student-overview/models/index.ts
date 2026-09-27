import { CourseSummary, CourseProgressSummary } from '../../../../courses/models/course.models';

export interface EnrolledCourseWithProgress extends CourseSummary {
  progress?: CourseProgressSummary;
  progressPercentage?: number;
  lastAccessed?: Date;
}

export interface Transaction {
  id: string;
  date: string;
  courseName: string;
  instructor: string;
  amount: number;
  paymentMethod: string;
  status: 'completed' | 'pending' | 'failed';
}

export interface StudentStatistics {
  totalEnrolled: number;
  totalPaid: number;
  completedCourses: number;
  inProgressCourses: number;
}
