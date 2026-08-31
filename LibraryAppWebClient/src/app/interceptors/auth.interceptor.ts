import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { ApiService } from '../services/api.service';

/**
 * The backend uses ASP.NET Identity's cookie auth (no bearer token), so this
 * interceptor's job is just to make sure the session cookie is sent, and to
 * drop stale client-side auth state when the server says the session is gone.
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const apiService = inject(ApiService);
  const router = inject(Router);

  return next(req.clone({ withCredentials: true })).pipe(
    catchError(err => {
      if (err.status === 401 && apiService.isAuthenticated()) {
        apiService.clearUser();
        router.navigate(['/login']);
      }
      return throwError(() => err);
    })
  );
};
