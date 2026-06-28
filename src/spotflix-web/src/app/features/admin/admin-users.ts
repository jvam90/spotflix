import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { UsersService } from '../../core/api/users.service';
import { AuthService } from '../../core/auth/auth.service';
import { ToastService } from '../../core/notify/toast.service';
import { User } from '../../core/models';
import { apiError, formatDate } from '../../core/util';

@Component({
  selector: 'app-admin-users',
  imports: [FormsModule],
  template: `
    <div class="bar"><h2>Usuários</h2></div>

    <div class="toolbar">
      <input class="input" placeholder="Buscar por nome ou e-mail…" [(ngModel)]="search" (input)="onSearch()" />
      <span class="muted">{{ total() }} usuário(s)</span>
    </div>

    @if (loading()) {
      <p class="muted">Carregando…</p>
    } @else {
      <table class="grid-table">
        <thead><tr><th>Usuário</th><th>Papéis</th><th>Status</th><th>Criado</th><th></th></tr></thead>
        <tbody>
          @for (u of users(); track u.id) {
            <tr>
              <td>
                <div class="name">{{ u.fullName || '—' }}</div>
                <div class="meta muted">{{ u.email }}</div>
              </td>
              <td>
                <div class="row gap-8">
                  @for (r of u.roles; track r) { <span class="tag">{{ r }}</span> }
                  <button class="icon-btn" (click)="toggleAdmin(u)" [title]="isAdmin(u) ? 'Remover admin' : 'Tornar admin'">
                    <span class="material-symbols-rounded">{{ isAdmin(u) ? 'remove_moderator' : 'add_moderator' }}</span>
                  </button>
                </div>
              </td>
              <td>
                @if (u.isLockedOut) { <span class="tag" style="color: var(--danger)">Bloqueado</span> }
                @else if (!u.emailConfirmed) { <span class="tag" style="color: var(--warn)">Não confirmado</span> }
                @else { <span class="tag" style="color: var(--accent)">Ativo</span> }
              </td>
              <td class="muted">{{ formatDate(u.createdAt) }}</td>
              <td class="right">
                <div class="row gap-8" style="justify-content:flex-end">
                  @if (u.isLockedOut) {
                    <button class="icon-btn" (click)="unlock(u)" title="Desbloquear"><span class="material-symbols-rounded">lock_open</span></button>
                  } @else {
                    <button class="icon-btn" (click)="lock(u)" title="Bloquear"><span class="material-symbols-rounded">lock</span></button>
                  }
                  <button class="icon-btn danger" (click)="remove(u)" title="Excluir"><span class="material-symbols-rounded">delete</span></button>
                </div>
              </td>
            </tr>
          }
        </tbody>
      </table>

      <div class="pager">
        <button (click)="prev()" [disabled]="page() <= 1"><span class="material-symbols-rounded">chevron_left</span></button>
        <span>Página {{ page() }} de {{ totalPages() }}</span>
        <button (click)="next()" [disabled]="page() >= totalPages()"><span class="material-symbols-rounded">chevron_right</span></button>
      </div>
    }
  `,
  styleUrl: './admin-shared.scss',
})
export class AdminUsers {
  private api = inject(UsersService);
  private auth = inject(AuthService);
  private toast = inject(ToastService);

  protected users = signal<User[]>([]);
  protected loading = signal(true);
  protected page = signal(1);
  protected total = signal(0);
  protected readonly pageSize = 15;
  protected search = '';
  protected formatDate = formatDate;

  private debounce?: ReturnType<typeof setTimeout>;

  constructor() { this.load(); }

  protected totalPages() { return Math.max(1, Math.ceil(this.total() / this.pageSize)); }
  protected isAdmin = (u: User) => u.roles.includes('Admin');

  protected onSearch() {
    clearTimeout(this.debounce);
    this.debounce = setTimeout(() => { this.page.set(1); this.load(); }, 300);
  }

  protected prev() { if (this.page() > 1) { this.page.update((p) => p - 1); this.load(); } }
  protected next() { if (this.page() < this.totalPages()) { this.page.update((p) => p + 1); this.load(); } }

  private load() {
    this.loading.set(true);
    this.api.list(this.page(), this.pageSize, this.search).subscribe({
      next: (res) => { this.users.set(res.items); this.total.set(res.totalCount); this.loading.set(false); },
      error: (e) => { this.loading.set(false); this.toast.error(apiError(e)); },
    });
  }

  protected toggleAdmin(u: User) {
    const roles = this.isAdmin(u) ? u.roles.filter((r) => r !== 'Admin') : [...u.roles, 'Admin'];
    const finalRoles = roles.length ? roles : ['User'];
    this.api.setRoles(u.id, finalRoles).subscribe({
      next: () => { this.toast.success('Papéis atualizados.'); this.load(); },
      error: (e) => this.toast.error(apiError(e)),
    });
  }

  protected lock(u: User) {
    this.api.lock(u.id).subscribe({
      next: () => { this.toast.success('Usuário bloqueado.'); this.load(); },
      error: (e) => this.toast.error(apiError(e)),
    });
  }

  protected unlock(u: User) {
    this.api.unlock(u.id).subscribe({
      next: () => { this.toast.success('Usuário desbloqueado.'); this.load(); },
      error: (e) => this.toast.error(apiError(e)),
    });
  }

  protected remove(u: User) {
    if (u.email === this.auth.email()) { this.toast.error('Você não pode excluir a si mesmo.'); return; }
    if (!confirm(`Excluir o usuário ${u.email}?`)) return;
    this.api.remove(u.id).subscribe({
      next: () => { this.toast.success('Usuário excluído.'); this.load(); },
      error: (e) => this.toast.error(apiError(e)),
    });
  }
}
