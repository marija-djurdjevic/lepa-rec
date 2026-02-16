import { ErrorHandler, Injectable } from '@angular/core';

/**
 * Global error handler that logs all unhandled errors.
 * Extend this to send errors to logging services (Sentry, Application Insights, etc.).
 */
@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  handleError(error: Error): void {
    const timestamp = new Date().toISOString();
    const errorMessage = error.message || 'An unknown error occurred';
    const errorStack = error.stack || 'No stack trace available';

    // Log structured error information
    console.error('Global Error Handler:', {
      timestamp,
      message: errorMessage,
      stack: errorStack,
      error,
    });

    // In production, you might want to:
    // - Send to error tracking service
    // - Show user-friendly notification
    // - Log to analytics
  }
}
