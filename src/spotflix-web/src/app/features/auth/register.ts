import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { ToastService } from '../../core/notify/toast.service';
import { apiError } from '../../core/util';

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink],
  template: `
    <h1 class="title">Crie sua conta</h1>

    <form (ngSubmit)="submit()">
      <div class="field">
        <label for="name">Nome completo</label>
        <input id="name" class="input" name="name" [(ngModel)]="fullName" placeholder="Seu nome" autocomplete="name" />
      </div>
      <div class="field">
        <label for="email">E-mail</label>
        <input id="email" class="input" type="email" name="email" [(ngModel)]="email" required placeholder="voce@email.com" autocomplete="email" />
      </div>
      <div class="field">
        <label for="password">Senha</label>
        <input id="password" class="input" type="password" name="password" [(ngModel)]="password" required placeholder="Mín. 8 caracteres" autocomplete="new-password" />
        <small class="faint">8+ caracteres, com maiúscula, minúscula e número.</small>
      </div>

      @if (error()) { <p class="error-text">{{ error() }}</p> }

      <button class="btn btn--block" type="submit" [disabled]="loading()">
        {{ loading() ? 'Criando…' : 'Cadastrar' }}
      </button>
    </form>

    <hr class="sep" />
    <p class="center muted">Já tem conta? <a class="link-accent" routerLink="/auth/login">Entrar</a></p>
  `,
  styles: [`
    .title { font-size: 26px; text-align: center; margin-bottom: 26px; }
    .sep { border: none; border-top: 1px solid var(--border); margin: 22px 0; }
    .link-accent { color: var(--accent); font-weight: 600; }
    .link-accent:hover { text-decoration: underline; }
    small { font-size: 12px; }
  `],
})
export class Register {
  private auth = inject(AuthService);
  private toast = inject(ToastService);
  private router = inject(Router);

  protected fullName = '';
  protected email = '';
  protected password = '';
  protected loading = signal(false);
  protected error = signal('');

  protected submit() {
    if (!this.email || !this.password) { this.error.set('Preencha e-mail e senha.'); return; }
    this.loading.set(true);
    this.error.set('');

    this.auth.register({ email: this.email, password: this.password, fullName: this.fullName || undefined }).subscribe({
      next: (res) => {
        this.toast.success(res.message || 'Conta criada! Confirme seu e-mail.');
        this.router.navigate(['/auth/confirm-email'], { queryParams: { email: this.email } });
      },
      error: (e) => {
        this.loading.set(false);
        this.error.set(apiError(e, 'Não foi possível concluir o cadastro.'));
      },
    });
  }
}
