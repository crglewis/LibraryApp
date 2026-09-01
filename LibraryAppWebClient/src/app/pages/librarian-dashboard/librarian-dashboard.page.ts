import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { Booking } from '../../models';
import { NavComponent } from '../../components/nav.component';

@Component({
  selector: 'app-librarian-dashboard',
  standalone: true,
  // Angular 22 defaults components to OnPush. This component keeps state in plain
  // mutable fields updated from RxJS subscribe callbacks, so it needs eager checking.
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [CommonModule, RouterLink, NavComponent],
  templateUrl: './librarian-dashboard.page.html',
  styleUrls: ['./librarian-dashboard.page.css'],
})
export class LibrarianDashboardPage implements OnInit {
  bookings: Booking[] = [];
  isLoading = false;

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.loadBookings();
  }

  loadBookings(): void {
    this.isLoading = true;
    this.apiService.getAllBookings().subscribe({
      next: (data) => {
        this.bookings = data;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        alert('Failed to load bookings');
      },
    });
  }

  get activeBookings(): Booking[] {
    return this.bookings.filter((b) => !b.isReturned);
  }

  get overdueBookings(): Booking[] {
    const now = Date.now();
    return this.activeBookings.filter((b) => new Date(b.dueDate).getTime() < now);
  }

  isOverdue(booking: Booking): boolean {
    return !booking.isReturned && new Date(booking.dueDate).getTime() < Date.now();
  }

  markReturned(booking: Booking): void {
    if (!confirm(`Mark "${booking.book?.title}" as returned?`)) return;
    this.apiService.returnBook(booking.bookId).subscribe({
      next: () => this.loadBookings(),
      error: () => alert('Failed to process return'),
    });
  }
}
