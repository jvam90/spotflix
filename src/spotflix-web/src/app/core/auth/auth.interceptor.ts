import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, catchError, filter, switchMap, take, throwError } from 'rxjs';
import { AuthService } from './auth.service';

// Shared refresh state so concurrent 401s trigger a single refresh.
let refreshing = false;
const refreshed$ = new BehaviorSubject<string | null>(null);

const PUBLIC = ['/api/auth/login', '/api/auth/register', '/api/auth/refresh', '/api/auth/forgot-password', '/api/auth/reset-password', '/api/auth/confirm-email'];

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const isApi = req.url.startsWith('/api');
  const isPublic = PUBLIC.some((p) => req.url.startsWith(p));
  const token = auth.accessToken;

  const authReq = isApi && token && !isPublic
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status !== 401 || isPublic || !auth.refreshTokenValue) {
        return throwError(() => err);
      }

      if (refreshing) {
        console.debug('[AuthInterceptor] Refresh in progress, queuing request');
        return refreshed$.pipe(
          filter((t) => t !== null),
          take(1),
          switchMap((t) => next(req.clone({ setHeaders: { Authorization: `Bearer ${t}` } }))),
        );
      }

      refreshing = true;
      refreshed$.next(null);
      console.info('[AuthInterceptor] Attempting token refresh');

      return auth.refresh().pipe(
        switchMap((tokens) => {
          console.info('[AuthInterceptor] Token refresh successful');
          refreshing = false;
          refreshed$.next(tokens.accessToken);
          return next(req.clone({ setHeaders: { Authorization: `Bearer ${tokens.accessToken}` } }));
        }),
        catchError((refreshErr) => {
          console.warn('[AuthInterceptor] Token refresh failed, clearing session', refreshErr.status);
          refreshing = false;
          auth.clear();
          router.navigate(['/auth/login']);
          return throwError(() => refreshErr);
        }),
      );
    }),
  );
};
