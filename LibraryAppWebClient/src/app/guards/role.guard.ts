import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { map } from 'rxjs/operators';
import { ApiService } from '../services/api.service';
import { confirmSession } from './guard-utils';

/**
 * Route guard for pages restricted to a single role, e.g. `data: { role: 'Librarian' }`.
 * Re-confirms the user's role against the server rather than trusting the
 * client-cached copy, since localStorage can be edited freely in devtools.
 */
export const roleGuard: CanActivateFn = (route) => {
  const apiService = inject(ApiService);
  const router = inject(Router);
  const requiredRole = route.data['role'];

  return confirmSession(apiService, router).pipe(
    map(user => {
      if (!user) return false; // confirmSession already redirected to /login
      if (requiredRole && user.role !== requiredRole) {
        router.navigate(['/home']);
        return false;
      }
      return true;
    })
  );
};
