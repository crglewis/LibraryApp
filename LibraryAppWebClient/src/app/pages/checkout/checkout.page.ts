import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { Book, User } from '../../models';
import { NavComponent } from '../../components/nav.component';

@Component({
  selector: 'app-checkout',
  standalone: true,
  // Angular 22 defaults components to OnPush. This component keeps state in plain
  // mutable fields updated from RxJS subscribe callbacks, so it needs eager checking.
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [CommonModule, RouterLink, NavComponent],
  templateUrl: './checkout.page.html',
  styleUrls: ['./checkout.page.css'],
})
export class CheckoutPage implements OnInit {
  book: Book | null = null;
  loading = true;
  currentUser: User | null = null;
  confirmation: { dueDate: string } | null = null;
  error = '';

  constructor(
    private apiService: ApiService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.currentUser = this.apiService.getUser();
    const bookId = Number(this.route.snapshot.paramMap.get('bookId'));
    if (!bookId) {
      this.router.navigate(['/home']);
      return;
    }
    this.apiService.getBookById(bookId).subscribe({
      next: (data) => {
        this.book = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.router.navigate(['/home']);
      },
    });
  }

  confirmCheckout(): void {
    if (!this.book) return;
    this.apiService.checkoutBook(this.book.id).subscribe({
      next: (result) => {
        this.confirmation = { dueDate: result.dueDate };
      },
      error: (err) => {
        this.error = err?.error || 'Checkout failed. Please try again.';
      },
    });
  }
}
