import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { App } from './app';
import { AuthService } from './core/auth/auth.service';
import { describe, it, expect, beforeEach } from 'vitest';

describe('App', () => {
  let component: App;
  let fixture: ComponentFixture<App>;
  let compiled: HTMLElement;
  let authService: AuthService;

  beforeEach(async () => {
    localStorage.clear();

    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(App);
    component = fixture.componentInstance;
    compiled = fixture.nativeElement as HTMLElement;
    authService = TestBed.inject(AuthService);
    fixture.detectChanges();
  });

  it('should create the app', () => {
    expect(component).toBeTruthy();
  });

  it('should have a title signal', () => {
    expect(component.title()).toBe('Angular .NET Baseline');
  });

  it('should hide header when not authenticated', () => {
    authService.accessToken.set(null);
    fixture.detectChanges();

    const header = compiled.querySelector('header');
    expect(header).toBeFalsy();
  });

  it('should show header when authenticated', async () => {
    authService.accessToken.set('some-token');
    fixture.detectChanges();
    await fixture.whenStable();

    const header = compiled.querySelector('header');
    expect(header).toBeTruthy();
    expect(header?.querySelector('h1')?.textContent).toBe('Angular .NET Baseline');
  });

  it('should have a router outlet', () => {
    const routerOutlet = compiled.querySelector('router-outlet');
    expect(routerOutlet).toBeTruthy();
  });
});
