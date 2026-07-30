import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-instructor-additional-data',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './instructor-additional-data.component.html',
  styleUrl: './instructor-additional-data.component.scss'
})
export class InstructorAdditionalDataComponent {
  @Input() phoneNumber: string = '';
  @Input() cvUrl: string = '';
  @Input() linkedInProfileUrl: string = '';
  @Input() gitHubProfileUrl: string = '';
  @Input() status: string = '';
}
