import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResult, User } from '../models';

@Injectable({ providedIn: 'root' })
export class UsersService {
  private http = inject(HttpClient);

  list(page = 1, pageSize = 20, search?: string): Observable<PagedResult<User>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (search?.trim()) params = params.set('search', search.trim());
    return this.http.get<PagedResult<User>>('/api/users', { params });
  }

  setRoles(id: string, roles: string[]): Observable<void> {
    return this.http.put<void>(`/api/users/${id}/roles`, { roles });
  }

  lock(id: string): Observable<void> {
    return this.http.post<void>(`/api/users/${id}/lock`, {});
  }

  unlock(id: string): Observable<void> {
    return this.http.post<void>(`/api/users/${id}/unlock`, {});
  }

  remove(id: string): Observable<void> {
    return this.http.delete<void>(`/api/users/${id}`);
  }
}
