import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AddCardRequest, AuthorizeResult, Card, Transaction } from '../models';

@Injectable({ providedIn: 'root' })
export class PaymentsService {
  private http = inject(HttpClient);

  listCards(): Observable<Card[]> {
    return this.http.get<Card[]>('/api/cards');
  }

  addCard(body: AddCardRequest): Observable<Card> {
    return this.http.post<Card>('/api/cards', body);
  }

  authorize(cardId: string, merchant: string, amount: number): Observable<AuthorizeResult> {
    return this.http.post<AuthorizeResult>('/api/transactions/authorize', { cardId, merchant, amount });
  }

  history(cardId: string): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(`/api/cards/${cardId}/transactions`);
  }
}
