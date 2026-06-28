import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, tap, catchError, throwError } from 'rxjs';
import { Album, AlbumDetail, Band, BandDetail, PagedResult, SearchResult, Song } from '../models';
import { retryableHttpRequest } from '../util';

export interface BandInput {
  name: string;
  genre?: string | null;
  bio?: string | null;
  formedYear?: number | null;
}

@Injectable({ providedIn: 'root' })
export class CatalogService {
  private http = inject(HttpClient);

  // ---- Bands ----
  listBands(page = 1, pageSize = 24, search?: string): Observable<PagedResult<Band>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (search?.trim()) params = params.set('search', search.trim());
    return retryableHttpRequest(
      this.http.get<PagedResult<Band>>('/api/bands', { params }).pipe(
        tap(() => console.debug(`[CatalogService] Bands loaded: page ${page}`)),
        catchError((err) => {
          console.error('[CatalogService] Error loading bands', err.status);
          return throwError(() => err);
        }),
      ),
    );
  }

  getBand(id: string): Observable<BandDetail> {
    return retryableHttpRequest(
      this.http.get<BandDetail>(`/api/bands/${id}`).pipe(
        tap((band) => console.debug(`[CatalogService] Band detail loaded: ${band.name}`)),
        catchError((err) => {
          console.error('[CatalogService] Error loading band detail', err.status);
          return throwError(() => err);
        }),
      ),
    );
  }

  createBand(body: BandInput): Observable<Band> {
    return this.http.post<Band>('/api/bands', body);
  }

  updateBand(id: string, body: BandInput): Observable<void> {
    return this.http.put<void>(`/api/bands/${id}`, body);
  }

  deleteBand(id: string): Observable<void> {
    return this.http.delete<void>(`/api/bands/${id}`);
  }

  // ---- Albums ----
  getAlbum(id: string): Observable<AlbumDetail> {
    return this.http.get<AlbumDetail>(`/api/albums/${id}`);
  }

  createAlbum(bandId: string, body: { title: string; releaseYear?: number | null }): Observable<Album> {
    return this.http.post<Album>(`/api/bands/${bandId}/albums`, body);
  }

  updateAlbum(id: string, body: { title: string; releaseYear?: number | null }): Observable<void> {
    return this.http.put<void>(`/api/albums/${id}`, body);
  }

  deleteAlbum(id: string): Observable<void> {
    return this.http.delete<void>(`/api/albums/${id}`);
  }

  // ---- Songs ----
  createSong(albumId: string, body: { title: string; durationSeconds: number; trackNumber: number }): Observable<Song> {
    return this.http.post<Song>(`/api/albums/${albumId}/songs`, body);
  }

  updateSong(id: string, body: { title: string; durationSeconds: number; trackNumber: number }): Observable<void> {
    return this.http.put<void>(`/api/songs/${id}`, body);
  }

  deleteSong(id: string): Observable<void> {
    return this.http.delete<void>(`/api/songs/${id}`);
  }

  // ---- Search ----
  search(q: string, limit = 20): Observable<SearchResult> {
    const params = new HttpParams().set('q', q).set('limit', limit);
    return retryableHttpRequest(
      this.http.get<SearchResult>('/api/search', { params }).pipe(
        tap((result) => console.debug(`[CatalogService] Search results: ${result.bands.length} bands, ${result.songs.length} songs`)),
        catchError((err) => {
          console.error('[CatalogService] Error in search', err.status);
          return throwError(() => err);
        }),
      ),
    );
  }
}
