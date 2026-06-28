import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { ToastService } from '../../core/notify/toast.service';
import { apiError } from '../../core/util';

@Component({
  selector: 'app-reset-password',
  imports: [FormsModule, RouterLink],
  template: `
    <h1 class="title">Redefinir senha</h1>

    <form (ngSubmit)="submit()">
      <div class="field">
        <label for="email">E-mail</label>
        <input id="email" class="input" type="email" name="email" [(ngModel)]="email" required placeholder="voce@email.com" />
      </div>
      <div class="field">
        <label for="token">Token</label>
        <input id="token" class="input" name="token" [(ngModel)]="token" required placeholder="Token recebido" />
      </div>
      <div class="field">
        <label for="pw">Nova senha</label>
        <input id="pw" class="input" type="password" name="pw" [(ngModel)]="newPassword" required placeholder="Mín. 8 caracteres" autocomplete="new-password" />
      </div>

      @if (error()) { <p class="error-text">{{ error() }}</p> }

      <button class="btn btn--block" type="submit" [disabled]="loading()">
        {{ loading() ? 'Salvando…' : 'Redefinir senha' }}
      </button>
    </form>

    <hr class="sep" />
    <p class="center muted"><a class="link-accent" routerLink="/auth/login">Voltar para o login</a></p>
  `,
  styles: [`
    .title { font-size: 24px; text-align: center; margin-bottom: 22px; }
    .sep { border: none; border-top: 1px solid var(--border); margin: 22px 0; }
    .link-accent { color: var(--accent); font-weight: 600; }
  `],
})
export class ResetPassword {
  private auth = inject(AuthService);
  private toast = inject(ToastService);
  private router = inject(Router);

  protected email = '';
  protected token = '';
  protected newPassword = '';
  protected loading = signal(false);
  protected error = signal('');

  protected submit() {
    if (!this.email || !this.token || !this.newPassword) { this.error.set('Preencha todos os campos.'); return; }
    this.loading.set(true);
    this.error.set('');

    this.auth.resetPassword(this.email, this.token.trim(), this.newPassword).subscribe({
      next: (res) => {
        this.toast.success(res.message || 'Senha redefinida!');
        this.router.navigate(['/auth/login']);
      },
      error: (e) => { this.loading.set(false); this.error.set(apiError(e)); },
    });
  }
}
