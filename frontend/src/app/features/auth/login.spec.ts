import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter, Router } from '@angular/router';
import { describe, it, expect, beforeEach, afterEach } from 'vitest';
import { Login } from './login';
import { AuthResponse } from '../../core/auth/auth.models';

describe('Login', () => {
  let component: Login;
  let fixture: ComponentFixture<Login>;
  let compiled: HTMLElement;
  let httpTesting: HttpTestingController;
  let router: Router;

  beforeEach(async () => {
    localStorage.clear();

    await TestBed.configureTestingModule({
      imports: [Login],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(Login);
    component = fixture.componentInstance;
    compiled = fixture.nativeElement as HTMLElement;
    httpTesting = TestBed.inject(HttpTestingController);
    router = TestBed.inject(Router);
    fixture.detectChanges();
  });

  afterEach(() => {
    httpTesting.verify();
    localStorage.clear();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should have email and password form controls', () => {
    expect(component.form.get('email')).toBeTruthy();
    expect(component.form.get('password')).toBeTruthy();
  });

  it('should have form invalid when empty', () => {
    expect(component.form.valid).toBe(false);
  });

  it('should disable submit button when form is invalid', () => {
    const button = compiled.querySelector('button[type="submit"]') as HTMLButtonElement;
    expect(button.disabled).toBe(true);
  });

  it('should enable submit button when form is valid', async () => {
    component.form.setValue({ email: 'test@example.com', password: 'password' });
    fixture.detectChanges();
    await fixture.whenStable();

    const button = compiled.querySelector('button[type="submit"]') as HTMLButtonElement;
    expect(button.disabled).toBe(false);
  });

  it('should call login on valid form submit', () => {
    const mockResponse: AuthResponse = {
      accessToken: 'token',
      refreshToken: 'refresh',
      expiresAt: '2026-01-01T00:00:00Z',
    };

    component.form.setValue({ email: 'test@example.com', password: 'password' });
    component.onSubmit();

    const req = httpTesting.expectOne('/api/auth/login');
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  it('should display error on failed login', async () => {
    component.form.setValue({ email: 'test@example.com', password: 'wrong' });
    component.onSubmit();

    const req = httpTesting.expectOne('/api/auth/login');
    req.flush('Unauthorized', { status: 401, statusText: 'Unauthorized' });

    fixture.detectChanges();
    await fixture.whenStable();

    const errorEl = compiled.querySelector('.error');
    expect(errorEl).toBeTruthy();
    expect(errorEl?.textContent).toContain('Invalid email or password');
  });

  it('should not submit when form is invalid', () => {
    component.onSubmit();
    httpTesting.expectNone('/api/auth/login');
  });
});
