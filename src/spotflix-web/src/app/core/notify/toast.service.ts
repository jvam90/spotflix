import { Injectable, signal } from '@angular/core';

export type ToastKind = 'success' | 'error' | 'info';

export interface Toast {
  id: number;
  kind: ToastKind;
  text: string;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private seq = 0;
  readonly toasts = signal<Toast[]>([]);
  private timeouts = new Map<number, ReturnType<typeof setTimeout>>();

  success(text: string) { this.push('success', text); }
  error(text: string) { this.push('error', text); }
  info(text: string) { this.push('info', text); }

  dismiss(id: number) {
    const timeout = this.timeouts.get(id);
    if (timeout) clearTimeout(timeout);
    this.timeouts.delete(id);
    this.toasts.update((list) => list.filter((t) => t.id !== id));
  }

  private push(kind: ToastKind, text: string) {
    const id = ++this.seq;
    console.log(`[ToastService] ${kind.toUpperCase()}: ${text}`);
    this.toasts.update((list) => [...list, { id, kind, text }]);

    const timeout = setTimeout(() => this.dismiss(id), 4200);
    this.timeouts.set(id, timeout);
  }
}
