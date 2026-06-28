import { Component, computed, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { BillingService } from '../../core/api/billing.service';
import { PaymentsService } from '../../core/api/payments.service';
import { AuthService } from '../../core/auth/auth.service';
import { ToastService } from '../../core/notify/toast.service';
import { BillingPeriod, Card, Plan, Subscription } from '../../core/models';
import { apiError, formatDate, formatMoney } from '../../core/util';

@Component({
  selector: 'app-plans',
  imports: [RouterLink],
  template: `
    <header class="hero">
      <span class="chip">Assinaturas</span>
      <h1>Escolha seu plano</h1>
      <p class="muted">Ouça suas bandas favoritas sem limites. Cancele quando quiser.</p>
    </header>

    @if (current(); as sub) {
      <div class="current">
        <span class="material-symbols-rounded">verified</span>
        <div>
          <strong>Plano atual: {{ sub.planName }}</strong>
          <div class="muted">{{ formatMoney(sub.price) }} · renova em {{ formatDate(sub.currentPeriodEnd) }}</div>
        </div>
        <div class="spacer"></div>
        <button class="btn btn--danger btn--sm" (click)="cancel()">Cancelar assinatura</button>
      </div>
    }

    @if (loading()) {
      <p class="muted">Carregando planos…</p>
    } @else {
      <div class="plans">
        @for (p of plans(); track p.id) {
          <div class="plan" [class.plan--featured]="p.price > 0">
            <h3>{{ p.name }}</h3>
            <div class="price">
              <span class="amount">{{ p.price === 0 ? 'Grátis' : formatMoney(p.price) }}</span>
              @if (p.price > 0) { <span class="per">/{{ period(p.period) }}</span> }
            </div>
            @if (p.description) { <p class="desc muted">{{ p.description }}</p> }
            <button class="btn btn--block"
                    [disabled]="isCurrent(p)"
                    (click)="choose(p)">
              {{ isCurrent(p) ? 'Plano atual' : 'Assinar' }}
            </button>
          </div>
        }
      </div>
    }

    <!-- Card picker for paid plans -->
    @if (picking(); as plan) {
      <div class="overlay" (click)="picking.set(null)">
        <div class="dialog" (click)="$event.stopPropagation()">
          <h3>Assinar {{ plan.name }}</h3>
          <p class="muted">{{ formatMoney(plan.price) }} / {{ period(plan.period) }}. Escolha o cartão de cobrança.</p>

          @if (cards().length === 0) {
            <p class="muted">Você não tem cartões cadastrados.</p>
            <a class="btn btn--block" routerLink="/account" (click)="picking.set(null)">Adicionar cartão</a>
          } @else {
            <div class="field">
              <label>Cartão</label>
              <select class="select" [value]="selectedCard()" (change)="selectedCard.set($any($event.target).value)">
                @for (c of cards(); track c.id) {
                  <option [value]="c.id">{{ c.brand }} •••• {{ c.last4 }} — limite {{ formatMoney(c.availableLimit) }}</option>
                }
              </select>
            </div>
            <div class="row gap-8" style="justify-content: flex-end;">
              <button class="btn btn--ghost btn--sm" (click)="picking.set(null)">Cancelar</button>
              <button class="btn btn--sm" [disabled]="submitting()" (click)="confirm(plan)">
                {{ submitting() ? 'Processando…' : 'Confirmar pagamento' }}
              </button>
            </div>
          }
        </div>
      </div>
    }
  `,
  styles: [`
    .hero { padding: 16px 0 26px; }
    .hero h1 { font-size: 34px; margin: 10px 0 6px; }
    .current {
      display: flex; align-items: center; gap: 14px; padding: 16px 20px; margin-bottom: 24px;
      background: rgba(29,185,84,.12); border: 1px solid rgba(29,185,84,.4); border-radius: 12px;
    }
    .current .material-symbols-rounded { color: var(--accent); font-size: 30px; }
    .plans { display: grid; grid-template-columns: repeat(auto-fit, minmax(240px, 1fr)); gap: 18px; }
    .plan { background: var(--bg-card); border: 1px solid var(--border); border-radius: 16px; padding: 26px; display: flex; flex-direction: column; }
    .plan--featured { border-color: rgba(29,185,84,.5); }
    .plan h3 { font-size: 20px; }
    .price { margin: 14px 0; }
    .amount { font-size: 30px; font-weight: 800; }
    .per { color: var(--text-soft); font-weight: 600; }
    .desc { flex: 1; line-height: 1.5; margin: 0 0 18px; }
    .plan .btn { margin-top: auto; }
    .overlay { position: fixed; inset: 0; background: rgba(0,0,0,.6); display: grid; place-items: center; z-index: 1000; }
    .dialog { background: var(--bg-elevated); border-radius: 14px; padding: 26px; width: 100%; max-width: 420px; box-shadow: var(--shadow); }
    .dialog h3 { font-size: 20px; margin-bottom: 6px; }
    .dialog p { margin-bottom: 18px; }
  `],
})
export class Plans {
  private billing = inject(BillingService);
  private payments = inject(PaymentsService);
  private auth = inject(AuthService);
  private toast = inject(ToastService);
  private router = inject(Router);

  protected plans = signal<Plan[]>([]);
  protected loading = signal(true);
  protected current = this.billing.currentSubscription;

  protected cards = signal<Card[]>([]);
  protected picking = signal<Plan | null>(null);
  protected selectedCard = signal<string>('');
  protected submitting = signal(false);

  protected formatMoney = formatMoney;
  protected formatDate = formatDate;

  constructor() {
    this.billing.listPlans().subscribe({
      next: (p) => { this.plans.set(p); this.loading.set(false); },
      error: (e) => { this.loading.set(false); this.toast.error(apiError(e)); },
    });
    if (this.auth.isAuthenticated()) {
      this.billing.mySubscription().subscribe({ error: () => {} });
    }
  }

  protected period(p: BillingPeriod) { return p === BillingPeriod.Yearly ? 'ano' : 'mês'; }
  protected isCurrent = (p: Plan) => computed(() => this.current()?.planId === p.id)();

  protected choose(plan: Plan) {
    if (!this.auth.isAuthenticated()) {
      this.toast.info('Entre na sua conta para assinar.');
      this.router.navigate(['/auth/login'], { queryParams: { returnUrl: '/plans' } });
      return;
    }
    if (plan.price === 0) {
      this.doSubscribe(plan);
      return;
    }
    this.payments.listCards().subscribe({
      next: (cards) => {
        this.cards.set(cards.filter((c) => c.active));
        this.selectedCard.set(this.cards()[0]?.id ?? '');
        this.picking.set(plan);
      },
      error: (e) => this.toast.error(apiError(e)),
    });
  }

  protected confirm(plan: Plan) {
    this.submitting.set(true);
    this.doSubscribe(plan, this.selectedCard() || undefined, () => this.submitting.set(false));
  }

  private doSubscribe(plan: Plan, cardId?: string, done?: () => void) {
    this.billing.subscribe(plan.id, cardId).subscribe({
      next: (sub: Subscription) => {
        done?.();
        this.picking.set(null);
        this.toast.success(`Assinatura ativada: ${sub.planName}!`);
      },
      error: (e) => { done?.(); this.toast.error(apiError(e, 'Não foi possível assinar.')); },
    });
  }

  protected cancel() {
    this.billing.cancel().subscribe({
      next: () => this.toast.success('Assinatura cancelada.'),
      error: (e) => this.toast.error(apiError(e)),
    });
  }
}
