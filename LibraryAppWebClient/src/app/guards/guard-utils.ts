import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ApiService } from '../services/api.service';
import { User } from '../models';

/**
 * Confirms the session against the server (GET /api/auth/me) rather than trusting
 * the client-cached user, since localStorage can be edited freely in devtools and
 * must never be the thing that decides route access.
 * On failure, clears the cached user, redirects to /login, and resolves to null.
 */
export function confirmSession(apiService: ApiService, router: Router): Observable<User | null> {
  return apiService.fetchCurrentUser().pipe(
    catchError(() => {
      apiService.clearUser();
      router.navigate(['/login']);
      return of(null);
    })
  );
}
