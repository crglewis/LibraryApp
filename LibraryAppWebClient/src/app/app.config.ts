import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors, withXhr } from '@angular/common/http';
import { authInterceptor } from './interceptors/auth.interceptor';
import { routes } from './routing';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideAnimationsAsync(),
    provideRouter(routes),
    // zone.js's fetch patch doesn't reliably keep HttpClient's default Fetch backend
    // inside Angular's zone, so responses can resolve without triggering change
    // detection (views silently stay stale until an unrelated CD tick catches up).
    // withXhr() uses XMLHttpRequest instead, which zone.js patches reliably.
    provideHttpClient(withInterceptors([authInterceptor]), withXhr()),
  ],
};
