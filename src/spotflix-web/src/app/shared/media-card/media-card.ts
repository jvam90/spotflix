import { Component, computed, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { gradientFor } from '../../core/util';

/** Square artwork card used in browse grids for bands and albums. */
@Component({
  selector: 'app-media-card',
  imports: [RouterLink],
  template: `
    <a class="mcard" [routerLink]="link()">
      <div class="art" [style.background]="cover()">
        <span class="material-symbols-rounded badge">{{ icon() }}</span>
        <button class="play" (click)="$event.preventDefault()">
          <span class="material-symbols-rounded">play_arrow</span>
        </button>
      </div>
      <div class="title">{{ title() }}</div>
      <div class="sub">{{ subtitle() }}</div>
    </a>
  `,
  styles: [`
    .mcard {
      display: block; background: var(--bg-card); border-radius: 12px; padding: 14px;
      transition: background .2s ease; cursor: pointer;
    }
    .mcard:hover { background: var(--bg-card-hover); }
    .mcard:hover .play { opacity: 1; transform: translateY(0); }
    .art {
      position: relative; width: 100%; aspect-ratio: 1; border-radius: 8px;
      margin-bottom: 14px; box-shadow: 0 8px 22px rgba(0,0,0,.4);
      display: grid; place-items: center; overflow: hidden;
    }
    .badge { font-size: 56px; color: rgba(255,255,255,.55); }
    .play {
      position: absolute; right: 10px; bottom: 10px;
      width: 46px; height: 46px; border-radius: 50%; border: none;
      background: var(--accent); color: #06130b; cursor: pointer;
      display: grid; place-items: center;
      opacity: 0; transform: translateY(8px);
      transition: opacity .2s, transform .2s, background .15s;
      box-shadow: var(--shadow);
    }
    .play:hover { background: var(--accent-hover); }
    .play .material-symbols-rounded { font-size: 28px; }
    .title { font-weight: 700; font-size: 15px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
    .sub { color: var(--text-soft); font-size: 13px; margin-top: 4px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
  `],
})
export class MediaCard {
  title = input.required<string>();
  subtitle = input<string>('');
  link = input.required<string | unknown[]>();
  seed = input<string>('');
  kind = input<'band' | 'album'>('band');

  protected cover = computed(() => gradientFor(this.seed() || this.title()));
  protected icon = computed(() => (this.kind() === 'band' ? 'groups' : 'album'));
}
