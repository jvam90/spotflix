import { Routes } from '@angular/router';
import { authGuard, adminGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then((m) => m.AUTH_ROUTES),
  },
  {
    path: '',
    loadComponent: () => import('./layout/shell/shell').then((m) => m.Shell),
    children: [
      { path: '', loadComponent: () => import('./features/home/home').then((m) => m.Home) },
      { path: 'search', loadComponent: () => import('./features/search/search').then((m) => m.Search) },
      { path: 'bands/:id', loadComponent: () => import('./features/band-detail/band-detail').then((m) => m.BandDetail) },
      { path: 'albums/:id', loadComponent: () => import('./features/album-detail/album-detail').then((m) => m.AlbumDetail) },
      { path: 'plans', loadComponent: () => import('./features/plans/plans').then((m) => m.Plans) },
      {
        path: 'favorites',
        canActivate: [authGuard],
        loadComponent: () => import('./features/favorites/favorites').then((m) => m.Favorites),
      },
      {
        path: 'account',
        canActivate: [authGuard],
        loadComponent: () => import('./features/account/account').then((m) => m.Account),
      },
      {
        path: 'admin',
        canActivate: [adminGuard],
        loadChildren: () => import('./features/admin/admin.routes').then((m) => m.ADMIN_ROUTES),
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
