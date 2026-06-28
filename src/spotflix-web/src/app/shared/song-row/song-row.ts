import { Component, computed, inject, input } from '@angular/core';
import { AuthService } from '../../core/auth/auth.service';
import { FavoritesService } from '../../core/api/favorites.service';
import { PlayerService, PlayableSong } from '../../core/player/player.service';
import { ToastService } from '../../core/notify/toast.service';
import { apiError, formatDuration } from '../../core/util';

@Component({
  selector: 'app-song-row',
  template: `
    @let s = song();
    <div class="song" [class.playing]="isCurrent()" [class.no-audio]="!s.hasAudio" (dblclick)="play()">
      <div class="idx">
        <span class="num">{{ index() }}</span>
        <button class="play" (click)="play()" [title]="s.hasAudio ? 'Tocar' : 'Sem áudio'" [disabled]="!s.hasAudio">
          <span class="material-symbols-rounded">{{ isCurrent() && player.isPlaying() ? 'pause' : 'play_arrow' }}</span>
        </button>
      </div>
      <div class="meta">
        <div class="title">{{ s.title }}</div>
        @if (s.bandName || s.albumTitle) {
          <div class="sub">{{ s.bandName }}{{ s.bandName && s.albumTitle ? ' · ' : '' }}{{ s.albumTitle }}</div>
        }
      </div>
      @if (auth.isAuthenticated()) {
        <button class="fav" [class.on]="isFav()" (click)="toggleFav()" title="Favoritar">
          <span class="material-symbols-rounded">{{ isFav() ? 'favorite' : 'favorite_border' }}</span>
        </button>
      }
      <div class="dur">{{ duration() }}</div>
    </div>
  `,
  styles: [`
    .song {
      display: grid; grid-template-columns: 36px 1fr auto auto; align-items: center;
      gap: 14px; padding: 8px 12px; border-radius: 6px; cursor: default;
    }
    .song:hover { background: var(--bg-card-hover); }
    .song:hover .num { display: none; }
    .song:hover .play { display: grid; }
    .song.playing .title { color: var(--accent); }
    .idx { display: grid; place-items: center; }
    .num { color: var(--text-soft); font-size: 14px; font-variant-numeric: tabular-nums; }
    .play { display: none; background: none; border: none; color: #fff; cursor: pointer; place-items: center; }
    .no-audio .title { opacity: 0.45; }
    .no-audio .play { opacity: 0.3; cursor: default; }
    .meta { min-width: 0; }
    .title { font-size: 15px; font-weight: 500; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
    .sub { font-size: 13px; color: var(--text-soft); white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
    .fav { background: none; border: none; color: var(--text-soft); cursor: pointer; display: grid; place-items: center; }
    .fav:hover { color: #fff; }
    .fav.on { color: var(--accent); }
    .dur { color: var(--text-soft); font-size: 13px; font-variant-numeric: tabular-nums; min-width: 42px; text-align: right; }
  `],
})
export class SongRow {
  song = input.required<PlayableSong>();
  index = input.required<number>();
  queue = input<PlayableSong[]>([]);

  protected auth = inject(AuthService);
  protected player = inject(PlayerService);
  private favorites = inject(FavoritesService);
  private toast = inject(ToastService);

  protected duration = computed(() => formatDuration(this.song().durationSeconds));
  protected isCurrent = computed(() => this.player.current()?.id === this.song().id);
  protected isFav = computed(() => this.favorites.songIds().has(this.song().id));

  protected play() {
    const q = this.queue();
    this.player.playSong(this.song(), q.length ? q : [this.song()]);
  }

  protected toggleFav() {
    const s = this.song();
    const fav = this.isFav();
    const op = fav ? this.favorites.removeSong(s.id) : this.favorites.addSong(s.id);
    op.subscribe({
      next: () => this.toast.success(fav ? 'Removido dos favoritos.' : 'Adicionado aos favoritos.'),
      error: (e) => this.toast.error(apiError(e)),
    });
  }
}
