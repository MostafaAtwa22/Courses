import { Component, inject, OnInit, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { GoogleAuthService } from '../services/google-auth.service';
import { FacebookAuthService } from '../services/facebook-auth.service';
import { GithubAuthService } from '../services/github-auth.service';
import { AuthResponseDto } from '../models/auth.models';

type LoginMethod = 'email' | 'google' | 'facebook' | 'github';

interface AuthState {
  loading: boolean;
  loadingMethod: LoginMethod | null;
  error: string;
  success: string;
}

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  private fb                  = inject(FormBuilder);
  private authService         = inject(AuthService);
  private googleAuthService   = inject(GoogleAuthService);
  private facebookAuthService = inject(FacebookAuthService);
  private githubAuthService   = inject(GithubAuthService);
  private router              = inject(Router);
  private route               = inject(ActivatedRoute);

  @ViewChild('googleButton', { static: true }) googleButton!: ElementRef;

  loginForm = this.fb.nonNullable.group({
    email:    ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
  });

  authState: AuthState = {
    loading: false,
    loadingMethod: null,
    error: '',
    success: ''
  };

  showPassword = false;

  get isEmailLoading()     { return this.authState.loadingMethod === 'email'; }
  get isGoogleLoading()    { return this.authState.loadingMethod === 'google'; }
  get isFacebookLoading()  { return this.authState.loadingMethod === 'facebook'; }
  get isGithubLoading() { return this.authState.loadingMethod === 'github'; }

  ngOnInit() {
    if (this.route.snapshot.queryParams['reset'] === 'success') {
      this.authState.success = 'Password reset successfully. You can now sign in.';
    }

    this.facebookAuthService.initialize();

    this.googleAuthService.initialize((idToken: string) => {
      this.handleGoogleLogin(idToken);
    }).then(() => {
      this.googleAuthService.renderButton(this.googleButton.nativeElement);
    });
  }


  onSubmit() {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.startLoading('email');

    this.authService.login(this.loginForm.getRawValue()).subscribe({
      next:  (response) => this.handleSuccess(response),
      error: (err) => this.handleError('email', err, 'Login failed. Please check your credentials.')
    });
  }

  handleGoogleLogin(idToken: string) {
    this.startLoading('google');

    this.authService.googleLogin({ idToken }).subscribe({
      next:  (response) => this.handleSuccess(response),
      error: (err) => this.handleError('google', err, 'Google login failed. Please try again.')
    });
  }

  async handleFacebookLogin() {
    try {
      this.startLoading('facebook');
      const accessToken = await this.facebookAuthService.login();

      this.authService.facebookLogin({ accessToken }).subscribe({
        next:  (response) => this.handleSuccess(response),
        error: (err) => this.handleError('facebook', err, 'Facebook login failed. Please try again.')
      });
    } catch {
      this.handleError('facebook', null, 'Facebook login was cancelled.');
    }
  }

  async handleGithubLogin() {
    try {
      this.startLoading('github');
      const code        = await this.githubAuthService.login();
      const redirectUri = this.githubAuthService.redirectUri;

      this.authService.githubLogin({ code, redirectUri }).subscribe({
        next:  (response) => this.handleSuccess(response),
        error: (err) => this.handleError('github', err, 'GitHub login failed. Please try again.')
      });
    } catch (err: any) {
      this.handleError('github', null, typeof err === 'string' ? err : 'GitHub login was cancelled.');
    }
  }

  private startLoading(method: LoginMethod) {
    this.authState = {
      loading: true,
      loadingMethod: method,
      error: '',
      success: ''
    };
  }

  private handleSuccess(response: AuthResponseDto) {
    this.stopLoading();

    if (response.requiresTwoFactor) {
      this.router.navigate(['/auth/two-factor'], {
        queryParams: { email: response.email ?? this.loginForm.value.email }
      });
    } else {
      this.router.navigate(['/']);
    }
  }

  private handleError(_method: LoginMethod, err: any, fallbackMessage: string) {
    this.stopLoading();
    this.authState.error = err?.error?.detail || err?.error?.title || fallbackMessage;
  }

  private stopLoading() {
    this.authState.loading = false;
    this.authState.loadingMethod = null;
  }
}
