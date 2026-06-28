import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AccountService } from '../../core/api/account.service';
import { BillingService } from '../../core/api/billing.service';
import { PaymentsService } from '../../core/api/payments.service';
import { AuthService } from '../../core/auth/auth.service';
import { ToastService } from '../../core/notify/toast.service';
import { Card, Subscription, Transaction, TransactionStatus } from '../../core/models';
import { apiError, formatDate, formatDuration, formatMoney } from '../../core/util';

@Component({
  selector: 'app-account',
  imports: [FormsModule, RouterLink],
  template: `
    <h1 class="page-title">Minha conta</h1>

    <div class="cols">
      <div class="col">
        <!-- Profile -->
        <section class="card">
          <h2>Perfil</h2>
          <div class="field">
            <label>E-mail</label>
            <input class="input" [value]="auth.email()" disabled />
          </div>
          <div class="field">
            <label>Nome completo</label>
            <input class="input" name="fullName" [(ngModel)]="fullName" placeholder="Seu nome" />
          </div>
          <div class="roles">
            @for (r of auth.roles(); track r) { <span class="chip">{{ r }}</span> }
          </div>
          <button class="btn" [disabled]="savingProfile()" (click)="saveProfile()">
            {{ savingProfile() ? 'Salvando…' : 'Salvar perfil' }}
          </button>
        </section>

        <!-- Password -->
        <section class="card">
          <h2>Alterar senha</h2>
          <div class="field">
            <label>Senha atual</label>
            <input class="input" type="password" name="cur" [(ngModel)]="currentPassword" autocomplete="current-password" />
          </div>
          <div class="field">
            <label>Nova senha</label>
            <input class="input" type="password" name="new" [(ngModel)]="newPassword" autocomplete="new-password" />
          </div>
          <button class="btn btn--ghost" [disabled]="savingPw()" (click)="changePassword()">
            {{ savingPw() ? 'Alterando…' : 'Alterar senha' }}
          </button>
        </section>

        <!-- Subscription -->
        <section class="card">
          <h2>Assinatura</h2>
          @if (subscription(); as sub) {
            <p><strong>{{ sub.planName }}</strong> — {{ formatMoney(sub.price) }}</p>
            <p class="muted">Início {{ formatDate(sub.startedAt) }} · renova em {{ formatDate(sub.currentPeriodEnd) }}</p>
            <button class="btn btn--danger btn--sm" (click)="cancelSub()">Cancelar</button>
          } @else {
            <p class="muted">Você não tem uma assinatura ativa.</p>
            <a class="btn btn--sm" routerLink="/plans">Ver planos</a>
          }
        </section>
      </div>

      <div class="col">
        <!-- Cards -->
        <section class="card">
          <h2>Meus cartões</h2>
          @if (cards().length === 0) {
            <p class="muted">Nenhum cartão cadastrado.</p>
          } @else {
            <div class="cards">
              @for (c of cards(); track c.id) {
                <button class="creditcard" [class.sel]="selectedCard() === c.id" (click)="selectCard(c)">
                  <div class="cc-brand">{{ c.brand }}</div>
                  <div class="cc-num">•••• •••• •••• {{ c.last4 }}</div>
                  <div class="cc-foot">
                    <span>{{ c.holderName }}</span>
                    <span>{{ formatMoney(c.availableLimit) }}</span>
                  </div>
                </button>
              }
            </div>
          }

          <details class="addcard">
            <summary>Adicionar cartão</summary>
            <div class="field"><label>Nome no cartão</label><input class="input" name="ch" [(ngModel)]="newCard.holderName" /></div>
            <div class="field"><label>Número</label><input class="input" name="cn" [(ngModel)]="newCard.number" placeholder="4111 1111 1111 1111" /></div>
            <div class="row gap-12">
              <div class="field" style="flex:1"><label>Bandeira</label><input class="input" name="cb" [(ngModel)]="newCard.brand" placeholder="Visa" /></div>
              <div class="field" style="flex:1"><label>Limite</label><input class="input" type="number" name="cl" [(ngModel)]="newCard.creditLimit" /></div>
            </div>
            <button class="btn btn--sm" [disabled]="addingCard()" (click)="addCard()">
              {{ addingCard() ? 'Adicionando…' : 'Adicionar cartão' }}
            </button>
          </details>
        </section>

        <!-- Transactions of selected card -->
        @if (selectedCard()) {
          <section class="card">
            <h2>Transações do cartão</h2>

            <div class="authbox">
              <input class="input" name="m" [(ngModel)]="auth_merchant" placeholder="Comerciante" />
              <input class="input" type="number" name="a" [(ngModel)]="auth_amount" placeholder="Valor" />
              <button class="btn btn--sm" [disabled]="authorizing()" (click)="authorize()">Autorizar</button>
            </div>

            @if (txLoading()) {
              <p class="muted">Carregando…</p>
            } @else if (transactions().length === 0) {
              <p class="muted">Nenhuma transação neste cartão.</p>
            } @else {
              <table class="tx">
                <thead><tr><th>Comerciante</th><th>Valor</th><th>Quando</th><th>Status</th></tr></thead>
                <tbody>
                  @for (t of transactions(); track t.id) {
                    <tr>
                      <td>{{ t.merchant }}</td>
                      <td>{{ formatMoney(t.amount) }}</td>
                      <td class="muted">{{ formatDate(t.occurredAt) }}</td>
                      <td>
                        <span class="badge" [class.ok]="t.status === 1" [class.no]="t.status === 2"
                              [title]="t.violations.join(', ')">
                          {{ t.status === 1 ? 'Autorizada' : 'Recusada' }}
                        </span>
                      </td>
                    </tr>
                  }
                </tbody>
              </table>
            }
          </section>
        }
      </div>
    </div>
  `,
  styles: [`
    .page-title { font-size: 30px; margin: 14px 0 22px; }
    .cols { display: grid; grid-template-columns: 1fr 1fr; gap: 18px; align-items: start; }
    .col { display: flex; flex-direction: column; gap: 18px; }
    .card h2 { font-size: 18px; margin-bottom: 16px; }
    .roles { display: flex; gap: 8px; flex-wrap: wrap; margin-bottom: 16px; }
    .cards { display: grid; gap: 12px; margin-bottom: 16px; }
    .creditcard {
      text-align: left; border: 1px solid var(--border); border-radius: 12px; padding: 16px; cursor: pointer;
      background: linear-gradient(135deg, #2a2a3a, #1a1a24); color: #fff; font: inherit;
    }
    .creditcard.sel { border-color: var(--accent); box-shadow: 0 0 0 1px var(--accent); }
    .cc-brand { font-weight: 700; text-transform: uppercase; font-size: 13px; color: var(--text-soft); }
    .cc-num { font-size: 18px; letter-spacing: 2px; margin: 12px 0; font-variant-numeric: tabular-nums; }
    .cc-foot { display: flex; justify-content: space-between; font-size: 13px; color: var(--text-soft); }
    .addcard { border-top: 1px solid var(--border); padding-top: 14px; }
    .addcard summary { cursor: pointer; font-weight: 600; color: var(--accent); margin-bottom: 12px; }
    .authbox { display: flex; gap: 8px; margin-bottom: 16px; }
    .authbox .input { flex: 1; }
    .tx { width: 100%; border-collapse: collapse; font-size: 14px; }
    .tx th { text-align: left; color: var(--text-soft); font-weight: 600; font-size: 12px; text-transform: uppercase; padding: 8px; border-bottom: 1px solid var(--border); }
    .tx td { padding: 10px 8px; border-bottom: 1px solid #20202a; }
    .badge { padding: 3px 10px; border-radius: 999px; font-size: 12px; font-weight: 600; }
    .badge.ok { background: rgba(29,185,84,.15); color: var(--accent); }
    .badge.no { background: rgba(240,80,110,.15); color: var(--danger); }
    @media (max-width: 900px) { .cols { grid-template-columns: 1fr; } }
  `],
})
export class Account {
  private accountApi = inject(AccountService);
  private billing = inject(BillingService);
  private payments = inject(PaymentsService);
  protected auth = inject(AuthService);
  private toast = inject(ToastService);

  protected formatMoney = formatMoney;
  protected formatDate = formatDate;
  protected formatDuration = formatDuration;

  protected fullName = '';
  protected currentPassword = '';
  protected newPassword = '';
  protected savingProfile = signal(false);
  protected savingPw = signal(false);

  protected subscription = signal<Subscription | null>(null);

  protected cards = signal<Card[]>([]);
  protected addingCard = signal(false);
  protected newCard = { holderName: '', number: '', brand: 'Visa', creditLimit: 5000 };

  protected selectedCard = signal<string>('');
  protected transactions = signal<Transaction[]>([]);
  protected txLoading = signal(false);
  protected authorizing = signal(false);
  protected auth_merchant = '';
  protected auth_amount: number | null = null;

  constructor() {
    this.accountApi.me().subscribe({
      next: (u) => { this.auth.currentUser.set(u); this.fullName = u.fullName ?? ''; },
      error: (e) => this.toast.error(apiError(e)),
    });
    this.billing.mySubscription().subscribe({
      next: (s) => this.subscription.set(s),
      error: () => this.subscription.set(null),
    });
    this.loadCards();
  }

  private loadCards() {
    this.payments.listCards().subscribe({
      next: (c) => this.cards.set(c),
      error: (e) => this.toast.error(apiError(e)),
    });
  }

  protected saveProfile() {
    this.savingProfile.set(true);
    this.accountApi.updateProfile(this.fullName || null).subscribe({
      next: () => {
        this.savingProfile.set(false);
        const u = this.auth.currentUser();
        if (u) this.auth.currentUser.set({ ...u, fullName: this.fullName });
        this.toast.success('Perfil atualizado.');
      },
      error: (e) => { this.savingProfile.set(false); this.toast.error(apiError(e)); },
    });
  }

  protected changePassword() {
    if (!this.currentPassword || !this.newPassword) { this.toast.error('Preencha as senhas.'); return; }
    this.savingPw.set(true);
    this.accountApi.changePassword(this.currentPassword, this.newPassword).subscribe({
      next: (r) => {
        this.savingPw.set(false);
        this.currentPassword = ''; this.newPassword = '';
        this.toast.success(r.message || 'Senha alterada.');
      },
      error: (e) => { this.savingPw.set(false); this.toast.error(apiError(e)); },
    });
  }

  protected cancelSub() {
    this.billing.cancel().subscribe({
      next: () => { this.subscription.set(null); this.toast.success('Assinatura cancelada.'); },
      error: (e) => this.toast.error(apiError(e)),
    });
  }

  protected addCard() {
    if (!this.newCard.holderName || !this.newCard.number) { this.toast.error('Informe nome e número do cartão.'); return; }
    this.addingCard.set(true);
    this.payments.addCard({ ...this.newCard }).subscribe({
      next: (c) => {
        this.addingCard.set(false);
        this.cards.update((list) => [...list, c]);
        this.newCard = { holderName: '', number: '', brand: 'Visa', creditLimit: 5000 };
        this.toast.success('Cartão adicionado.');
      },
      error: (e) => { this.addingCard.set(false); this.toast.error(apiError(e)); },
    });
  }

  protected selectCard(c: Card) {
    this.selectedCard.set(c.id);
    this.loadTransactions(c.id);
  }

  private loadTransactions(cardId: string) {
    this.txLoading.set(true);
    this.payments.history(cardId).subscribe({
      next: (t) => { this.transactions.set(t); this.txLoading.set(false); },
      error: (e) => { this.txLoading.set(false); this.toast.error(apiError(e)); },
    });
  }

  protected authorize() {
    const cardId = this.selectedCard();
    if (!cardId || !this.auth_merchant || !this.auth_amount) { this.toast.error('Preencha comerciante e valor.'); return; }
    this.authorizing.set(true);
    this.payments.authorize(cardId, this.auth_merchant, this.auth_amount).subscribe({
      next: (r) => {
        this.authorizing.set(false);
        this.auth_merchant = ''; this.auth_amount = null;
        if (r.authorized) this.toast.success('Transação autorizada!');
        else this.toast.error('Transação recusada: ' + r.transaction.violations.join(', '));
        this.loadTransactions(cardId);
        this.loadCards();
      },
      error: (e) => {
        this.authorizing.set(false);
        // 402 carries the declined transaction body.
        this.toast.error(apiError(e, 'Transação recusada.'));
        this.loadTransactions(cardId);
      },
    });
  }

  protected readonly TransactionStatus = TransactionStatus;
}
