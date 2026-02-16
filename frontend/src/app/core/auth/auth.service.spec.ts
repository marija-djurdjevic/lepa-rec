import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { AuthService } from './auth.service';
import { AuthResponse } from './auth.models';

describe('AuthService', () => {
  let service: AuthService;
  let httpTesting: HttpTestingController;

  const mockResponse: AuthResponse = {
    accessToken: 'test-access-token',
    refreshToken: 'test-refresh-token',
    expiresAt: '2026-01-01T00:00:00Z',
    userId: 'test-user-id',
    role: 'Administrator',
  };

  beforeEach(() => {
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(AuthService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
    localStorage.clear();
  });

  it('should initialize with no token', () => {
    expect(service.accessToken()).toBeNull();
    expect(service.userId()).toBeNull();
    expect(service.role()).toBeNull();
    expect(service.isAuthenticated()).toBe(false);
  });

  it('should restore session from localStorage on init', () => {
    localStorage.setItem('access_token', 'stored-token');
    localStorage.setItem('refresh_token', 'stored-refresh');
    localStorage.setItem('user_id', 'stored-user-id');
    localStorage.setItem('role', 'Administrator');

    TestBed.resetTestingModule();
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    const freshService = TestBed.inject(AuthService);
    httpTesting = TestBed.inject(HttpTestingController);

    expect(freshService.accessToken()).toBe('stored-token');
    expect(freshService.userId()).toBe('stored-user-id');
    expect(freshService.role()).toBe('Administrator');
    expect(freshService.isAuthenticated()).toBe(true);
  });

  it('should login and store tokens', () => {
    service.login('test@example.com', 'password').subscribe((response) => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpTesting.expectOne('/api/auth/login');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ email: 'test@example.com', password: 'password' });
    req.flush(mockResponse);

    expect(service.accessToken()).toBe('test-access-token');
    expect(service.userId()).toBe('test-user-id');
    expect(service.role()).toBe('Administrator');
    expect(service.isAuthenticated()).toBe(true);
    expect(localStorage.getItem('access_token')).toBe('test-access-token');
    expect(localStorage.getItem('refresh_token')).toBe('test-refresh-token');
    expect(localStorage.getItem('user_id')).toBe('test-user-id');
    expect(localStorage.getItem('role')).toBe('Administrator');
  });

  it('should refresh and update tokens', () => {
    localStorage.setItem('refresh_token', 'old-refresh-token');

    service.refresh().subscribe((response) => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpTesting.expectOne('/api/auth/refresh');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ refreshToken: 'old-refresh-token' });
    req.flush(mockResponse);

    expect(service.accessToken()).toBe('test-access-token');
    expect(service.userId()).toBe('test-user-id');
    expect(service.role()).toBe('Administrator');
    expect(localStorage.getItem('refresh_token')).toBe('test-refresh-token');
    expect(localStorage.getItem('user_id')).toBe('test-user-id');
    expect(localStorage.getItem('role')).toBe('Administrator');
  });

  it('should send logout request and clear tokens on success', () => {
    localStorage.setItem('access_token', 'some-token');
    localStorage.setItem('refresh_token', 'some-refresh');
    service.accessToken.set('some-token');

    service.logout().subscribe();

    const req = httpTesting.expectOne('/api/auth/logout');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ refreshToken: 'some-refresh' });
    req.flush(null, { status: 204, statusText: 'No Content' });

    expect(service.accessToken()).toBeNull();
    expect(service.userId()).toBeNull();
    expect(service.role()).toBeNull();
    expect(service.isAuthenticated()).toBe(false);
    expect(localStorage.getItem('access_token')).toBeNull();
    expect(localStorage.getItem('refresh_token')).toBeNull();
    expect(localStorage.getItem('user_id')).toBeNull();
    expect(localStorage.getItem('role')).toBeNull();
  });

  it('should clear tokens even if logout request fails', () => {
    localStorage.setItem('access_token', 'some-token');
    localStorage.setItem('refresh_token', 'some-refresh');
    service.accessToken.set('some-token');

    service.logout().subscribe({ error: () => {} });

    const req = httpTesting.expectOne('/api/auth/logout');
    req.flush('Server Error', { status: 500, statusText: 'Internal Server Error' });

    expect(service.accessToken()).toBeNull();
    expect(service.userId()).toBeNull();
    expect(service.role()).toBeNull();
    expect(service.isAuthenticated()).toBe(false);
    expect(localStorage.getItem('access_token')).toBeNull();
    expect(localStorage.getItem('refresh_token')).toBeNull();
    expect(localStorage.getItem('user_id')).toBeNull();
    expect(localStorage.getItem('role')).toBeNull();
  });

  it('should clear tokens via clearTokens', () => {
    localStorage.setItem('access_token', 'some-token');
    localStorage.setItem('refresh_token', 'some-refresh');
    service.accessToken.set('some-token');

    service.clearTokens();

    expect(service.accessToken()).toBeNull();
    expect(service.userId()).toBeNull();
    expect(service.role()).toBeNull();
    expect(service.isAuthenticated()).toBe(false);
    expect(localStorage.getItem('access_token')).toBeNull();
    expect(localStorage.getItem('refresh_token')).toBeNull();
    expect(localStorage.getItem('user_id')).toBeNull();
    expect(localStorage.getItem('role')).toBeNull();
  });
});
