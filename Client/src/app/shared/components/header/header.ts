import { Component, inject, HostListener, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../../features/auth/services/auth.service';
import { ThemeService } from '../../../core/services/theme.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class HeaderComponent {
  authService   = inject(AuthService);
  themeService  = inject(ThemeService);
  private router  = inject(Router);
  private elRef   = inject(ElementRef);

  isMenuOpen     = false;
  isDropdownOpen = false;

  get isDarkMode(): boolean {
    return this.themeService.isDarkModeSignal();
  }

  onThemeToggle(): void {
    this.themeService.toggleTheme();
  }

  toggleMenu(): void {
    this.isMenuOpen = !this.isMenuOpen;
  }

  toggleDropdown(): void {
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.elRef.nativeElement.contains(event.target)) {
      this.isDropdownOpen = false;
    }
  }

  logout(): void {
    this.isDropdownOpen = false;
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }
}
