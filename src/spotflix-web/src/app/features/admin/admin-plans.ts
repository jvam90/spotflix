import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs';
import { BillingService } from '../../core/api/billing.service';
import { ToastService } from '../../core/notify/toast.service';
import { BillingPeriod, Plan } from '../../core/models';
import { apiError, formatMoney } from '../../core/util';

interface PlanForm {
  id?: string;
  name: string;
  price: number;
  period: BillingPeriod;
  description: string;
  isActive: boolean;
}

@Component({
  selector: 'app-admin-plans',
  imports: [FormsModule],
  template: `
    <div class="bar">
      <h2>Planos</h2>
      <button class="btn btn--sm" (click)="startNew()"><span class="material-symbols-rounded">add</span> Novo plano</button>
    </div>

    @if (loading()) {
      <p class="muted">Carregando…</p>
    } @else {
      <table class="grid-table">
        <thead><tr><th>Nome</th><th>Preço</th><th>Período</th><th>Ativo</th><th></th></tr></thead>
        <tbody>
          @for (p of plans(); track p.id) {
            <tr>
              <td>{{ p.name }}</td>
              <td>{{ p.price === 0 ? 'Grátis' : formatMoney(p.price) }}</td>
              <td>{{ p.period === 2 ? 'Anual' : 'Mensal' }}</td>
              <td><span class="dot" [class.on]="p.isActive"></span>{{ p.isActive ? 'Sim' : 'Não' }}</td>
              <td class="right"><button class="btn btn--ghost btn--sm" (click)="edit(p)">Editar</button></td>
            </tr>
          }
        </tbody>
      </table>
    }

    @if (form(); as f) {
      <div class="overlay" (click)="form.set(null)">
        <div class="dialog" (click)="$event.stopPropagation()">
          <h3>{{ f.id ? 'Editar plano' : 'Novo plano' }}</h3>
          <div class="field"><label>Nome</label><input class="input" name="n" [(ngModel)]="f.name" /></div>
          <div class="row gap-12">
            <div class="field" style="flex:1"><label>Preço</label><input class="input" type="number" name="pr" [(ngModel)]="f.price" /></div>
            <div class="field" style="flex:1">
              <label>Período</label>
              <select class="select" name="pe" [(ngModel)]="f.period">
                <option [ngValue]="1">Mensal</option>
                <option [ngValue]="2">Anual</option>
              </select>
            </div>
          </div>
          <div class="field"><label>Descrição</label><input class="input" name="d" [(ngModel)]="f.description" /></div>
          @if (f.id) {
            <label class="check"><input type="checkbox" name="a" [(ngModel)]="f.isActive" /> Plano ativo</label>
          }
          <div class="row gap-8" style="justify-content:flex-end; margin-top: 12px;">
            <button class="btn btn--ghost btn--sm" (click)="form.set(null)">Cancelar</button>
            <button class="btn btn--sm" [disabled]="saving()" (click)="save()">{{ saving() ? 'Salvando…' : 'Salvar' }}</button>
          </div>
        </div>
      </div>
    }
  `,
  styleUrl: './admin-shared.scss',
})
export class AdminPlans {
  private billing = inject(BillingService);
  private toast = inject(ToastService);

  protected plans = signal<Plan[]>([]);
  protected loading = signal(true);
  protected form = signal<PlanForm | null>(null);
  protected saving = signal(false);
  protected formatMoney = formatMoney;

  constructor() { this.load(); }

  private load() {
    this.loading.set(true);
    this.billing.listPlans(true).subscribe({
      next: (p) => { this.plans.set(p); this.loading.set(false); },
      error: (e) => { this.loading.set(false); this.toast.error(apiError(e)); },
    });
  }

  protected startNew() {
    this.form.set({ name: '', price: 0, period: BillingPeriod.Monthly, description: '', isActive: true });
  }

  protected edit(p: Plan) {
    this.form.set({ id: p.id, name: p.name, price: p.price, period: p.period, description: p.description ?? '', isActive: p.isActive });
  }

  protected save() {
    const f = this.form();
    if (!f || !f.name) { this.toast.error('Informe o nome do plano.'); return; }
    this.saving.set(true);
    const body = { name: f.name, price: Number(f.price), period: Number(f.period), description: f.description || null };
    const op: Observable<unknown> = f.id
      ? this.billing.updatePlan(f.id, { ...body, isActive: f.isActive })
      : this.billing.createPlan(body);
    op.subscribe({
      next: () => { this.saving.set(false); this.form.set(null); this.toast.success('Plano salvo.'); this.load(); },
      error: (e) => { this.saving.set(false); this.toast.error(apiError(e)); },
    });
  }
}
