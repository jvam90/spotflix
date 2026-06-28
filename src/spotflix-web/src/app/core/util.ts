import { HttpErrorResponse } from '@angular/common/http';
import { Observable, retryWhen, delay, take } from 'rxjs';

/** Retry operator: waits 500ms between retries, max 2 attempts total */
export function retryableHttpRequest<T>(source: Observable<T>): Observable<T> {
  return source.pipe(
    retryWhen((errors) =>
      errors.pipe(
        delay(500),
        take(1),
      ),
    ),
  );
}

/** Formats seconds as m:ss (or h:mm:ss for long durations). */
export function formatDuration(totalSeconds: number): string {
  const s = Math.max(0, Math.floor(totalSeconds));
  const hrs = Math.floor(s / 3600);
  const mins = Math.floor((s % 3600) / 60);
  const secs = s % 60;
  const pad = (n: number) => n.toString().padStart(2, '0');
  return hrs > 0 ? `${hrs}:${pad(mins)}:${pad(secs)}` : `${mins}:${pad(secs)}`;
}

export function formatMoney(value: number): string {
  return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value ?? 0);
}

export function formatDate(value: string | null | undefined): string {
  if (!value) return '—';
  const d = new Date(value);
  return isNaN(d.getTime()) ? '—' : d.toLocaleDateString('pt-BR', { day: '2-digit', month: 'short', year: 'numeric' });
}

/** Pulls a human-readable message out of the API's varied error shapes. */
export function apiError(err: unknown, fallback = 'Algo deu errado. Tente novamente.'): string {
  if (err instanceof HttpErrorResponse) {
    const body = err.error;
    if (typeof body === 'string') return body;
    if (body?.message) return body.message;
    if (Array.isArray(body?.errors)) return body.errors.join(' ');
    if (Array.isArray(body?.violations) && body.violations.length) {
      return `${body.message ?? 'Operação recusada.'} (${body.violations.join(', ')})`;
    }
    if (err.status === 0) return 'Não foi possível conectar à API. Verifique se o backend está em execução.';
    if (err.status === 403) return 'Você não tem permissão para esta ação.';
  }
  return fallback;
}

/** Deterministic gradient cover from a string seed — used as artwork placeholder. */
export function gradientFor(seed: string): string {
  let hash = 0;
  for (let i = 0; i < seed.length; i++) hash = (hash * 31 + seed.charCodeAt(i)) | 0;
  const h1 = Math.abs(hash) % 360;
  const h2 = (h1 + 50) % 360;
  return `linear-gradient(135deg, hsl(${h1} 55% 38%), hsl(${h2} 60% 22%))`;
}

export function initials(text: string): string {
  const parts = text.trim().split(/\s+/).slice(0, 2);
  return parts.map((p) => p[0]?.toUpperCase() ?? '').join('') || '?';
}
