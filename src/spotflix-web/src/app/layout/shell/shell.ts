import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { FavoritesService } from '../../core/api/favorites.service';
import { ToastService } from '../../core/notify/toast.service';
import { PlayerBar } from '../../shared/player-bar/player-bar';
import { initials } from '../../core/util';

@Component({
  selector: 'app-shell',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, PlayerBar],
  template: `
    <div class="shell">
      <aside class="sidebar">
        <a class="brand" routerLink="/">
          <span class="material-symbols-rounded brand__logo">graphic_eq</span>
          <span class="brand__name">Spotflix</span>
        </a>

        <nav class="nav">
          <a routerLink="/" routerLinkActive="active" [routerLinkActiveOptions]="{ exact: true }">
            <span class="material-symbols-rounded">home</span> Início
          </a>
          <a routerLink="/search" routerLinkActive="active">
            <span class="material-symbols-rounded">search</span> Buscar
          </a>
          <a routerLink="/plans" routerLinkActive="active">
            <span class="material-symbols-rounded">workspace_premium</span> Planos
          </a>
          @if (auth.isAuthenticated()) {
            <a routerLink="/favorites" routerLinkActive="active">
              <span class="material-symbols-rounded">favorite</span> Favoritos
            </a>
          }
          @if (auth.isAdmin()) {
            <a routerLink="/admin" routerLinkActive="active">
              <span class="material-symbols-rounded">shield_person</span> Administração
            </a>
          }
        </nav>

        <div class="sidebar__foot faint">
          Spotflix &copy; {{ year }}
        </div>
      </aside>

      <div class="main">
        <header class="topbar">
          <div class="spacer"></div>

          @if (auth.isAuthenticated()) {
            <div class="user" (click)="toggleMenu()">
              <div class="avatar">{{ avatar() }}</div>
              <span class="user__name">{{ auth.currentUser()?.fullName || auth.email() }}</span>
              <span class="material-symbols-rounded">expand_more</span>
            </div>
            @if (menuOpen()) {
              <div class="menu" (click)="$event.stopPropagation()">
                <a routerLink="/account" (click)="closeMenu()">
                  <span class="material-symbols-rounded">person</span> Minha conta
                </a>
                <button (click)="logout()">
                  <span class="material-symbols-rounded">logout</span> Sair
                </button>
              </div>
            }
          } @else {
            <a class="btn btn--ghost btn--sm" routerLink="/auth/register">Cadastrar</a>
            <a class="btn btn--sm" routerLink="/auth/login">Entrar</a>
          }
        </header>

        <main class="content">
          <router-outlet />
        </main>
      </div>
    </div>

    <app-player-bar />
  `,
  styleUrl: './shell.scss',
})
export class Shell {
  protected auth = inject(AuthService);
  private favorites = inject(FavoritesService);
  private toast = inject(ToastService);
  private router = inject(Router);

  protected readonly year = new Date().getFullYear();
  protected menuOpen = signal(false);

  protected avatar = () => initials(this.auth.currentUser()?.fullName || this.auth.email() || '?');

  protected toggleMenu() { this.menuOpen.update((v) => !v); }
  protected closeMenu() { this.menuOpen.set(false); }

  protected back() { history.back(); }
  protected forward() { history.forward(); }

  protected logout() {
    this.auth.logout();
    this.favorites.reset();
    this.closeMenu();
    this.toast.info('Você saiu da sua conta.');
    this.router.navigate(['/']);
  }
}
