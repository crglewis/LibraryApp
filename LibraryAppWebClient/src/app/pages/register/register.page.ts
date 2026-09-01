import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-register',
  standalone: true,
  // Angular 22 defaults components to OnPush. This component keeps state in plain
  // mutable fields updated from RxJS subscribe callbacks, so it needs eager checking.
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [FormsModule, CommonModule, RouterLink],
  templateUrl: './register.page.html',
  styleUrls: ['./register.page.css'],
})
export class RegisterPage {
  email = '';
  password = '';
  confirmPassword = '';
  selectedRole: 'Librarian' | 'Customer' = 'Customer';
  error = '';
  submitting = false;

  constructor(private apiService: ApiService, private router: Router) {}

  register(): void {
    this.error = '';

    if (this.password !== this.confirmPassword) {
      this.error = 'Passwords do not match';
      return;
    }
    if (this.password.length < 6) {
      this.error = 'Password must be at least 6 characters';
      return;
    }

    this.submitting = true;
    this.apiService.register(this.email.trim(), this.password, this.selectedRole).subscribe({
      next: () => {
        this.submitting = false;
        alert('Registration successful! Please login.');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.submitting = false;
        this.error = err?.error?.[0]?.description || 'Registration failed. Please try again.';
      },
    });
  }
}
