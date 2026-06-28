import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-admin-layout',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <div class="admin-head">
      <h1><span class="material-symbols-rounded">shield_person</span> Administração</h1>
      <nav class="subnav">
        <a routerLink="catalog" routerLinkActive="active">Catálogo</a>
        <a routerLink="plans" routerLinkActive="active">Planos</a>
        <a routerLink="users" routerLinkActive="active">Usuários</a>
      </nav>
    </div>
    <router-outlet />
  `,
  styles: [`
    .admin-head { padding: 12px 0 4px; }
    .admin-head h1 { display: flex; align-items: center; gap: 10px; font-size: 30px; }
    .admin-head h1 .material-symbols-rounded { color: var(--accent); font-size: 30px; }
    .subnav { display: flex; gap: 6px; margin: 20px 0 24px; border-bottom: 1px solid var(--border); }
    .subnav a {
      padding: 10px 16px; color: var(--text-soft); font-weight: 600; font-size: 14px;
      border-bottom: 2px solid transparent; margin-bottom: -1px;
    }
    .subnav a:hover { color: #fff; }
    .subnav a.active { color: #fff; border-bottom-color: var(--accent); }
  `],
})
export class AdminLayout {}
