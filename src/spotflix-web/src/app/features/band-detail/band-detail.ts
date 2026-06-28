import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CatalogService } from '../../core/api/catalog.service';
import { FavoritesService } from '../../core/api/favorites.service';
import { AuthService } from '../../core/auth/auth.service';
import { ToastService } from '../../core/notify/toast.service';
import { MediaCard } from '../../shared/media-card/media-card';
import { BandDetail as BandDetailModel } from '../../core/models';
import { apiError, gradientFor } from '../../core/util';

@Component({
  selector: 'app-band-detail',
  imports: [MediaCard],
  template: `
    @let band = data();
    @if (band) {
      <header class="head" [style.background]="cover()">
        <div class="head__art" [style.background]="cover()">
          <span class="material-symbols-rounded">groups</span>
        </div>
        <div class="head__info">
          <span class="chip">Banda</span>
          <h1>{{ band.name }}</h1>
          <p class="meta">
            {{ band.genre || 'Gênero diverso' }}
            @if (band.formedYear) { · desde {{ band.formedYear }} }
            · {{ band.albums.length }} álbum(ns)
          </p>
        </div>
      </header>

      <div class="actions">
        @if (auth.isAuthenticated()) {
          <button class="btn" [class.btn--ghost]="isFav()" (click)="toggleFav()">
            <span class="material-symbols-rounded">{{ isFav() ? 'favorite' : 'favorite_border' }}</span>
            {{ isFav() ? 'Seguindo' : 'Seguir banda' }}
          </button>
        }
      </div>

      @if (band.bio) { <p class="bio">{{ band.bio }}</p> }

      <h2 class="section-title">Álbuns</h2>
      @if (band.albums.length === 0) {
        <p class="muted">Esta banda ainda não tem álbuns cadastrados.</p>
      } @else {
        <div class="grid">
          @for (a of band.albums; track a.id) {
            <app-media-card
              [title]="a.title"
              [subtitle]="(a.releaseYear || '—') + ' · ' + a.songCount + ' faixa(s)'"
              [link]="['/albums', a.id]"
              [seed]="a.id"
              kind="album" />
          }
        </div>
      }
    } @else if (error()) {
      <div class="empty"><span class="material-symbols-rounded">error</span><p>{{ error() }}</p></div>
    } @else {
      <p class="muted">Carregando…</p>
    }
  `,
  styles: [`
    .head { display: flex; align-items: flex-end; gap: 24px; padding: 40px 24px; border-radius: 16px; margin-bottom: 8px; }
    .head__art { width: 150px; height: 150px; border-radius: 50%; display: grid; place-items: center; box-shadow: var(--shadow); flex: 0 0 auto; }
    .head__art .material-symbols-rounded { font-size: 72px; color: rgba(255,255,255,.7); }
    .head h1 { font-size: 48px; margin: 8px 0; letter-spacing: -0.04em; }
    .meta { color: var(--text-soft); font-weight: 500; }
    .actions { padding: 18px 4px; }
    .bio { color: var(--text-soft); max-width: 720px; line-height: 1.6; margin: 0 0 20px; }
    .grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(180px, 1fr)); gap: 18px; }
    .empty { display: grid; place-items: center; gap: 10px; padding: 80px 0; color: var(--text-soft); }
    .empty .material-symbols-rounded { font-size: 44px; }
  `],
})
export class BandDetail {
  private route = inject(ActivatedRoute);
  private catalog = inject(CatalogService);
  private favorites = inject(FavoritesService);
  protected auth = inject(AuthService);
  private toast = inject(ToastService);

  protected data = signal<BandDetailModel | null>(null);
  protected error = signal('');

  protected cover = computed(() => gradientFor(this.data()?.id ?? 'band'));
  protected isFav = computed(() => !!this.data() && this.favorites.bandIds().has(this.data()!.id));

  constructor() {
    this.route.paramMap.subscribe((p) => {
      const id = p.get('id');
      if (id) this.load(id);
    });
  }

  private load(id: string) {
    this.data.set(null);
    this.error.set('');
    this.catalog.getBand(id).subscribe({
      next: (b) => this.data.set(b),
      error: (e) => this.error.set(apiError(e, 'Banda não encontrada.')),
    });
  }

  protected toggleFav() {
    const band = this.data();
    if (!band) return;
    const fav = this.isFav();
    const op = fav ? this.favorites.removeBand(band.id) : this.favorites.addBand(band.id);
    op.subscribe({
      next: () => this.toast.success(fav ? 'Deixou de seguir.' : 'Seguindo a banda!'),
      error: (e) => this.toast.error(apiError(e)),
    });
  }
}
