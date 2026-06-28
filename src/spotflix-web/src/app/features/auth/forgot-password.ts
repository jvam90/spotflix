import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { apiError } from '../../core/util';

@Component({
  selector: 'app-forgot-password',
  imports: [FormsModule, RouterLink],
  template: `
    <h1 class="title">Recuperar senha</h1>
    <p class="muted center hint">Enviaremos instruções de redefinição se o e-mail existir.</p>

    <form (ngSubmit)="submit()">
      <div class="field">
        <label for="email">E-mail</label>
        <input id="email" class="input" type="email" name="email" [(ngModel)]="email" required placeholder="voce@email.com" />
      </div>

      @if (message()) { <p class="ok">{{ message() }}</p> }
      @if (error()) { <p class="error-text">{{ error() }}</p> }

      <button class="btn btn--block" type="submit" [disabled]="loading()">
        {{ loading() ? 'Enviando…' : 'Enviar instruções' }}
      </button>
    </form>

    <hr class="sep" />
    <p class="center muted">
      Já tem o token? <a class="link-accent" routerLink="/auth/reset-password">Redefinir senha</a>
    </p>
    <p class="center muted"><a routerLink="/auth/login">Voltar para o login</a></p>
  `,
  styles: [`
    .title { font-size: 24px; text-align: center; margin-bottom: 12px; }
    .hint { font-size: 13px; margin-bottom: 22px; }
    .ok { color: var(--accent); font-size: 13px; }
    .sep { border: none; border-top: 1px solid var(--border); margin: 22px 0; }
    .link-accent { color: var(--accent); font-weight: 600; }
  `],
})
export class ForgotPassword {
  private auth = inject(AuthService);

  protected email = '';
  protected loading = signal(false);
  protected error = signal('');
  protected message = signal('');

  protected submit() {
    if (!this.email) { this.error.set('Informe seu e-mail.'); return; }
    this.loading.set(true);
    this.error.set('');
    this.message.set('');

    this.auth.forgotPassword(this.email).subscribe({
      next: (res) => { this.loading.set(false); this.message.set(res.message); },
      error: (e) => { this.loading.set(false); this.error.set(apiError(e)); },
    });
  }
}
