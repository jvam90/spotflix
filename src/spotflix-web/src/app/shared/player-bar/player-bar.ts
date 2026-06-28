import { Component, computed, inject } from '@angular/core';
import { PlayerService } from '../../core/player/player.service';
import { formatDuration, gradientFor } from '../../core/util';

@Component({
  selector: 'app-player-bar',
  template: `
    <footer class="player">
      @let song = player.current();
      <div class="player__meta">
        @if (song) {
          <div class="cover" [style.background]="cover()"></div>
          <div class="info">
            <div class="title">{{ song.title }}</div>
            <div class="sub">{{ song.bandName || song.albumTitle || 'Spotflix' }}</div>
          </div>
        } @else {
          <div class="cover cover--empty"></div>
          <div class="info"><div class="sub">Nada tocando</div></div>
        }
      </div>

      <div class="player__controls">
        <div class="buttons">
          <button class="ctrl" (click)="player.previous()" [disabled]="!song" title="Anterior">
            <span class="material-symbols-rounded">skip_previous</span>
          </button>
          <button class="ctrl ctrl--play" (click)="player.toggle()" [disabled]="!song" title="Tocar/Pausar">
            <span class="material-symbols-rounded">{{ player.isPlaying() ? 'pause' : 'play_arrow' }}</span>
          </button>
          <button class="ctrl" (click)="player.next()" [disabled]="!song" title="Próxima">
            <span class="material-symbols-rounded">skip_next</span>
          </button>
        </div>
        <div class="seek">
          <span class="time">{{ pos() }}</span>
          <input class="bar" type="range" min="0" [max]="duration()" [value]="player.position()"
                 [disabled]="!song" (input)="onSeek($event)" />
          <span class="time">{{ dur() }}</span>
        </div>
      </div>

      <div class="player__volume">
        <span class="material-symbols-rounded">{{ player.volume() === 0 ? 'volume_off' : 'volume_up' }}</span>
        <input class="bar bar--vol" type="range" min="0" max="1" step="0.01"
               [value]="player.volume()" (input)="onVolume($event)" />
      </div>
    </footer>
  `,
  styles: [`
    .player {
      height: 84px; flex: 0 0 84px;
      background: var(--bg-elevated); border-top: 1px solid var(--border);
      display: grid; grid-template-columns: 1fr 2fr 1fr; align-items: center;
      padding: 0 18px; gap: 16px;
    }
    .player__meta { display: flex; align-items: center; gap: 12px; min-width: 0; }
    .cover { width: 52px; height: 52px; border-radius: 8px; flex: 0 0 auto; }
    .cover--empty { background: #23232e; }
    .info { min-width: 0; }
    .title { font-weight: 600; font-size: 14px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
    .sub { color: var(--text-soft); font-size: 12px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }

    .player__controls { display: flex; flex-direction: column; align-items: center; gap: 6px; }
    .buttons { display: flex; align-items: center; gap: 14px; }
    .ctrl {
      background: none; border: none; color: var(--text-soft); cursor: pointer;
      display: grid; place-items: center; transition: color .15s, transform .1s;
    }
    .ctrl:hover:not(:disabled) { color: #fff; }
    .ctrl:disabled { opacity: .4; cursor: default; }
    .ctrl--play {
      background: #fff; color: #000; width: 38px; height: 38px; border-radius: 50%;
    }
    .ctrl--play:hover:not(:disabled) { transform: scale(1.06); color: #000; }
    .ctrl--play .material-symbols-rounded { font-size: 24px; }

    .seek { display: flex; align-items: center; gap: 10px; width: 100%; max-width: 540px; }
    .time { font-size: 11px; color: var(--text-soft); width: 38px; text-align: center; }
    .bar {
      -webkit-appearance: none; appearance: none; height: 4px; border-radius: 4px;
      background: #4a4a55; flex: 1; cursor: pointer;
    }
    .bar::-webkit-slider-thumb {
      -webkit-appearance: none; width: 12px; height: 12px; border-radius: 50%;
      background: #fff; cursor: pointer;
    }
    .player__volume { display: flex; align-items: center; gap: 8px; justify-content: flex-end; color: var(--text-soft); }
    .bar--vol { max-width: 110px; flex: none; width: 110px; }
  `],
})
export class PlayerBar {
  protected player = inject(PlayerService);

  protected cover = computed(() => gradientFor(this.player.current()?.id ?? 'empty'));
  protected duration = computed(() => this.player.current()?.durationSeconds ?? 0);
  protected pos = computed(() => formatDuration(this.player.position()));
  protected dur = computed(() => formatDuration(this.duration()));

  protected onSeek(e: Event) { this.player.seek(Number((e.target as HTMLInputElement).value)); }
  protected onVolume(e: Event) { this.player.setVolume(Number((e.target as HTMLInputElement).value)); }
}
