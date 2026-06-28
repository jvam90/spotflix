import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Plan, Subscription } from '../models';

@Injectable({ providedIn: 'root' })
export class BillingService {
  private http = inject(HttpClient);

  readonly currentSubscription = signal<Subscription | null>(null);

  listPlans(includeInactive = false): Observable<Plan[]> {
    const params = new HttpParams().set('includeInactive', includeInactive);
    return this.http.get<Plan[]>('/api/plans', { params });
  }

  createPlan(body: { name: string; price: number; period: number; description?: string | null }): Observable<Plan> {
    return this.http.post<Plan>('/api/plans', body);
  }

  updatePlan(id: string, body: { name: string; price: number; period: number; description?: string | null; isActive: boolean }): Observable<void> {
    return this.http.put<void>(`/api/plans/${id}`, body);
  }

  mySubscription(): Observable<Subscription> {
    return this.http.get<Subscription>('/api/subscriptions/me').pipe(
      tap((sub) => this.currentSubscription.set(sub)),
    );
  }

  subscribe(planId: string, cardId?: string): Observable<Subscription> {
    return this.http.post<Subscription>('/api/subscriptions', { planId, cardId }).pipe(
      tap((sub) => this.currentSubscription.set(sub)),
    );
  }

  cancel(): Observable<void> {
    return this.http.post<void>('/api/subscriptions/cancel', {}).pipe(
      tap(() => this.currentSubscription.set(null)),
    );
  }
}
