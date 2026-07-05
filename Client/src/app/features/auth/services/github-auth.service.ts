import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class GithubAuthService {
  private readonly authorizeUrl = 'https://github.com/login/oauth/authorize';
  private readonly scope = 'read:user user:email';
  private readonly callbackPath = '/auth/github-callback';

  get redirectUri(): string {
    return `${window.location.origin}${this.callbackPath}`;
  }


  login(): Promise<string> {
    return new Promise((resolve, reject) => {
      const state = this.generateState();
      sessionStorage.setItem('github_oauth_state', state);

      const params = new URLSearchParams({
        client_id: environment.githubClientId,
        redirect_uri: this.redirectUri,
        scope: this.scope,
        state
      });

      const popupUrl = `${this.authorizeUrl}?${params.toString()}`;
      const popupWidth = 600;
      const popupHeight = 700;
      const left = window.screenX + (window.outerWidth - popupWidth) / 2;
      const top = window.screenY + (window.outerHeight - popupHeight) / 2;

      const popup = window.open(
        popupUrl,
        'github-oauth',
        `width=${popupWidth},height=${popupHeight},left=${left},top=${top},toolbar=no,menubar=no,scrollbars=yes`
      );

      if (!popup) {
        reject('Popup was blocked. Please allow popups for this site and try again.');
        return;
      }

      const messageHandler = (event: MessageEvent) => {
        // Only accept messages from our own origin
        if (event.origin !== window.location.origin) return;

        if (event.data?.type === 'github-oauth-callback') {
          cleanup();

          const { code, state: returnedState, error } = event.data;

          if (error) {
            reject(`GitHub login failed: ${error}`);
            return;
          }

          const savedState = sessionStorage.getItem('github_oauth_state');
          sessionStorage.removeItem('github_oauth_state');

          if (returnedState !== savedState) {
            reject('OAuth state mismatch. Please try again.');
            return;
          }

          if (!code) {
            reject('No authorization code received from GitHub.');
            return;
          }

          resolve(code);
        }
      };

      const cleanup = () => {
        window.removeEventListener('message', messageHandler);
        clearInterval(popupMonitor);
        if (popup && !popup.closed) popup.close();
      };

      // Detect if user closes the popup manually
      const popupMonitor = setInterval(() => {
        if (popup.closed) {
          cleanup();
          reject('GitHub login was cancelled.');
        }
      }, 500);

      window.addEventListener('message', messageHandler);
    });
  }

  private generateState(): string {
    const array = new Uint8Array(16);
    crypto.getRandomValues(array);
    return Array.from(array, b => b.toString(16).padStart(2, '0')).join('');
  }
}
