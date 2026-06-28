// ===== Domain models mirroring the Spotflix.Api DTOs =====

// ---- Auth ----
export interface TokenResponse {
  accessToken: string;
  accessTokenExpiresAt: string;
  refreshToken: string;
  refreshTokenExpiresAt: string;
}

export interface LoginRequest { email: string; password: string; }
export interface RegisterRequest { email: string; password: string; fullName?: string; }

// ---- Users / account ----
export interface User {
  id: string;
  email: string;
  fullName?: string | null;
  emailConfirmed: boolean;
  createdAt: string;
  lockoutEnd?: string | null;
  isLockedOut: boolean;
  roles: string[];
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
}

// ---- Catalog ----
export interface Band {
  id: string;
  name: string;
  genre?: string | null;
  formedYear?: number | null;
  albumCount: number;
}

export interface BandDetail {
  id: string;
  name: string;
  genre?: string | null;
  bio?: string | null;
  formedYear?: number | null;
  albums: Album[];
}

export interface Album {
  id: string;
  title: string;
  releaseYear?: number | null;
  bandId: string;
  songCount: number;
}

export interface AlbumDetail {
  id: string;
  title: string;
  releaseYear?: number | null;
  bandId: string;
  bandName: string;
  songs: Song[];
}

export interface Song {
  id: string;
  title: string;
  durationSeconds: number;
  trackNumber: number;
  albumId: string;
}

export interface SongSearchHit {
  id: string;
  title: string;
  durationSeconds: number;
  albumId: string;
  albumTitle: string;
  bandId: string;
  bandName: string;
}

export interface SearchResult {
  bands: Band[];
  songs: SongSearchHit[];
}

// ---- Billing ----
export enum BillingPeriod { Monthly = 1, Yearly = 2 }
export enum SubscriptionStatus { Active = 1, Canceled = 2, Expired = 3 }

export interface Plan {
  id: string;
  name: string;
  price: number;
  period: BillingPeriod;
  description?: string | null;
  isActive: boolean;
}

export interface Subscription {
  id: string;
  planId: string;
  planName: string;
  price: number;
  status: SubscriptionStatus;
  startedAt: string;
  currentPeriodEnd: string;
  canceledAt?: string | null;
}

// ---- Payments ----
export enum TransactionStatus { Authorized = 1, Declined = 2 }

export interface Card {
  id: string;
  holderName: string;
  last4: string;
  brand: string;
  active: boolean;
  availableLimit: number;
}

export interface AddCardRequest {
  holderName: string;
  number: string;
  brand: string;
  creditLimit: number;
}

export interface Transaction {
  id: string;
  cardId: string;
  merchant: string;
  amount: number;
  occurredAt: string;
  status: TransactionStatus;
  violations: string[];
}

export interface AuthorizeResult {
  authorized: boolean;
  transaction: Transaction;
  availableLimit: number;
}
