import { Routes } from '@angular/router';

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./admin-layout').then((m) => m.AdminLayout),
    children: [
      { path: 'catalog', loadComponent: () => import('./admin-catalog').then((m) => m.AdminCatalog) },
      { path: 'plans', loadComponent: () => import('./admin-plans').then((m) => m.AdminPlans) },
      { path: 'users', loadComponent: () => import('./admin-users').then((m) => m.AdminUsers) },
      { path: '', redirectTo: 'catalog', pathMatch: 'full' },
    ],
  },
];
