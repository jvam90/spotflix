import { Component } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';

@Component({
  selector: 'app-auth-layout',
  imports: [RouterOutlet, RouterLink],
  template: `
    <div class="auth">
      <div class="auth__panel">
        <a class="brand" routerLink="/">
          <span class="material-symbols-rounded">graphic_eq</span> Spotflix
        </a>
        <router-outlet />
        <p class="legal faint">Som sem limites. Bandas, álbuns e playlists num só lugar.</p>
      </div>
    </div>
  `,
  styles: [`
    .auth {
      min-height: 100vh; display: grid; place-items: center; padding: 24px;
      background: radial-gradient(1200px 600px at 50% -10%, #1f3d2a 0%, var(--bg-base) 55%);
    }
    .auth__panel {
      width: 100%; max-width: 420px;
      background: var(--bg-elevated); border-radius: 18px; padding: 40px 36px;
      box-shadow: var(--shadow);
    }
    .brand {
      display: flex; align-items: center; justify-content: center; gap: 10px;
      font-size: 26px; font-weight: 800; margin-bottom: 28px;
    }
    .brand .material-symbols-rounded { color: var(--accent); font-size: 32px; }
    .legal { text-align: center; font-size: 12px; margin: 24px 0 0; }
  `],
})
export class AuthLayout {}
