import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { ApiService } from '../services/api.service';
import { SignalrService } from '../services/signalr.service';
import { Book, Review, User } from '../models';
import { ReviewFormComponent } from '../components/review-form.component';
import { NavComponent } from '../components/nav.component';
import { computeAverageRating, getRatingStars } from '../utils';

@Component({
  selector: 'app-book-detail',
  standalone: true,
  // Angular 22 defaults components to OnPush. This component keeps state in plain
  // mutable fields updated from RxJS subscribe callbacks, so it needs eager checking.
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [CommonModule, RouterLink, ReviewFormComponent, NavComponent],
  templateUrl: './book-detail.page.html',
  styleUrls: ['./book-detail.page.css'],
})
export class BookDetailPage implements OnInit, OnDestroy {
  book: Book | null = null;
  reviews: Review[] = [];
  loading = true;
  currentUser: User | null = null;

  private availabilitySubscription?: Subscription;

  constructor(
    private apiService: ApiService,
    private route: ActivatedRoute,
    private router: Router,
    private signalrService: SignalrService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) {
      this.router.navigate(['/home']);
      return;
    }

    this.currentUser = this.apiService.getUser();
    this.loadBook(id);
    this.loadReviews(id);

    this.signalrService.connect();
    this.availabilitySubscription = this.signalrService.bookAvailabilityChanged$.subscribe((event) => {
      if (this.book && event.bookId === this.book.id) {
        this.book = { ...this.book, isAvailable: event.isAvailable };
      }
    });
  }

  ngOnDestroy(): void {
    this.availabilitySubscription?.unsubscribe();
  }

  loadBook(id: number): void {
    this.loading = true;
    this.apiService.getBookById(id).subscribe({
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

  loadReviews(id: number): void {
    this.apiService.getBookReviews(id).subscribe({
      next: (reviews) => (this.reviews = reviews),
    });
  }

  get canReview(): boolean {
    return this.currentUser?.role === 'Customer';
  }

  goToCheckout(): void {
    if (!this.book) return;
    if (this.currentUser?.role !== 'Customer') {
      alert('Only customers can check out books. Please login as a customer.');
      return;
    }
    if (!this.book.isAvailable) {
      alert('Sorry, this book is currently checked out. Please check back later.');
      return;
    }
    this.router.navigate(['/checkout', this.book.id]);
  }

  onReviewSubmitted(): void {
    if (this.book) this.loadReviews(this.book.id);
  }

  get averageRating(): number | null {
    return computeAverageRating(this.reviews) ?? null;
  }

  protected readonly getRatingStars = getRatingStars;
}
