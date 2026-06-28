import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs';
import { CatalogService } from '../../core/api/catalog.service';
import { ToastService } from '../../core/notify/toast.service';
import { Album, Band, Song } from '../../core/models';
import { apiError, formatDuration } from '../../core/util';

type Dialog =
  | { type: 'band'; id?: string; name: string; genre: string; bio: string; formedYear: number | null }
  | { type: 'album'; id?: string; title: string; releaseYear: number | null }
  | { type: 'song'; id?: string; title: string; durationSeconds: number; trackNumber: number };

@Component({
  selector: 'app-admin-catalog',
  imports: [FormsModule],
  template: `
    <div class="bar"><h2>Catálogo</h2></div>

    <div class="toolbar">
      <input class="input" placeholder="Buscar banda…" [(ngModel)]="search" (input)="onSearch()" />
    </div>

    <div class="columns">
      <!-- Bands -->
      <div class="panel">
        <h3><span class="material-symbols-rounded">groups</span> Bandas
          <button class="icon-btn" style="margin-left:auto" (click)="newBand()"><span class="material-symbols-rounded">add</span></button>
        </h3>
        @for (b of bands(); track b.id) {
          <div class="list-row" [class.sel]="selectedBand()?.id === b.id" (click)="selectBand(b)">
            <div class="grow">
              <div class="name">{{ b.name }}</div>
              <div class="meta">{{ b.genre || 'Banda' }} · {{ b.albumCount }} álbum(ns)</div>
            </div>
            <button class="icon-btn" (click)="editBand(b, $event)"><span class="material-symbols-rounded">edit</span></button>
            <button class="icon-btn danger" (click)="deleteBand(b, $event)"><span class="material-symbols-rounded">delete</span></button>
          </div>
        } @empty { <p class="muted" style="padding:10px">Nenhuma banda.</p> }
      </div>

      <!-- Albums -->
      <div class="panel">
        <h3><span class="material-symbols-rounded">album</span> Álbuns
          @if (selectedBand()) {
            <button class="icon-btn" style="margin-left:auto" (click)="newAlbum()"><span class="material-symbols-rounded">add</span></button>
          }
        </h3>
        @if (!selectedBand()) {
          <p class="muted" style="padding:10px">Selecione uma banda.</p>
        } @else {
          @for (a of albums(); track a.id) {
            <div class="list-row" [class.sel]="selectedAlbum()?.id === a.id" (click)="selectAlbum(a)">
              <div class="grow">
                <div class="name">{{ a.title }}</div>
                <div class="meta">{{ a.releaseYear || '—' }} · {{ a.songCount }} faixa(s)</div>
              </div>
              <button class="icon-btn" (click)="editAlbum(a, $event)"><span class="material-symbols-rounded">edit</span></button>
              <button class="icon-btn danger" (click)="deleteAlbum(a, $event)"><span class="material-symbols-rounded">delete</span></button>
            </div>
          } @empty { <p class="muted" style="padding:10px">Sem álbuns.</p> }
        }
      </div>

      <!-- Songs -->
      <div class="panel">
        <h3><span class="material-symbols-rounded">music_note</span> Faixas
          @if (selectedAlbum()) {
            <button class="icon-btn" style="margin-left:auto" (click)="newSong()"><span class="material-symbols-rounded">add</span></button>
          }
        </h3>
        @if (!selectedAlbum()) {
          <p class="muted" style="padding:10px">Selecione um álbum.</p>
        } @else {
          @for (s of songs(); track s.id) {
            <div class="list-row">
              <span class="meta" style="width:22px">{{ s.trackNumber }}</span>
              <div class="grow"><div class="name">{{ s.title }}</div></div>
              <span class="meta">{{ formatDuration(s.durationSeconds) }}</span>
              <button class="icon-btn" (click)="editSong(s)"><span class="material-symbols-rounded">edit</span></button>
              <button class="icon-btn danger" (click)="deleteSong(s)"><span class="material-symbols-rounded">delete</span></button>
            </div>
          } @empty { <p class="muted" style="padding:10px">Sem faixas.</p> }
        }
      </div>
    </div>

    <!-- Dialogs -->
    @if (dialog(); as d) {
      <div class="overlay" (click)="dialog.set(null)">
        <div class="dialog" (click)="$event.stopPropagation()">
          @switch (d.type) {
            @case ('band') {
              <h3>{{ d.id ? 'Editar banda' : 'Nova banda' }}</h3>
              <div class="field"><label>Nome</label><input class="input" name="n" [(ngModel)]="d.name" /></div>
              <div class="row gap-12">
                <div class="field" style="flex:1"><label>Gênero</label><input class="input" name="g" [(ngModel)]="d.genre" /></div>
                <div class="field" style="flex:1"><label>Ano de formação</label><input class="input" type="number" name="fy" [(ngModel)]="d.formedYear" /></div>
              </div>
              <div class="field"><label>Bio</label><input class="input" name="b" [(ngModel)]="d.bio" /></div>
            }
            @case ('album') {
              <h3>{{ d.id ? 'Editar álbum' : 'Novo álbum' }}</h3>
              <div class="field"><label>Título</label><input class="input" name="t" [(ngModel)]="d.title" /></div>
              <div class="field"><label>Ano de lançamento</label><input class="input" type="number" name="ry" [(ngModel)]="d.releaseYear" /></div>
            }
            @case ('song') {
              <h3>{{ d.id ? 'Editar faixa' : 'Nova faixa' }}</h3>
              <div class="field"><label>Título</label><input class="input" name="st" [(ngModel)]="d.title" /></div>
              <div class="row gap-12">
                <div class="field" style="flex:1"><label>Faixa nº</label><input class="input" type="number" name="tn" [(ngModel)]="d.trackNumber" /></div>
                <div class="field" style="flex:1"><label>Duração (s)</label><input class="input" type="number" name="ds" [(ngModel)]="d.durationSeconds" /></div>
              </div>
            }
          }
          <div class="row gap-8" style="justify-content:flex-end; margin-top: 12px;">
            <button class="btn btn--ghost btn--sm" (click)="dialog.set(null)">Cancelar</button>
            <button class="btn btn--sm" [disabled]="saving()" (click)="save()">{{ saving() ? 'Salvando…' : 'Salvar' }}</button>
          </div>
        </div>
      </div>
    }
  `,
  styleUrl: './admin-shared.scss',
  styles: [`
    .columns { display: grid; grid-template-columns: repeat(3, 1fr); gap: 14px; align-items: start; }
    @media (max-width: 1000px) { .columns { grid-template-columns: 1fr; } }
  `],
})
export class AdminCatalog {
  private catalog = inject(CatalogService);
  private toast = inject(ToastService);

  protected bands = signal<Band[]>([]);
  protected albums = signal<Album[]>([]);
  protected songs = signal<Song[]>([]);
  protected selectedBand = signal<Band | null>(null);
  protected selectedAlbum = signal<Album | null>(null);

  protected dialog = signal<Dialog | null>(null);
  protected saving = signal(false);
  protected search = '';
  protected formatDuration = formatDuration;

  private debounce?: ReturnType<typeof setTimeout>;

  constructor() { this.loadBands(); }

  protected onSearch() {
    clearTimeout(this.debounce);
    this.debounce = setTimeout(() => this.loadBands(), 300);
  }

  private loadBands() {
    this.catalog.listBands(1, 50, this.search).subscribe({
      next: (res) => this.bands.set(res.items),
      error: (e) => this.toast.error(apiError(e)),
    });
  }

  protected selectBand(b: Band) {
    this.selectedBand.set(b);
    this.selectedAlbum.set(null);
    this.songs.set([]);
    this.catalog.getBand(b.id).subscribe({
      next: (d) => this.albums.set(d.albums),
      error: (e) => this.toast.error(apiError(e)),
    });
  }

  protected selectAlbum(a: Album) {
    this.selectedAlbum.set(a);
    this.catalog.getAlbum(a.id).subscribe({
      next: (d) => this.songs.set(d.songs),
      error: (e) => this.toast.error(apiError(e)),
    });
  }

  // ---- open dialogs ----
  protected newBand() { this.dialog.set({ type: 'band', name: '', genre: '', bio: '', formedYear: null }); }
  protected editBand(b: Band, e: Event) { e.stopPropagation(); this.dialog.set({ type: 'band', id: b.id, name: b.name, genre: b.genre ?? '', bio: '', formedYear: b.formedYear ?? null }); }
  protected newAlbum() { this.dialog.set({ type: 'album', title: '', releaseYear: null }); }
  protected editAlbum(a: Album, e: Event) { e.stopPropagation(); this.dialog.set({ type: 'album', id: a.id, title: a.title, releaseYear: a.releaseYear ?? null }); }
  protected newSong() { const next = this.songs().length + 1; this.dialog.set({ type: 'song', title: '', durationSeconds: 180, trackNumber: next }); }
  protected editSong(s: Song) { this.dialog.set({ type: 'song', id: s.id, title: s.title, durationSeconds: s.durationSeconds, trackNumber: s.trackNumber }); }

  protected save() {
    const d = this.dialog();
    if (!d) return;
    this.saving.set(true);
    const done = (msg: string) => { this.saving.set(false); this.dialog.set(null); this.toast.success(msg); };
    const fail = (e: unknown) => { this.saving.set(false); this.toast.error(apiError(e)); };

    if (d.type === 'band') {
      const body = { name: d.name, genre: d.genre || null, bio: d.bio || null, formedYear: d.formedYear ? Number(d.formedYear) : null };
      const op: Observable<unknown> = d.id ? this.catalog.updateBand(d.id, body) : this.catalog.createBand(body);
      op.subscribe({ next: () => { done('Banda salva.'); this.loadBands(); if (this.selectedBand()?.id === d.id) this.selectBand(this.selectedBand()!); }, error: fail });
    } else if (d.type === 'album') {
      const band = this.selectedBand();
      if (!band) { this.saving.set(false); return; }
      const body = { title: d.title, releaseYear: d.releaseYear ? Number(d.releaseYear) : null };
      const op: Observable<unknown> = d.id ? this.catalog.updateAlbum(d.id, body) : this.catalog.createAlbum(band.id, body);
      op.subscribe({ next: () => { done('Álbum salvo.'); this.selectBand(band); }, error: fail });
    } else {
      const album = this.selectedAlbum();
      if (!album) { this.saving.set(false); return; }
      const body = { title: d.title, durationSeconds: Number(d.durationSeconds), trackNumber: Number(d.trackNumber) };
      const op: Observable<unknown> = d.id ? this.catalog.updateSong(d.id, body) : this.catalog.createSong(album.id, body);
      op.subscribe({ next: () => { done('Faixa salva.'); this.selectAlbum(album); }, error: fail });
    }
  }

  protected deleteBand(b: Band, e: Event) {
    e.stopPropagation();
    if (!confirm(`Excluir a banda "${b.name}" e todo o seu conteúdo?`)) return;
    this.catalog.deleteBand(b.id).subscribe({
      next: () => {
        this.toast.success('Banda excluída.');
        if (this.selectedBand()?.id === b.id) { this.selectedBand.set(null); this.albums.set([]); this.songs.set([]); }
        this.loadBands();
      },
      error: (err) => this.toast.error(apiError(err)),
    });
  }

  protected deleteAlbum(a: Album, e: Event) {
    e.stopPropagation();
    if (!confirm(`Excluir o álbum "${a.title}"?`)) return;
    this.catalog.deleteAlbum(a.id).subscribe({
      next: () => {
        this.toast.success('Álbum excluído.');
        if (this.selectedAlbum()?.id === a.id) { this.selectedAlbum.set(null); this.songs.set([]); }
        if (this.selectedBand()) this.selectBand(this.selectedBand()!);
      },
      error: (err) => this.toast.error(apiError(err)),
    });
  }

  protected deleteSong(s: Song) {
    if (!confirm(`Excluir a faixa "${s.title}"?`)) return;
    this.catalog.deleteSong(s.id).subscribe({
      next: () => { this.toast.success('Faixa excluída.'); if (this.selectedAlbum()) this.selectAlbum(this.selectedAlbum()!); },
      error: (err) => this.toast.error(apiError(err)),
    });
  }
}
