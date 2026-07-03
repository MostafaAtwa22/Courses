import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';

declare const google: any;

@Injectable({
  providedIn: 'root',
})
export class GoogleAuthService {

  private initialized = false;

  /** Waits for the Google GSI script to load, then initializes with the given callback. */
  initialize(callback: (idToken: string) => void): Promise<void> {
    return new Promise((resolve) => {
      const init = () => {
        google.accounts.id.initialize({
          client_id: environment.googleClientId,
          callback: (response: any) => {
            callback(response.credential);
          }
        });
        this.initialized = true;
        resolve();
      };

      if (typeof google !== 'undefined' && google?.accounts?.id) {
        init();
      } else {
        const script = document.querySelector('script[src*="accounts.google.com/gsi/client"]') as HTMLScriptElement | null;
        if (script) {
          script.addEventListener('load', init, { once: true });
        } else {
          const interval = setInterval(() => {
            if (typeof google !== 'undefined' && google?.accounts?.id) {
              clearInterval(interval);
              init();
            }
          }, 100);
        }
      }
    });
  }

  renderButton(element: HTMLElement): void {
    if (!this.initialized) return;
    google.accounts.id.renderButton(element, {
      theme: 'outline',
      size: 'large',
      type: 'icon',
      shape: 'circle',
      width: 48
    });
  }
}
