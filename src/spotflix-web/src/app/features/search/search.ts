import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CatalogService } from '../../core/api/catalog.service';
import { ToastService } from '../../core/notify/toast.service';
import { MediaCard } from '../../shared/media-card/media-card';
import { SongRow } from '../../shared/song-row/song-row';
import { SearchResult } from '../../core/models';
import { PlayableSong } from '../../core/player/player.service';
import { apiError } from '../../core/util';

@Component({
  selector: 'app-search',
  imports: [FormsModule, MediaCard, SongRow],
  template: `
    <div class="searchbox">
      <span class="material-symbols-rounded">search</span>
      <input class="input" autofocus placeholder="O que você quer ouvir? (bandas e músicas)"
             [ngModel]="term()" (ngModelChange)="onInput($event)" />
      @if (term()) {
        <button class="clear" (click)="clear()"><span class="material-symbols-rounded">close</span></button>
      }
    </div>

    @if (loading()) {
      <p class="muted">Buscando…</p>
    } @else if (!hasTerm()) {
      <div class="empty"><span class="material-symbols-rounded">search</span><p>Digite ao menos 2 caracteres para buscar.</p></div>
    } @else if (isEmpty()) {
      <div class="empty"><span class="material-symbols-rounded">sentiment_dissatisfied</span><p>Nada encontrado para "{{ term() }}".</p></div>
    } @else {
      @let r = result();
      @if (r && r.songs.length) {
        <h2 class="section-title">Músicas</h2>
        <div class="songs">
          @for (s of songQueue(); track s.id; let i = $index) {
            <app-song-row [song]="s" [index]="i + 1" [queue]="songQueue()" />
          }
        </div>
      }
      @if (r && r.bands.length) {
        <h2 class="section-title">Bandas</h2>
        <div class="grid">
          @for (b of r.bands; track b.id) {
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
    .searchbox { position: relative; max-width: 560px; margin: 12px 0 26px; }
    .searchbox > .material-symbols-rounded { position: absolute; left: 14px; top: 50%; transform: translateY(-50%); color: var(--text-faint); }
    .searchbox .input { padding: 14px 44px; border-radius: 999px; font-size: 15px; }
    .clear { position: absolute; right: 8px; top: 50%; transform: translateY(-50%); background: none; border: none; color: var(--text-soft); cursor: pointer; display: grid; place-items: center; }
    .clear:hover { color: #fff; }
    .grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(180px, 1fr)); gap: 18px; margin-bottom: 28px; }
    .songs { margin-bottom: 28px; }
    .empty { display: grid; place-items: center; gap: 12px; padding: 70px 0; color: var(--text-soft); }
    .empty .material-symbols-rounded { font-size: 48px; color: var(--text-faint); }
  `],
})
export class Search {
  private catalog = inject(CatalogService);
  private toast = inject(ToastService);

  protected term = signal('');
  protected result = signal<SearchResult | null>(null);
  protected loading = signal(false);

  private debounce?: ReturnType<typeof setTimeout>;

  protected hasTerm = computed(() => this.term().trim().length >= 2);
  protected isEmpty = computed(() => {
    const r = this.result();
    return !!r && r.bands.length === 0 && r.songs.length === 0;
  });
  protected songQueue = computed<PlayableSong[]>(() =>
    (this.result()?.songs ?? []).map((s) => ({
      id: s.id, title: s.title, durationSeconds: s.durationSeconds, trackNumber: 0,
      albumId: s.albumId, bandName: s.bandName, albumTitle: s.albumTitle,
      hasAudio: s.hasAudio,
    })));

  protected onInput(value: string) {
    this.term.set(value);
    clearTimeout(this.debounce);
    const q = value.trim();
    if (q.length < 2) { this.result.set(null); return; }
    this.debounce = setTimeout(() => this.run(q), 300);
  }

  protected clear() { this.onInput(''); }

  private run(q: string) {
    this.loading.set(true);
    this.catalog.search(q, 20).subscribe({
      next: (r) => { this.result.set(r); this.loading.set(false); },
      error: (e) => { this.loading.set(false); this.toast.error(apiError(e)); },
    });
  }
}
