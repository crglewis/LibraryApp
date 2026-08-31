import { Injectable, inject } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from '../services/api.service';
import { confirmSession } from './guard-utils';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  private apiService = inject(ApiService);
  private router = inject(Router);

  canActivate(): Observable<boolean> {
    return confirmSession(this.apiService, this.router).pipe(map(user => !!user));
  }
}
