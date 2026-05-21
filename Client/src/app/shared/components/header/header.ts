import { Component, Input, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../../features/auth/services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class HeaderComponent {
  @Input() isDarkMode = false;
  @Output() themeToggled = new EventEmitter<void>();

  authService = inject(AuthService);
  private router = inject(Router);

  isMenuOpen = false;

  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }

  onThemeToggle() {
    this.themeToggled.emit();
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }
}
