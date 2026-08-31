import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../services/api.service';
import { Book } from '../models';
import { NavComponent } from '../components/nav.component';

interface BookFormModel {
  id?: number;
  title: string;
  author: string;
  description: string;
  coverImage: string;
  publisher: string;
  publicationDate: string;
  category: string;
  isbn: string;
  pageCount: number | null;
}

function emptyForm(): BookFormModel {
  return {
    title: '',
    author: '',
    description: '',
    coverImage: '',
    publisher: '',
    publicationDate: '',
    category: '',
    isbn: '',
    pageCount: null,
  };
}

@Component({
  selector: 'app-inventory',
  standalone: true,
  // Angular 22 defaults components to OnPush. This component keeps state in plain
  // mutable fields updated from RxJS subscribe callbacks, so it needs eager checking.
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [CommonModule, FormsModule, NavComponent],
  templateUrl: './inventory.page.html',
  styleUrls: ['./inventory.page.css'],
})
export class InventoryPage implements OnInit {
  books: Book[] = [];
  isLoading = false;
  viewType: 'list' | 'form' = 'list';
  bookForm: BookFormModel = emptyForm();

  constructor(public apiService: ApiService, private router: Router) {}

  ngOnInit(): void {
    this.loadBooks();
  }

  loadBooks(): void {
    this.isLoading = true;
    this.apiService.getBooks().subscribe({
      next: (data) => {
        this.books = data;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        alert('Failed to load books');
      },
    });
  }

  switchView(view: 'list' | 'form'): void {
    this.viewType = view;
    if (view === 'form') this.bookForm = emptyForm();
  }

  editBook(book: Book): void {
    this.bookForm = {
      id: book.id,
      title: book.title,
      author: book.author,
      description: book.description || '',
      coverImage: book.coverImage || '',
      publisher: book.publisher || '',
      publicationDate: book.publicationDate ? book.publicationDate.slice(0, 10) : '',
      category: book.category || '',
      isbn: book.isbn || '',
      pageCount: book.pageCount ?? null,
    };
    this.viewType = 'form';
  }

  // The backend's PublicationDate/PageCount fields are non-nullable, so posting
  // the form's blank-string/null placeholders fails JSON deserialization before
  // the request even reaches model validation. Omit them when unset instead.
  private buildPayload(): Partial<Book> {
    const { publicationDate, pageCount, ...rest } = this.bookForm;
    const payload: Partial<Book> = { ...rest } as unknown as Partial<Book>;
    if (publicationDate) payload.publicationDate = publicationDate;
    if (pageCount !== null) payload.pageCount = pageCount;
    return payload;
  }

  saveBook(): void {
    if (!this.bookForm.title.trim() || !this.bookForm.author.trim()) {
      alert('Title and author are required');
      return;
    }

    const payload = this.buildPayload();

    if (this.bookForm.id) {
      this.apiService.updateBook(this.bookForm.id, payload).subscribe({
        next: () => {
          alert('Book updated successfully');
          this.switchView('list');
          this.loadBooks();
        },
        error: () => alert('Failed to update book'),
      });
    } else {
      this.apiService.addBook({ ...payload, isAvailable: true }).subscribe({
        next: () => {
          alert('New book added');
          this.switchView('list');
          this.loadBooks();
        },
        error: () => alert('Failed to add book'),
      });
    }
  }

  deleteBook(book: Book): void {
    if (!confirm(`Are you sure you want to delete "${book.title}"?`)) return;
    this.apiService.deleteBook(book.id).subscribe(() => {
      this.loadBooks();
    });
  }

  backToDashboard(): void {
    this.router.navigate(['/admin']);
  }
}
