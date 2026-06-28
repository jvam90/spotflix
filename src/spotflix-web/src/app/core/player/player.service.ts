import { Injectable, computed, signal } from '@angular/core';
import { Song } from '../models';

/** A song enriched with display context for the player bar. */
export interface PlayableSong extends Song {
  bandName?: string;
  albumTitle?: string;
}

@Injectable({ providedIn: 'root' })
export class PlayerService {
  private queue = signal<PlayableSong[]>([]);
  private index = signal<number>(-1);

  readonly current = computed<PlayableSong | null>(() => this.queue()[this.index()] ?? null);
  readonly isPlaying = signal(false);
  readonly position = signal(0);
  readonly volume = signal(0.8);

  private readonly audio = new Audio();

  constructor() {
    this.audio.volume = this.volume();

    this.audio.addEventListener('timeupdate', () => {
      this.position.set(Math.floor(this.audio.currentTime));
    });

    this.audio.addEventListener('ended', () => {
      this.next();
    });

    this.audio.addEventListener('play', () => {
      this.isPlaying.set(true);
    });

    this.audio.addEventListener('pause', () => {
      this.isPlaying.set(false);
    });
  }

  playSong(song: PlayableSong, queue?: PlayableSong[]): void {
    if (!song.hasAudio) return;

    if (queue?.length) {
      this.queue.set(queue);
      this.index.set(Math.max(0, queue.findIndex((s) => s.id === song.id)));
    } else {
      this.queue.set([song]);
      this.index.set(0);
    }

    this.position.set(0);
    this.audio.src = `/api/songs/${song.id}/stream`;
    this.audio.play();
  }

  toggle(): void {
    if (!this.current()) return;
    this.audio.paused ? this.audio.play() : this.audio.pause();
  }

  next(): void {
    if (this.index() < this.queue().length - 1) {
      this.index.update((i) => i + 1);
      this.position.set(0);
      const next = this.current();
      if (next?.hasAudio) {
        this.audio.src = `/api/songs/${next.id}/stream`;
        this.audio.play();
      } else {
        this.isPlaying.set(false);
      }
    } else {
      this.audio.pause();
      this.position.set(0);
    }
  }

  previous(): void {
    if (this.position() > 3) {
      this.audio.currentTime = 0;
      return;
    }
    if (this.index() > 0) {
      this.index.update((i) => i - 1);
      this.position.set(0);
      const prev = this.current();
      if (prev?.hasAudio) {
        this.audio.src = `/api/songs/${prev.id}/stream`;
        this.audio.play();
      } else {
        this.isPlaying.set(false);
      }
    } else {
      this.audio.currentTime = 0;
      this.position.set(0);
    }
  }

  seek(seconds: number): void {
    const target = Math.max(0, seconds);
    this.audio.currentTime = target;
    this.position.set(target);
  }

  setVolume(v: number): void {
    const vol = Math.min(1, Math.max(0, v));
    this.volume.set(vol);
    this.audio.volume = vol;
  }
}
