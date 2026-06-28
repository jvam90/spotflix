import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models';

@Injectable({ providedIn: 'root' })
export class AccountService {
  private http = inject(HttpClient);

  me(): Observable<User> {
    return this.http.get<User>('/api/account/me');
  }

  updateProfile(fullName: string | null): Observable<void> {
    return this.http.put<void>('/api/account/me', { fullName });
  }

  changePassword(currentPassword: string, newPassword: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>('/api/account/change-password', { currentPassword, newPassword });
  }
}
