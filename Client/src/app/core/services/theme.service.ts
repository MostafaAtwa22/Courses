import { Injectable, signal, computed, effect } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly isDarkMode = signal(false);
  readonly isDarkModeSignal = this.isDarkMode.asReadonly();

  constructor() {
    this.initTheme();
  }

  private initTheme(): void {
    const savedTheme = localStorage.getItem('theme');
    const isDark = savedTheme === 'dark';
    this.isDarkMode.set(isDark);
    this.applyTheme();
  }

  private applyTheme(): void {
    if (this.isDarkMode()) {
      document.body.classList.add('dark');
    } else {
      document.body.classList.remove('dark');
    }
  }

  toggleTheme(): void {
    this.isDarkMode.update(current => !current);
    this.applyTheme();
    localStorage.setItem('theme', this.isDarkMode() ? 'dark' : 'light');
  }

  setTheme(isDark: boolean): void {
    this.isDarkMode.set(isDark);
    this.applyTheme();
    localStorage.setItem('theme', isDark ? 'dark' : 'light');
  }
}
