import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { ApiService } from '../services/api.service';

/** Route guard for pages restricted to a single role, e.g. `data: { role: 'Librarian' }`. */
export const roleGuard: CanActivateFn = (route) => {
  const apiService = inject(ApiService);
  const router = inject(Router);
  const requiredRole = route.data['role'];

  const user = apiService.getUser();
  if (!user) {
    router.navigate(['/login']);
    return false;
  }
  if (requiredRole && user.role !== requiredRole) {
    router.navigate(['/home']);
    return false;
  }
  return true;
};
