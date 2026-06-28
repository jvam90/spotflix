import { Injectable, computed, signal } from '@angular/core';
import { Song } from '../models';

/** A song enriched with display context for the player bar. */
export interface PlayableSong extends Song {
  bandName?: string;
  albumTitle?: string;
}

/**
 * Simulated playback engine. The catalog stores metadata only (no audio files),
 * so this advances a virtual playhead with a timer to drive the player UI.
 */
@Injectable({ providedIn: 'root' })
export class PlayerService {
  private queue = signal<PlayableSong[]>([]);
  private index = signal<number>(-1);

  readonly current = computed<PlayableSong | null>(() => this.queue()[this.index()] ?? null);
  readonly isPlaying = signal(false);
  readonly position = signal(0); // seconds into the current track
  readonly volume = signal(0.8);

  private ticker?: ReturnType<typeof setInterval>;

  playSong(song: PlayableSong, queue?: PlayableSong[]): void {
    if (queue?.length) {
      this.queue.set(queue);
      this.index.set(Math.max(0, queue.findIndex((s) => s.id === song.id)));
    } else {
      this.queue.set([song]);
      this.index.set(0);
    }
    this.position.set(0);
    this.start();
  }

  toggle(): void {
    if (!this.current()) return;
    this.isPlaying() ? this.pause() : this.start();
  }

  next(): void {
    if (this.index() < this.queue().length - 1) {
      this.index.update((i) => i + 1);
      this.position.set(0);
      this.start();
    } else {
      this.pause();
      this.position.set(0);
    }
  }

  previous(): void {
    if (this.position() > 3) { this.position.set(0); return; }
    if (this.index() > 0) {
      this.index.update((i) => i - 1);
      this.position.set(0);
      this.start();
    } else {
      this.position.set(0);
    }
  }

  seek(seconds: number): void {
    this.position.set(Math.max(0, seconds));
  }

  setVolume(v: number): void {
    this.volume.set(Math.min(1, Math.max(0, v)));
  }

  private start(): void {
    this.isPlaying.set(true);
    this.stopTicker();
    this.ticker = setInterval(() => {
      const cur = this.current();
      if (!cur) { this.pause(); return; }
      const nextPos = this.position() + 1;
      if (nextPos >= cur.durationSeconds) {
        this.next();
      } else {
        this.position.set(nextPos);
      }
    }, 1000);
  }

  private pause(): void {
    this.isPlaying.set(false);
    this.stopTicker();
  }

  private stopTicker(): void {
    if (this.ticker) { clearInterval(this.ticker); this.ticker = undefined; }
  }
}
