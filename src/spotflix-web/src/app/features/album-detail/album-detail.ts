import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CatalogService } from '../../core/api/catalog.service';
import { ToastService } from '../../core/notify/toast.service';
import { SongRow } from '../../shared/song-row/song-row';
import { PlayerService, PlayableSong } from '../../core/player/player.service';
import { AlbumDetail as AlbumDetailModel } from '../../core/models';
import { apiError, formatDuration, gradientFor } from '../../core/util';

@Component({
  selector: 'app-album-detail',
  imports: [RouterLink, SongRow],
  template: `
    @let album = data();
    @if (album) {
      <header class="head">
        <div class="head__art" [style.background]="cover()">
          <span class="material-symbols-rounded">album</span>
        </div>
        <div class="head__info">
          <span class="chip">Álbum</span>
          <h1>{{ album.title }}</h1>
          <p class="meta">
            <a class="band" [routerLink]="['/bands', album.bandId]">{{ album.bandName }}</a>
            @if (album.releaseYear) { · {{ album.releaseYear }} }
            · {{ album.songs.length }} faixa(s) · {{ totalTime() }}
          </p>
        </div>
      </header>

      <div class="actions">
        <button class="btn btn--play" (click)="playAll()" [disabled]="album.songs.length === 0">
          <span class="material-symbols-rounded">play_arrow</span> Reproduzir
        </button>
      </div>

      @if (album.songs.length === 0) {
        <p class="muted">Nenhuma faixa cadastrada neste álbum.</p>
      } @else {
        <div class="tracklist">
          <div class="track-head">
            <span>#</span><span>Título</span><span></span>
            <span class="material-symbols-rounded">schedule</span>
          </div>
          @for (s of queue(); track s.id; let i = $index) {
            <app-song-row [song]="s" [index]="i + 1" [queue]="queue()" />
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
    .head { display: flex; align-items: flex-end; gap: 24px; padding: 32px 24px 24px; }
    .head__art { width: 170px; height: 170px; border-radius: 10px; display: grid; place-items: center; box-shadow: var(--shadow); flex: 0 0 auto; }
    .head__art .material-symbols-rounded { font-size: 80px; color: rgba(255,255,255,.7); }
    .head h1 { font-size: 46px; margin: 8px 0; letter-spacing: -0.04em; }
    .meta { color: var(--text-soft); font-weight: 500; }
    .band { color: #fff; font-weight: 700; }
    .band:hover { text-decoration: underline; }
    .actions { padding: 8px 24px 18px; }
    .btn--play { padding: 14px 28px; font-size: 15px; }
    .btn--play .material-symbols-rounded { font-size: 22px; }
    .tracklist { padding: 0 12px; }
    .track-head {
      display: grid; grid-template-columns: 36px 1fr auto 42px; gap: 14px;
      padding: 8px 12px; color: var(--text-soft); font-size: 12px; text-transform: uppercase; letter-spacing: .06em;
      border-bottom: 1px solid var(--border); margin-bottom: 8px;
    }
    .track-head .material-symbols-rounded { font-size: 18px; justify-self: end; }
    .empty { display: grid; place-items: center; gap: 10px; padding: 80px 0; color: var(--text-soft); }
    .empty .material-symbols-rounded { font-size: 44px; }
  `],
})
export class AlbumDetail {
  private route = inject(ActivatedRoute);
  private catalog = inject(CatalogService);
  private player = inject(PlayerService);
  private toast = inject(ToastService);

  protected data = signal<AlbumDetailModel | null>(null);
  protected error = signal('');

  protected cover = computed(() => gradientFor(this.data()?.id ?? 'album'));
  protected queue = computed<PlayableSong[]>(() => {
    const a = this.data();
    if (!a) return [];
    return a.songs.map((s) => ({ ...s, bandName: a.bandName, albumTitle: a.title }));
  });
  protected totalTime = computed(() =>
    formatDuration(this.queue().reduce((sum, s) => sum + s.durationSeconds, 0)));

  constructor() {
    this.route.paramMap.subscribe((p) => {
      const id = p.get('id');
      if (id) this.load(id);
    });
  }

  private load(id: string) {
    this.data.set(null);
    this.error.set('');
    this.catalog.getAlbum(id).subscribe({
      next: (a) => this.data.set(a),
      error: (e) => this.error.set(apiError(e, 'Álbum não encontrado.')),
    });
  }

  protected playAll() {
    const q = this.queue();
    if (q.length) this.player.playSong(q[0], q);
  }
}
