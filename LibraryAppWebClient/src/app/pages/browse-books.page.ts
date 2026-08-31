import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../services/api.service';
import { Book } from '../models';
import { NavComponent } from '../components/nav.component';
import { getRatingStars } from '../utils';

@Component({
  selector: 'app-browse-books',
  standalone: true,
  // Angular 22 defaults components to OnPush. This component keeps state in plain
  // mutable fields updated from RxJS subscribe callbacks, so it needs eager checking.
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [CommonModule, FormsModule, NavComponent],
  templateUrl: './browse-books.page.html',
  styleUrls: ['./browse-books.page.css'],
})
export class BrowseBooksPage implements OnInit {
  books: Book[] = [];
  filteredBooks: Book[] = [];
  loading = true;

  searchQuery = '';
  authorFilter = '';
  availabilityFilter: '' | 'available' | 'unavailable' = '';
  sortBy: '' | 'title' | 'author' | 'averageRating' = '';

  constructor(private apiService: ApiService, private router: Router) {}

  ngOnInit(): void {
    this.loadBooks();
  }

  loadBooks(): void {
    this.loading = true;
    this.apiService.getBooks().subscribe({
      next: (data) => {
        this.books = data;
        this.loading = false;
        this.applyFilters();
      },
      error: () => {
        this.loading = false;
        alert('Failed to load books');
      },
    });
  }

  applyFilters(): void {
    let result = this.books.filter((book) => {
      const matchesSearch = !this.searchQuery || book.title.toLowerCase().includes(this.searchQuery.toLowerCase());
      const matchesAuthor = !this.authorFilter || book.author === this.authorFilter;
      const matchesAvailability =
        !this.availabilityFilter ||
        (this.availabilityFilter === 'available' ? book.isAvailable : !book.isAvailable);
      return matchesSearch && matchesAuthor && matchesAvailability;
    });

    if (this.sortBy === 'title') {
      result = [...result].sort((a, b) => a.title.localeCompare(b.title));
    } else if (this.sortBy === 'author') {
      result = [...result].sort((a, b) => a.author.localeCompare(b.author));
    } else if (this.sortBy === 'averageRating') {
      result = [...result].sort((a, b) => (b.averageRating || 0) - (a.averageRating || 0));
    }

    this.filteredBooks = result;
  }

  get uniqueAuthors(): string[] {
    return [...new Set(this.books.map((b) => b.author))].sort();
  }

  goToBookDetail(id: number): void {
    this.router.navigate(['/book', id]);
  }

  protected readonly getRatingStars = getRatingStars;
}
