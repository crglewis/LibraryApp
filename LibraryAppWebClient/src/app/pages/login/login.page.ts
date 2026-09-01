import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-login',
  standalone: true,
  // Angular 22 defaults components to OnPush. This component keeps state in plain
  // mutable fields updated from RxJS subscribe callbacks, so it needs eager checking.
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [FormsModule, CommonModule, RouterLink],
  templateUrl: './login.page.html',
  styleUrls: ['./login.page.css'],
})
export class LoginPage {
  email = '';
  password = '';
  error = '';
  loginInProgress = false;

  constructor(private apiService: ApiService, private router: Router) {}

  login(): void {
    this.error = '';
    this.loginInProgress = true;
    this.apiService.login(this.email.trim(), this.password).subscribe({
      next: () => {
        this.loginInProgress = false;
        this.router.navigate(['/home']);
      },
      error: () => {
        this.loginInProgress = false;
        this.error = 'Invalid email or password';
      },
    });
  }
}
