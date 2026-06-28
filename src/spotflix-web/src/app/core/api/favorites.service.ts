import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Band, Song } from '../models';

@Injectable({ providedIn: 'root' })
export class FavoritesService {
  private http = inject(HttpClient);

  // Cached id sets so the UI can render heart state instantly across pages.
  readonly songIds = signal<Set<string>>(new Set());
  readonly bandIds = signal<Set<string>>(new Set());

  readonly favoriteSongs = signal<Song[]>([]);
  readonly favoriteBands = signal<Band[]>([]);

  isSongFavorite = (id: string) => computed(() => this.songIds().has(id));
  isBandFavorite = (id: string) => computed(() => this.bandIds().has(id));

  loadSongs(): Observable<Song[]> {
    return this.http.get<Song[]>('/api/favorites/songs').pipe(
      tap((songs) => {
        this.favoriteSongs.set(songs);
        this.songIds.set(new Set(songs.map((s) => s.id)));
      }),
    );
  }

  loadBands(): Observable<Band[]> {
    return this.http.get<Band[]>('/api/favorites/bands').pipe(
      tap((bands) => {
        this.favoriteBands.set(bands);
        this.bandIds.set(new Set(bands.map((b) => b.id)));
      }),
    );
  }

  addSong(songId: string): Observable<void> {
    return this.http.put<void>(`/api/favorites/songs/${songId}`, {}).pipe(
      tap(() => this.songIds.update((s) => new Set(s).add(songId))),
    );
  }

  removeSong(songId: string): Observable<void> {
    return this.http.delete<void>(`/api/favorites/songs/${songId}`).pipe(
      tap(() => {
        this.songIds.update((s) => { const n = new Set(s); n.delete(songId); return n; });
        this.favoriteSongs.update((list) => list.filter((x) => x.id !== songId));
      }),
    );
  }

  addBand(bandId: string): Observable<void> {
    return this.http.put<void>(`/api/favorites/bands/${bandId}`, {}).pipe(
      tap(() => this.bandIds.update((s) => new Set(s).add(bandId))),
    );
  }

  removeBand(bandId: string): Observable<void> {
    return this.http.delete<void>(`/api/favorites/bands/${bandId}`).pipe(
      tap(() => {
        this.bandIds.update((s) => { const n = new Set(s); n.delete(bandId); return n; });
        this.favoriteBands.update((list) => list.filter((x) => x.id !== bandId));
      }),
    );
  }

  reset(): void {
    this.songIds.set(new Set());
    this.bandIds.set(new Set());
    this.favoriteSongs.set([]);
    this.favoriteBands.set([]);
  }
}
