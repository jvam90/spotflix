import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { ToastService } from '../../core/notify/toast.service';
import { apiError } from '../../core/util';

@Component({
  selector: 'app-confirm-email',
  imports: [FormsModule, RouterLink],
  template: `
    <h1 class="title">Confirme seu e-mail</h1>
    <p class="muted center hint">
      Verifique sua caixa de entrada e informe o <strong>userId</strong> e o <strong>token</strong> recebidos por e-mail.
    </p>

    <form (ngSubmit)="submit()">
      <div class="field">
        <label for="userId">User ID</label>
        <input id="userId" class="input" name="userId" [(ngModel)]="userId" required placeholder="GUID do usuário" />
      </div>
      <div class="field">
        <label for="token">Token</label>
        <input id="token" class="input" name="token" [(ngModel)]="token" required placeholder="Token de confirmação" />
      </div>

      @if (error()) { <p class="error-text">{{ error() }}</p> }

      <button class="btn btn--block" type="submit" [disabled]="loading()">
        {{ loading() ? 'Confirmando…' : 'Confirmar e-mail' }}
      </button>
    </form>

    <hr class="sep" />
    <p class="center muted"><a class="link-accent" routerLink="/auth/login">Voltar para o login</a></p>
  `,
  styles: [`
    .title { font-size: 24px; text-align: center; margin-bottom: 12px; }
    .hint { font-size: 13px; margin-bottom: 22px; }
    .sep { border: none; border-top: 1px solid var(--border); margin: 22px 0; }
    .link-accent { color: var(--accent); font-weight: 600; }
  `],
})
export class ConfirmEmail {
  private auth = inject(AuthService);
  private toast = inject(ToastService);
  private router = inject(Router);

  protected userId = '';
  protected token = '';
  protected loading = signal(false);
  protected error = signal('');

  protected submit() {
    if (!this.userId || !this.token) { this.error.set('Informe userId e token.'); return; }
    this.loading.set(true);
    this.error.set('');

    this.auth.confirmEmail(this.userId.trim(), this.token.trim()).subscribe({
      next: (res) => {
        this.toast.success(res.message || 'E-mail confirmado!');
        this.router.navigate(['/auth/login']);
      },
      error: (e) => {
        this.loading.set(false);
        this.error.set(apiError(e, 'Não foi possível confirmar o e-mail.'));
      },
    });
  }
}
