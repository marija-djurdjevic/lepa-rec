import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { App } from './app';
import { describe, it, expect, beforeEach } from 'vitest';

describe('App', () => {
  let component: App;
  let fixture: ComponentFixture<App>;
  let compiled: HTMLElement;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(App);
    component = fixture.componentInstance;
    compiled = fixture.nativeElement as HTMLElement;
    fixture.detectChanges();
  });

  it('should create the app', () => {
    expect(component).toBeTruthy();
  });

  it('should have a title signal', () => {
    expect(component.title).toBeDefined();
    expect(component.title()).toBe('Angular .NET Baseline');
  });

  it('should display the title in the template', () => {
    const h1 = compiled.querySelector('h1');
    expect(h1).toBeTruthy();
    expect(h1?.textContent).toBe('Angular .NET Baseline');
  });

  it('should render the welcome message', () => {
    const welcomeDiv = compiled.querySelector('.welcome');
    expect(welcomeDiv).toBeTruthy();

    const paragraphs = welcomeDiv?.querySelectorAll('p');
    expect(paragraphs?.length).toBe(2);
    expect(paragraphs?.[0].textContent).toContain('Angular v21 baseline application');
    expect(paragraphs?.[1].textContent).toBe('Ready for features');
  });

  it('should have a router outlet', () => {
    const routerOutlet = compiled.querySelector('router-outlet');
    expect(routerOutlet).toBeTruthy();
  });

  it('should render header and main elements', () => {
    const header = compiled.querySelector('header');
    const main = compiled.querySelector('main');

    expect(header).toBeTruthy();
    expect(main).toBeTruthy();
  });
});