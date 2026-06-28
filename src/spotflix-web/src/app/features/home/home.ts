import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CatalogService } from '../../core/api/catalog.service';
import { AuthService } from '../../core/auth/auth.service';
import { ToastService } from '../../core/notify/toast.service';
import { MediaCard } from '../../shared/media-card/media-card';
import { Band, PagedResult } from '../../core/models';
import { apiError } from '../../core/util';

@Component({
  selector: 'app-home',
  imports: [FormsModule, MediaCard],
  template: `
    <section class="hero">
      <div>
        <p class="hello">{{ greeting() }}</p>
        <h1>Explore o catálogo</h1>
      </div>
      <div class="searchbox">
        <span class="material-symbols-rounded">search</span>
        <input class="input" placeholder="Filtrar bandas por nome…"
               [(ngModel)]="term" (input)="onSearch()" />
      </div>
    </section>

    @if (loading() && bands().length === 0) {
      <div class="grid">
        @for (i of skeletons; track i) { <div class="skel"></div> }
      </div>
    } @else if (bands().length === 0) {
      <div class="empty">
        <span class="material-symbols-rounded">music_off</span>
        <p>Nenhuma banda encontrada{{ term ? ' para "' + term + '"' : '' }}.</p>
      </div>
    } @else {
      <h2 class="section-title">Bandas</h2>
      <div class="grid">
        @for (b of bands(); track b.id) {
          <app-media-card
            [title]="b.name"
            [subtitle]="(b.genre || 'Banda') + ' · ' + b.albumCount + ' álbum(ns)'"
            [link]="['/bands', b.id]"
            [seed]="b.id"
            kind="band" />
        }
      </div>

      @if (hasMore()) {
        <div class="more">
          <button class="btn btn--ghost" (click)="loadMore()" [disabled]="loading()">
            {{ loading() ? 'Carregando…' : 'Carregar mais' }}
          </button>
        </div>
      }
    }
  `,
  styles: [`
    .hero { display: flex; align-items: flex-end; justify-content: space-between; gap: 24px; flex-wrap: wrap; margin: 12px 0 26px; }
    .hello { color: var(--text-soft); font-weight: 600; margin: 0 0 4px; }
    .hero h1 { font-size: 32px; }
    .searchbox { position: relative; min-width: 280px; }
    .searchbox .material-symbols-rounded { position: absolute; left: 12px; top: 50%; transform: translateY(-50%); color: var(--text-faint); }
    .searchbox .input { padding-left: 42px; border-radius: 999px; }
    .grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(180px, 1fr)); gap: 18px; }
    .skel { aspect-ratio: .82; border-radius: 12px; background: linear-gradient(90deg, #181820, #20202b, #181820); background-size: 200% 100%; animation: pulse 1.3s ease infinite; }
    @keyframes pulse { 0% { background-position: 200% 0; } 100% { background-position: -200% 0; } }
    .more { display: flex; justify-content: center; margin-top: 28px; }
    .empty { display: grid; place-items: center; gap: 12px; padding: 80px 0; color: var(--text-soft); }
    .empty .material-symbols-rounded { font-size: 48px; color: var(--text-faint); }
  `],
})
export class Home {
  private catalog = inject(CatalogService);
  private auth = inject(AuthService);
  private toast = inject(ToastService);

  protected term = '';
  protected bands = signal<Band[]>([]);
  protected loading = signal(false);
  protected page = signal(1);
  protected total = signal(0);
  protected readonly pageSize = 24;
  protected readonly skeletons = Array.from({ length: 12 }, (_, i) => i);

  private debounce?: ReturnType<typeof setTimeout>;

  constructor() { this.fetch(true); }

  protected greeting(): string {
    const name = this.auth.currentUser()?.fullName?.split(' ')[0];
    const h = new Date().getHours();
    const part = h < 12 ? 'Bom dia' : h < 18 ? 'Boa tarde' : 'Boa noite';
    return name ? `${part}, ${name}` : part;
  }

  protected hasMore() { return this.bands().length < this.total(); }

  protected onSearch() {
    clearTimeout(this.debounce);
    this.debounce = setTimeout(() => { this.page.set(1); this.fetch(true); }, 300);
  }

  protected loadMore() { this.page.update((p) => p + 1); this.fetch(false); }

  private fetch(reset: boolean) {
    this.loading.set(true);
    this.catalog.listBands(this.page(), this.pageSize, this.term).subscribe({
      next: (res: PagedResult<Band>) => {
        this.total.set(res.totalCount);
        this.bands.update((cur) => (reset ? res.items : [...cur, ...res.items]));
        this.loading.set(false);
      },
      error: (e) => { this.loading.set(false); this.toast.error(apiError(e)); },
    });
  }
}
