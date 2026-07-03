import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

/**
 * This component is loaded inside the GitHub OAuth popup.
 * It reads the code/state from the URL and posts them back to the opener,
 * then closes itself.
 */
@Component({
  selector: 'app-github-callback',
  standalone: true,
  template: `
    <div style="display:flex;align-items:center;justify-content:center;height:100vh;font-family:sans-serif;color:#555;">
      <p>Completing GitHub sign-in…</p>
    </div>
  `
})
export class GithubCallbackComponent implements OnInit {
  constructor(private route: ActivatedRoute) {}

  ngOnInit(): void {
    const params = this.route.snapshot.queryParams;
    const code   = params['code']  ?? null;
    const state  = params['state'] ?? null;
    const error  = params['error'] ?? null;

    if (window.opener) {
      window.opener.postMessage(
        { type: 'github-oauth-callback', code, state, error },
        window.location.origin
      );
      window.close();
    } else {
      // Fallback: if not opened as popup, redirect to login with error
      window.location.href = '/auth/login';
    }
  }
}
