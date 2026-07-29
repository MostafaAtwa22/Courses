import { Component, signal, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ThemeService } from './core/services/theme.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('Client');
  private themeService = inject(ThemeService);

  constructor() {
    // ThemeService is automatically initialized in its constructor
    // This ensures theme is applied globally when the app starts
  }
}
