import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-hero',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './hero.html',
  styleUrl: './hero.scss'
})
export class HeroComponent {
  @Input() slides: any[] = [];
  @Input() currentSlide = 0;
  
  @Output() slideChanged = new EventEmitter<number>();
  @Output() nextRequested = new EventEmitter<void>();
  @Output() prevRequested = new EventEmitter<void>();

  onSetSlide(index: number) {
    this.slideChanged.emit(index);
  }

  onNext() {
    this.nextRequested.emit();
  }

  onPrev() {
    this.prevRequested.emit();
  }
}
