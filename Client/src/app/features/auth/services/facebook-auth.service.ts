import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';

declare const FB: any;
declare global {
  interface Window {
    fbAsyncInit: () => void;
  }
}

@Injectable({
  providedIn: 'root',
})

export class FacebookAuthService {
  initialize(): Promise<void> {
    return new Promise(resolve => {
      window.fbAsyncInit = () => {
        FB.init({
          appId: environment.facebookAppId,
          cookie: true,
          xfbml: false,
          version: 'v23.0'
        });
        resolve();
      };
    });
  }

  login(): Promise<string> {
    return new Promise((resolve, reject) => {
      FB.login((response: any) => {
        if (response.authResponse) {
          resolve(response.authResponse.accessToken);
        }
        else {
          reject('Facebook login cancelled.');
        }
      }, 
      {
        scope: 'public_profile,email'
      });
    });
  }

  logout(): void {
    FB.logout();
  }
}
