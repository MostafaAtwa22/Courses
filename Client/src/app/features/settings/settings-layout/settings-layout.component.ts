import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HeaderComponent } from '../../../shared/components/header/header';
import { SessionService } from '../../auth/services/session.service';
import { ThemeService } from '../../../core/services/theme.service';

@Component({
  selector: 'app-settings-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, HeaderComponent],
  templateUrl: './settings-layout.component.html',
  styleUrl: './settings-layout.component.scss'
})
export class SettingsLayoutComponent {
  private themeService = inject(ThemeService);
  private sessionService = inject(SessionService);
  isDarkMode = this.themeService.isDarkModeSignal();
  currentUser = this.sessionService.currentUser;

  toggleTheme() {
    this.themeService.toggleTheme();
    this.isDarkMode = this.themeService.isDarkModeSignal();
  }
}
