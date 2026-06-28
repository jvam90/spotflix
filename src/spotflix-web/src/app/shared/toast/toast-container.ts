import { Component, inject } from '@angular/core';
import { ToastService } from '../../core/notify/toast.service';

@Component({
  selector: 'app-toast-container',
  template: `
    <div class="toasts">
      @for (t of toast.toasts(); track t.id) {
        <div class="toast toast--{{ t.kind }}" (click)="toast.dismiss(t.id)">
          <span class="material-symbols-rounded">
            {{ t.kind === 'success' ? 'check_circle' : t.kind === 'error' ? 'error' : 'info' }}
          </span>
          <span class="toast__text">{{ t.text }}</span>
        </div>
      }
    </div>
  `,
  styles: [`
    .toasts {
      position: fixed; right: 20px; bottom: 100px; z-index: 9999;
      display: flex; flex-direction: column; gap: 10px; max-width: 360px;
    }
    .toast {
      display: flex; align-items: center; gap: 10px;
      padding: 13px 16px; border-radius: 10px; cursor: pointer;
      background: #23232e; color: #fff; box-shadow: var(--shadow);
      border-left: 4px solid var(--text-faint);
      animation: slide-in .22s ease;
      font-size: 14px;
    }
    .toast--success { border-left-color: var(--accent); }
    .toast--success .material-symbols-rounded { color: var(--accent); }
    .toast--error { border-left-color: var(--danger); }
    .toast--error .material-symbols-rounded { color: var(--danger); }
    .toast--info { border-left-color: #4a9eff; }
    .toast--info .material-symbols-rounded { color: #4a9eff; }
    .toast__text { flex: 1; }
    @keyframes slide-in { from { opacity: 0; transform: translateX(30px); } to { opacity: 1; transform: none; } }
  `],
})
export class ToastContainer {
  protected toast = inject(ToastService);
}
