import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { Home } from './home';
import { AuthService } from '../../core/auth/auth.service';

describe('Home', () => {
  let fixture: ComponentFixture<Home>;
  let compiled: HTMLElement;
  let authService: AuthService;
  let router: Router;

  beforeEach(async () => {
    localStorage.clear();

    await TestBed.configureTestingModule({
      imports: [Home],
      providers: [provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(Home);
    compiled = fixture.nativeElement as HTMLElement;
    authService = TestBed.inject(AuthService);
    router = TestBed.inject(Router);
    fixture.detectChanges();
  });

  it('should display welcome message', () => {
    const message = compiled.querySelector('p');
    expect(message?.textContent).toContain('Welcome! You are logged in.');
  });

  it('should call logout and navigate on sign out click', () => {
    const logoutSpy = vi.spyOn(authService, 'logout');
    const navigateSpy = vi.spyOn(router, 'navigate');

    const button = compiled.querySelector('button') as HTMLButtonElement;
    button.click();

    expect(logoutSpy).toHaveBeenCalled();
    expect(navigateSpy).toHaveBeenCalledWith(['/login']);
  });
});
