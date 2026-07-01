import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';

declare const google: any;

@Injectable({
  providedIn: 'root',
})
export class GoogleAuthService {
  initialize(callback: (idToken: string) => void): void {

    google.accounts.id.initialize({
      client_id: environment.googleClientId,
      callback: (response: any) => {
        callback(response.credential);
      }
    });
  }

  renderButton(element: HTMLElement): void {
    google.accounts.id.renderButton(element, {
      theme: 'outline',
      size: 'large',
      width: 300
    });
  }

  prompt(): void {
    google.accounts.id.prompt();
  }
}
