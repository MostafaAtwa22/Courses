import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-faq',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './faq.html',
  styleUrl: './faq.scss'
})
export class FaqComponent {
  @Input() faqs: any[] = [];
  @Output() faqToggled = new EventEmitter<number>();

  onToggleFaq(index: number) {
    this.faqToggled.emit(index);
  }
}
