import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from '../../../shared/components/header/header';
import { FooterComponent } from '../../../shared/components/footer/footer';

interface UserData {
  id?: string;
  firstName?: string;
  lastName?: string;
  email?: string;
  userName?: string;
  phoneNumber?: string;
  gender?: string;
  dateOfBirth?: string;
  profilePictureUrl?: string;
  emailConfirmed?: boolean;
  role?: string;
  bio?: string;
  professionalTitle?: string;
  expertise?: string[];
  joinedDate?: string;
}

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, HeaderComponent, FooterComponent],
  templateUrl: './profile.html',
  styleUrl: './profile.scss'
})
export class ProfileComponent implements OnInit {
  isDarkMode = false;
  userData: UserData = {};
  defaultProfilePic = 'https://lh3.googleusercontent.com/aida-public/AB6AXuD846MwoUL-gkyvmsCklyIYaIw3PhBRkAUk6nVXKaYTrcacVtiOrABJiidiM1MBF8Wck8necDtqco_FMeElw1udKJrkkFWgC2fanzvlTicGLAhPrYUuO5I2CwJ5HVSBQImVlalM8jhng7_7jVtd4AeTcwtfMuxbpHNTtVlgeLtkYbspaV5WTc80SbMBcTBe59cSuo3WuincuF3O8orpD9K26Bot36sPQ2JAwQJce-BzeCFSug5kt1gimP6D3pZwU91VJpwXAyZSv1y1';

  ngOnInit() {
    // Load theme
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark') {
      this.isDarkMode = true;
      document.body.classList.add('dark');
    }

    // Load user data from localStorage
    this.loadUserData();
  }

  loadUserData() {
    const userDataString = localStorage.getItem('EduFocus_user');
    if (userDataString) {
      try {
        this.userData = JSON.parse(userDataString);
      } catch (error) {
        console.error('Error parsing user data from localStorage:', error);
        this.userData = {};
      }
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

  get fullName(): string {
    return `${this.userData.firstName || ''} ${this.userData.lastName || ''}`.trim() || 'User';
  }

  get profilePicture(): string {
    return this.userData.profilePictureUrl || this.defaultProfilePic;
  }

  get userRole(): string {
    return this.userData.role || 'Student';
  }

  get emailStatus(): string {
    return this.userData.emailConfirmed ? 'Verified' : 'Not Verified';
  }

  get formattedJoinDate(): string {
    if (this.userData.joinedDate) {
      const date = new Date(this.userData.joinedDate);
      return date.toLocaleDateString('en-US', { month: 'short', year: 'numeric' });
    }
    return 'N/A';
  }

  get userExpertise(): string[] {
    return this.userData.expertise || ['React', 'Node.js', 'Cloud Arch'];
  }
}
