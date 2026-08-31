import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../services/api.service';
import { Book } from '../models';
import { NavComponent } from '../components/nav.component';

@Component({
  selector: 'app-search',
  standalone: true,
  // Angular 22 defaults components to OnPush. This component keeps state in plain
  // mutable fields updated from RxJS subscribe callbacks, so it needs eager checking.
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [CommonModule, FormsModule, NavComponent],
  templateUrl: './search.page.html',
  styleUrls: ['./search.page.css'],
})
export class SearchPage implements OnInit {
  searchQuery = '';
  results: Book[] = [];
  loading = false;
  searched = false;

  constructor(
    private apiService: ApiService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const queryParam = this.route.snapshot.queryParamMap.get('query');
    if (queryParam) {
      this.searchQuery = queryParam;
      this.search();
    }
  }

  onSearch(): void {
    this.search();
  }

  private search(): void {
    if (!this.searchQuery.trim()) return;
    this.loading = true;
    this.apiService.searchBooks(this.searchQuery.trim()).subscribe({
      next: (books) => {
        this.results = books;
        this.loading = false;
        this.searched = true;
      },
      error: () => {
        this.loading = false;
        this.searched = true;
      },
    });
  }

  goToBook(id: number): void {
    this.router.navigate(['/book', id]);
  }

  truncate(text: string, length: number): string {
    return text.length > length ? text.slice(0, length) + '…' : text;
  }

  getRatingStars(rating?: number): string {
    const rounded = Math.round(rating || 0);
    let stars = '';
    for (let i = 1; i <= 5; i++) stars += i <= rounded ? '★' : '☆';
    return stars;
  }
}
