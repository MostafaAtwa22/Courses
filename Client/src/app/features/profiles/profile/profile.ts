import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from '../../../shared/components/header/header';
import { FooterComponent } from '../../../shared/components/footer/footer';
import { SessionService } from '../../auth/services/session.service';
import { BaseIdentityResponse } from '../../auth/models/auth.models';
import { Gender } from '../../../shared/models/identity.models';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, HeaderComponent, FooterComponent],
  templateUrl: './profile.html',
  styleUrl: './profile.scss'
})
export class ProfileComponent implements OnInit {
  isDarkMode = false;
  currentUser: BaseIdentityResponse | null = null;

  private readonly defaultMalePic   = 'assets/users/default-male.png';
  private readonly defaultFemalePic = 'assets/users/default-female.png';

  constructor(private sessionService: SessionService) {}

  ngOnInit() {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark') {
      this.isDarkMode = true;
      document.body.classList.add('dark');
    }

    this.currentUser = this.sessionService.currentUser();
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
    return `${this.currentUser?.firstName || ''} ${this.currentUser?.lastName || ''}`.trim() || 'User';
  }

  get profilePicture(): string {
    if (this.currentUser?.profilePicture) return this.currentUser.profilePicture;
    return this.currentUser?.gender === Gender.Female ? this.defaultFemalePic : this.defaultMalePic;
  }

  get userRole(): string {
    return this.currentUser?.roles?.[0] || 'Student';
  }

  get emailStatus(): string {
    return 'Verified';
  }

  get formattedJoinDate(): string {
    return 'N/A';
  }

  get userExpertise(): string[] {
    return [];
  }
}
