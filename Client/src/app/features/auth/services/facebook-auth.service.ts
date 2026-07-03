import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';

declare const FB: any;

declare global {
  interface Window {
    fbAsyncInit: () => void;
  }
}

@Injectable({
  providedIn: 'root'
})
export class FacebookAuthService {

  private initialized = false;

  initialize(): Promise<void> {
    return new Promise((resolve, reject) => {
      if (this.initialized) {
        resolve();
        return;
      }
      window.fbAsyncInit = () => {
        try {
          FB.init({
            appId: environment.facebookAppId,
            cookie: true,
            xfbml: false,
            version: 'v22.0'
          });
          this.initialized = true;
          resolve();
        } catch (error) {
          reject(error);
        }
      };
      if ((window as any).FB) {
        window.fbAsyncInit();
      }
    });
  }

  async login(): Promise<string> {
    if (!this.initialized) {
      await this.initialize();
    }
    return new Promise((resolve, reject) => {
      FB.login(
        (response: any) => {
          if (response.authResponse) {
            resolve(response.authResponse.accessToken);
          } else {
            reject('Facebook login cancelled.');
          }
        },
        {
          scope: 'public_profile,email'
        }
      );
    });
  }

  logout(): Promise<void> {
    return new Promise(resolve => {
      FB.logout(() => {
        resolve();
      });
    });
  }

  getLoginStatus(): Promise<any> {
    return new Promise(resolve => {
      FB.getLoginStatus((response: any) => {
        resolve(response);

      });
    });
  }
}