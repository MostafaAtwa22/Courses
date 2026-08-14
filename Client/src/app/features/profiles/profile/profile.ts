import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from '../../../shared/components/header/header';
import { FooterComponent } from '../../../shared/components/footer/footer';
import { SessionService } from '../../auth/services/session.service';
import { BaseIdentityResponse } from '../../auth/models/auth.models';
import { Gender } from '../../../shared/models/identity.models';
import { ThemeService } from '../../../core/services/theme.service';
import { InstructorService } from '../../instructors/services/instructor.service';
import { InstructorPrivateResponse } from '../../instructors/models/instructor.models';
import { InstructorStatsComponent } from '../instructor-profile/instructor-stats/instructor-stats.component';
import { InstructorAboutComponent } from '../instructor-profile/instructor-about/instructor-about.component';
import { InstructorAdditionalDataComponent } from '../instructor-profile/instructor-additional-data/instructor-additional-data.component';
import { RouterLink } from '@angular/router';
import { UserCoursesComponent } from '../user-courses/user-courses.component';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, HeaderComponent, FooterComponent, InstructorStatsComponent, InstructorAboutComponent, InstructorAdditionalDataComponent, RouterLink, UserCoursesComponent],
  templateUrl: './profile.html',
  styleUrl: './profile.scss'
})
export class ProfileComponent implements OnInit {
  private themeService = inject(ThemeService);
  private instructorService = inject(InstructorService);
  private sessionService = inject(SessionService);
  isDarkMode = this.themeService.isDarkModeSignal();
  currentUser: BaseIdentityResponse | null = null;
  instructorData: InstructorPrivateResponse | null = null;

  private readonly defaultMalePic   = 'assets/users/default-male.png';
  private readonly defaultFemalePic = 'assets/users/default-female.png';

  ngOnInit() {
    this.currentUser = this.sessionService.currentUser();
    if (this.isInstructor) {
      this.loadInstructorData();
    }
  }

  private async loadInstructorData() {
    try {
      const data = await this.instructorService.getCurrentInstructor().toPromise();
      this.instructorData = data ?? null;
    } catch (error) {
      console.error('Failed to load instructor data:', error);
    }
  }

  toggleTheme() {
    this.themeService.toggleTheme();
    this.isDarkMode = this.themeService.isDarkModeSignal();
  }

  get fullName(): string {
    return `${this.currentUser?.firstName || ''} ${this.currentUser?.lastName || ''}`.trim() || 'User';
  }

  get profilePicture(): string {
    if (this.currentUser?.profilePicture) return this.currentUser.profilePicture;
    return this.currentUser?.gender === Gender.Female ? this.defaultFemalePic : this.defaultMalePic;
  }

  get userRole(): string {
    return this.currentUser?.roles?.[0] || 'Student';
  }

  get isInstructor(): boolean {
    return this.currentUser?.roles?.includes('Instructor') || false;
  }

  get emailStatus(): string {
    return 'Verified';
  }

  get formattedJoinDate(): string {
    if (!this.currentUser?.createdAt) return 'N/A';
    const date = new Date(this.currentUser.createdAt);
    const options: Intl.DateTimeFormatOptions = { year: 'numeric', month: 'long' };
    return date.toLocaleDateString('en-US', options);
  }

  // Instructor-specific data
  get totalCourses(): number {
    return this.instructorData?.totalCourses || 0;
  }

  get totalStudents(): number {
    return this.instructorData?.totalStudents || 0;
  }

  get totalReviews(): number {
    return this.instructorData?.totalReviews || 0;
  }

  get averageRate(): number {
    return this.instructorData?.averageRate || 0;
  }

  get instructorBio(): string {
    return this.instructorData?.bio || '';
  }

  get instructorTitle(): string {
    return this.instructorData?.title || '';
  }

  get instructorPhoneNumber(): string {
    return this.instructorData?.phoneNumber || '';
  }

  get instructorCvUrl(): string {
    return this.instructorData?.cvUrl || '';
  }

  get instructorLinkedInProfileUrl(): string {
    return this.instructorData?.linkedInProfileUrl || '';
  }

  get instructorGitHubProfileUrl(): string {
    return this.instructorData?.gitHubProfileUrl || '';
  }

  get instructorStatus(): string {
    return this.instructorData?.status || '';
  }

  get userExpertise(): string[] {
    return [];
  }
}
