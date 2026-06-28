import { Routes } from '@angular/router';

export const AUTH_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./auth-layout').then((m) => m.AuthLayout),
    children: [
      { path: 'login', loadComponent: () => import('./login').then((m) => m.Login) },
      { path: 'register', loadComponent: () => import('./register').then((m) => m.Register) },
      { path: 'confirm-email', loadComponent: () => import('./confirm-email').then((m) => m.ConfirmEmail) },
      { path: 'forgot-password', loadComponent: () => import('./forgot-password').then((m) => m.ForgotPassword) },
      { path: 'reset-password', loadComponent: () => import('./reset-password').then((m) => m.ResetPassword) },
      { path: '', redirectTo: 'login', pathMatch: 'full' },
    ],
  },
];
