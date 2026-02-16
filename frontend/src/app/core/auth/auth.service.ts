import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { finalize, Observable, tap } from 'rxjs';
import { AuthResponse } from './auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);

  readonly accessToken = signal<string | null>(localStorage.getItem('access_token'));
  readonly userId = signal<string | null>(localStorage.getItem('user_id'));
  readonly role = signal<string | null>(localStorage.getItem('role'));
  readonly isAuthenticated = computed(() => this.accessToken() !== null);

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>('/api/auth/login', { email, password }).pipe(
      tap((response) => this.storeTokens(response)),
    );
  }

  refresh(): Observable<AuthResponse> {
    const refreshToken = localStorage.getItem('refresh_token');
    return this.http.post<AuthResponse>('/api/auth/refresh', { refreshToken }).pipe(
      tap((response) => this.storeTokens(response)),
    );
  }

  logout(): Observable<void> {
    const refreshToken = localStorage.getItem('refresh_token');
    return this.http.post<void>('/api/auth/logout', { refreshToken }).pipe(
      finalize(() => this.clearTokens()),
    );
  }

  clearTokens(): void {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('user_id');
    localStorage.removeItem('role');
    this.accessToken.set(null);
    this.userId.set(null);
    this.role.set(null);
  }

  private storeTokens(response: AuthResponse): void {
    localStorage.setItem('access_token', response.accessToken);
    localStorage.setItem('refresh_token', response.refreshToken);
    localStorage.setItem('user_id', response.userId);
    localStorage.setItem('role', response.role);
    this.accessToken.set(response.accessToken);
    this.userId.set(response.userId);
    this.role.set(response.role);
  }
}
