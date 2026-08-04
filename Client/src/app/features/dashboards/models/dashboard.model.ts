export interface DashboardMetric {
  id: string;
  title: string;
  value: string | number;
  change: string;
  isPositive: boolean;
  icon: string;
  color: 'primary' | 'success' | 'warning' | 'info' | 'purple';
  description: string;
}

export interface CourseAnalytics {
  id: string;
  title: string;
  category: string;
  instructor: string;
  enrolledStudents: number;
  completionRate: number; // 0 - 100
  avgRating: number;
  status: 'Active' | 'Draft' | 'Archived';
  progressColor: string;
}

export interface AttendanceSummary {
  overallPercentage: number;
  presentCount: number;
  absentCount: number;
  lateCount: number;
  excusedCount: number;
  gradeBreakdown: {
    grade: string;
    percentage: number;
  }[];
}

export interface ScheduleItem {
  id: string;
  title: string;
  courseName: string;
  time: string;
  duration: string;
  roomOrLink: string;
  instructor: string;
  type: 'Lecture' | 'Lab' | 'Quiz' | 'Meeting';
  status: 'Upcoming' | 'Live' | 'Completed';
}

export interface ActivityLogItem {
  id: string;
  user: {
    name: string;
    avatar: string;
    role: string;
  };
  action: string;
  target: string;
  timestamp: string;
  type: 'enrollment' | 'submission' | 'grade' | 'announcement' | 'system';
}

export interface QuickAction {
  id: string;
  label: string;
  icon: string;
  badge?: string;
  actionKey: string;
  colorClass: string;
}
