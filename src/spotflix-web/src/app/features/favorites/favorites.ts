import { Component, computed, inject, signal } from '@angular/core';
import { FavoritesService } from '../../core/api/favorites.service';
import { ToastService } from '../../core/notify/toast.service';
import { MediaCard } from '../../shared/media-card/media-card';
import { SongRow } from '../../shared/song-row/song-row';
import { PlayableSong } from '../../core/player/player.service';
import { apiError } from '../../core/util';

@Component({
  selector: 'app-favorites',
  imports: [MediaCard, SongRow],
  template: `
    <header class="head">
      <div class="head__art"><span class="material-symbols-rounded">favorite</span></div>
      <div>
        <span class="chip">Coleção</span>
        <h1>Seus favoritos</h1>
        <p class="muted">{{ songs().length }} música(s) · {{ bands().length }} banda(s)</p>
      </div>
    </header>

    <div class="tabs">
      <button [class.active]="tab() === 'songs'" (click)="tab.set('songs')">Músicas</button>
      <button [class.active]="tab() === 'bands'" (click)="tab.set('bands')">Bandas</button>
    </div>

    @if (loading()) {
      <p class="muted">Carregando…</p>
    } @else if (tab() === 'songs') {
      @if (songs().length === 0) {
        <div class="empty"><span class="material-symbols-rounded">music_note</span><p>Você ainda não favoritou músicas.</p></div>
      } @else {
        <div class="songs">
          @for (s of songQueue(); track s.id; let i = $index) {
            <app-song-row [song]="s" [index]="i + 1" [queue]="songQueue()" />
          }
        </div>
      }
    } @else {
      @if (bands().length === 0) {
        <div class="empty"><span class="material-symbols-rounded">groups</span><p>Você ainda não segue bandas.</p></div>
      } @else {
        <div class="grid">
          @for (b of bands(); track b.id) {
            <app-media-card
              [title]="b.name"
              [subtitle]="(b.genre || 'Banda') + ' · ' + b.albumCount + ' álbum(ns)'"
              [link]="['/bands', b.id]" [seed]="b.id" kind="band" />
          }
        </div>
      }
    }
  `,
  styles: [`
    .head { display: flex; align-items: flex-end; gap: 24px; padding: 28px 12px; }
    .head__art { width: 150px; height: 150px; border-radius: 12px; display: grid; place-items: center;
      background: linear-gradient(135deg, #4b1f8f, #c13584); box-shadow: var(--shadow); }
    .head__art .material-symbols-rounded { font-size: 70px; color: rgba(255,255,255,.85); }
    .head h1 { font-size: 44px; margin: 8px 0 6px; }
    .tabs { display: flex; gap: 8px; margin: 8px 12px 22px; }
    .tabs button {
      background: #23232e; border: none; color: var(--text-soft); cursor: pointer;
      padding: 8px 18px; border-radius: 999px; font: inherit; font-weight: 600; font-size: 14px;
    }
    .tabs button.active { background: #fff; color: #000; }
    .songs { padding: 0 12px; }
    .grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(180px, 1fr)); gap: 18px; padding: 0 12px; }
    .empty { display: grid; place-items: center; gap: 12px; padding: 70px 0; color: var(--text-soft); }
    .empty .material-symbols-rounded { font-size: 48px; color: var(--text-faint); }
  `],
})
export class Favorites {
  private favorites = inject(FavoritesService);
  private toast = inject(ToastService);

  protected tab = signal<'songs' | 'bands'>('songs');
  protected loading = signal(true);

  protected songs = this.favorites.favoriteSongs;
  protected bands = this.favorites.favoriteBands;

  protected songQueue = computed<PlayableSong[]>(() => this.songs().map((s) => ({ ...s })));

  constructor() {
    let pending = 2;
    const done = () => { if (--pending === 0) this.loading.set(false); };
    this.favorites.loadSongs().subscribe({ next: done, error: (e) => { this.toast.error(apiError(e)); done(); } });
    this.favorites.loadBands().subscribe({ next: done, error: (e) => { this.toast.error(apiError(e)); done(); } });
  }
}
