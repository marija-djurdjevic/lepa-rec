import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideZonelessChangeDetection, ErrorHandler } from '@angular/core';
import { App } from './app/app';
import { routes } from './app/app.routes';
import { GlobalErrorHandler } from './app/core/error-handling/global-error.handler';

bootstrapApplication(App, {
  providers: [
    provideZonelessChangeDetection(),
    provideRouter(routes),
    provideHttpClient(),
    { provide: ErrorHandler, useClass: GlobalErrorHandler },
  ],
}).catch((err) => console.error(err));
