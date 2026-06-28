import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { FavoritesService } from '../../core/api/favorites.service';
import { ToastService } from '../../core/notify/toast.service';
import { apiError } from '../../core/util';

@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink],
  template: `
    <h1 class="title">Entrar no Spotflix</h1>

    <form (ngSubmit)="submit()">
      <div class="field">
        <label for="email">E-mail</label>
        <input id="email" class="input" type="email" name="email" [(ngModel)]="email" required autocomplete="email" placeholder="voce@email.com" />
      </div>
      <div class="field">
        <label for="password">Senha</label>
        <input id="password" class="input" type="password" name="password" [(ngModel)]="password" required autocomplete="current-password" placeholder="••••••••" />
      </div>

      @if (error()) { <p class="error-text">{{ error() }}</p> }

      <button class="btn btn--block" type="submit" [disabled]="loading()">
        {{ loading() ? 'Entrando…' : 'Entrar' }}
      </button>
    </form>

    <div class="links">
      <a routerLink="/auth/forgot-password">Esqueceu a senha?</a>
    </div>
    <hr class="sep" />
    <p class="center muted">Não tem conta? <a class="link-accent" routerLink="/auth/register">Cadastre-se</a></p>
  `,
  styles: [`
    .title { font-size: 26px; text-align: center; margin-bottom: 26px; }
    .links { text-align: center; margin-top: 14px; }
    .links a { color: var(--text-soft); font-size: 14px; }
    .links a:hover { color: #fff; text-decoration: underline; }
    .sep { border: none; border-top: 1px solid var(--border); margin: 22px 0; }
    .link-accent { color: var(--accent); font-weight: 600; }
    .link-accent:hover { text-decoration: underline; }
  `],
})
export class Login {
  private auth = inject(AuthService);
  private favorites = inject(FavoritesService);
  private toast = inject(ToastService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  protected email = '';
  protected password = '';
  protected loading = signal(false);
  protected error = signal('');

  protected submit() {
    if (!this.email || !this.password) { this.error.set('Preencha e-mail e senha.'); return; }
    this.loading.set(true);
    this.error.set('');

    this.auth.login({ email: this.email, password: this.password }).subscribe({
      next: () => {
        this.auth.loadProfile().subscribe();
        this.favorites.loadSongs().subscribe({ error: () => {} });
        this.favorites.loadBands().subscribe({ error: () => {} });
        this.toast.success('Bem-vindo de volta!');
        const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') || '/';
        this.router.navigateByUrl(returnUrl);
      },
      error: (e) => {
        this.loading.set(false);
        this.error.set(apiError(e, 'Não foi possível entrar.'));
      },
    });
  }
}
