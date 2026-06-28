import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, catchError, of } from 'rxjs';
import { LoginRequest, RegisterRequest, TokenResponse, User } from '../models';

const ACCESS_KEY = 'sfx.access';
const REFRESH_KEY = 'sfx.refresh';

interface JwtPayload {
  sub?: string;
  email?: string;
  exp?: number;
  [claim: string]: unknown;
}

const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);

  private readonly _accessToken = signal<string | null>(this.getStoredToken());
  readonly currentUser = signal<User | null>(null);

  readonly isAuthenticated = computed(() => !!this._accessToken());
  readonly roles = computed<string[]>(() => {
    const payload = this.decode(this._accessToken());
    if (!payload) return [];
    const raw = payload[ROLE_CLAIM] ?? payload['role'] ?? payload['roles'];
    if (!raw) return [];
    return Array.isArray(raw) ? (raw as string[]) : [String(raw)];
  });
  readonly isAdmin = computed(() => this.roles().includes('Admin'));
  readonly email = computed(() => this.decode(this._accessToken())?.email ?? null);

  private getStoredToken(): string | null {
    try {
      return localStorage.getItem(ACCESS_KEY);
    } catch {
      console.warn('Unable to read token from localStorage');
      return null;
    }
  }

  get accessToken(): string | null { return this._accessToken(); }
  get refreshTokenValue(): string | null { return localStorage.getItem(REFRESH_KEY); }

  login(body: LoginRequest): Observable<TokenResponse> {
    console.info('[AuthService] Login attempt for', body.email);
    return this.http.post<TokenResponse>('/api/auth/login', body).pipe(
      tap((tokens) => {
        console.info('[AuthService] Login successful');
        this.storeTokens(tokens);
      }),
      catchError((err) => {
        console.warn('[AuthService] Login failed', err.status);
        throw err;
      }),
    );
  }

  register(body: RegisterRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>('/api/auth/register', body);
  }

  confirmEmail(userId: string, token: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>('/api/auth/confirm-email', { userId, token });
  }

  forgotPassword(email: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>('/api/auth/forgot-password', { email });
  }

  resetPassword(email: string, token: string, newPassword: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>('/api/auth/reset-password', { email, token, newPassword });
  }

  refresh(): Observable<TokenResponse> {
    return this.http.post<TokenResponse>('/api/auth/refresh', { refreshToken: this.refreshTokenValue }).pipe(
      tap((tokens) => this.storeTokens(tokens)),
    );
  }

  loadProfile(): Observable<User> {
    return this.http.get<User>('/api/account/me').pipe(
      tap((user) => this.currentUser.set(user)),
    );
  }

  logout(): void {
    const refreshToken = this.refreshTokenValue;
    if (refreshToken) {
      this.http.post('/api/auth/logout', { refreshToken }).subscribe({
        next: () => console.info('[AuthService] Logout successful'),
        error: (err) => console.warn('[AuthService] Logout notification failed', err.status),
      });
    }
    console.info('[AuthService] Clearing local session');
    this.clear();
  }

  storeTokens(tokens: TokenResponse): void {
    try {
      localStorage.setItem(ACCESS_KEY, tokens.accessToken);
      localStorage.setItem(REFRESH_KEY, tokens.refreshToken);
      this._accessToken.set(tokens.accessToken);
    } catch (err) {
      console.error('[AuthService] Failed to store tokens', err);
      throw err;
    }
  }

  clear(): void {
    localStorage.removeItem(ACCESS_KEY);
    localStorage.removeItem(REFRESH_KEY);
    this._accessToken.set(null);
    this.currentUser.set(null);
  }

  private decode(token: string | null): JwtPayload | null {
    if (!token) return null;
    try {
      const part = token.split('.')[1];
      if (!part) return null;
      const json = atob(part.replace(/-/g, '+').replace(/_/g, '/'));
      return JSON.parse(decodeURIComponent(escape(json))) as JwtPayload;
    } catch (err) {
      console.warn('[AuthService] Invalid JWT token', err);
      return null;
    }
  }
}
