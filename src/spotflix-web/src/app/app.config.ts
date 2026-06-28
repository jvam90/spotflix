import {
  ApplicationConfig,
  ErrorHandler,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection,
  inject,
  provideAppInitializer,
} from '@angular/core';
import { provideRouter, withInMemoryScrolling } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { tap, catchError, of } from 'rxjs';

import { routes } from './app.routes';
import { authInterceptor } from './core/auth/auth.interceptor';
import { AuthService } from './core/auth/auth.service';
import { GlobalErrorHandler } from './core/error.handler';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(routes, withInMemoryScrolling({ scrollPositionRestoration: 'top' })),
    provideHttpClient(withInterceptors([authInterceptor])),
    { provide: ErrorHandler, useClass: GlobalErrorHandler },
    // Hydrate the current user on startup when a token is already present.
    provideAppInitializer(() => {
      const auth = inject(AuthService);
      if (auth.isAuthenticated()) {
        return auth.loadProfile().pipe(
          tap((user) => console.info('[AppInitializer] User profile loaded:', user.email)),
          catchError((err) => {
            console.warn('[AppInitializer] Failed to load user profile, clearing session', err.status);
            auth.clear();
            return of(null);
          }),
        );
      }
      return undefined;
    }),
  ],
};
