import { GlobalErrorHandler } from './global-error.handler';
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';

describe('GlobalErrorHandler', () => {
  let errorHandler: GlobalErrorHandler;
  let consoleErrorSpy: ReturnType<typeof vi.spyOn>;

  beforeEach(() => {
    errorHandler = new GlobalErrorHandler();
    consoleErrorSpy = vi.spyOn(console, 'error').mockImplementation(() => {});
  });

  afterEach(() => {
    consoleErrorSpy.mockRestore();
  });

  it('should log error with structured format', () => {
    const testError = new Error('Test error message');

    errorHandler.handleError(testError);

    expect(consoleErrorSpy).toHaveBeenCalledTimes(1);
    expect(consoleErrorSpy).toHaveBeenCalledWith(
      'Global Error Handler:',
      expect.objectContaining({
        timestamp: expect.any(String),
        message: 'Test error message',
        stack: expect.any(String),
        error: testError,
      })
    );
  });
});
