import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { Book } from '../../models';
import { NavComponent } from '../../components/nav.component';

@Component({
  selector: 'app-home',
  standalone: true,
  // Angular 22 defaults components to OnPush. This component keeps state in plain
  // mutable fields updated from RxJS subscribe callbacks, so it needs eager checking.
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [CommonModule, FormsModule, RouterLink, NavComponent],
  templateUrl: './home.page.html',
  styleUrls: ['./home.page.css'],
})
export class HomePage implements OnInit {
  featuredBooks: Book[] = [];
  loading = true;
  searchQuery = '';

  constructor(private apiService: ApiService, private router: Router) {}

  ngOnInit(): void {
    this.loadFeaturedBooks();
  }

  loadFeaturedBooks(): void {
    this.loading = true;
    this.apiService.getBooks().subscribe({
      next: (books) => {
        this.featuredBooks = [...books].sort(() => 0.5 - Math.random()).slice(0, 5);
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  goToBookDetail(id: number): void {
    this.router.navigate(['/book', id]);
  }

  onSearch(): void {
    if (this.searchQuery.trim()) {
      this.router.navigate(['/search'], { queryParams: { query: this.searchQuery.trim() } });
    }
  }

  truncate(text: string, length: number): string {
    return text.length > length ? text.slice(0, length) + '…' : text;
  }
}
