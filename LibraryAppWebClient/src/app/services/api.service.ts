import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, switchMap, tap } from 'rxjs/operators';
import { User, Book, Review, Booking } from '../models';
import { computeAverageRating } from '../utils';
import { SignalrService } from './signalr.service';

const API_BASE_URL = '/api';
const USER_STORAGE_KEY = 'libraryUser';

function withAverageRating(book: Book): Book {
  return { ...book, averageRating: computeAverageRating(book.reviews ?? []) };
}

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);
  private signalrService = inject(SignalrService);
  private currentUser: User | null = this.readStoredUser();

  private readStoredUser(): User | null {
    const stored = localStorage.getItem(USER_STORAGE_KEY);
    if (!stored) return null;
    try {
      return JSON.parse(stored) as User;
    } catch {
      return null;
    }
  }

  private storeUser(user: User | null): void {
    this.currentUser = user;
    if (user) {
      localStorage.setItem(USER_STORAGE_KEY, JSON.stringify(user));
    } else {
      localStorage.removeItem(USER_STORAGE_KEY);
    }
  }

  getUser(): User | null {
    return this.currentUser;
  }

  isAuthenticated(): boolean {
    return !!this.currentUser;
  }

  /** Called by the auth interceptor when the server rejects our session cookie. */
  clearUser(): void {
    this.storeUser(null);
    this.signalrService.disconnect();
  }

  login(email: string, password: string): Observable<User> {
    // Login only sets the auth cookie and confirms the credentials; fetch the
    // profile afterwards so the caller learns the user's id/role.
    return this.http.post(`${API_BASE_URL}/auth/login`, { email, password }).pipe(
      switchMap(() => this.fetchCurrentUser())
    );
  }

  register(email: string, password: string, role: 'Librarian' | 'Customer'): Observable<void> {
    return this.http.post<void>(`${API_BASE_URL}/auth/register`, { email, password, role });
  }

  fetchCurrentUser(): Observable<User> {
    return this.http.get<User>(`${API_BASE_URL}/auth/me`).pipe(
      tap(user => this.storeUser(user))
    );
  }

  logout(): Observable<void> {
    return this.http.post<void>(`${API_BASE_URL}/auth/logout`, {}).pipe(
      tap(() => {
        this.storeUser(null);
        this.signalrService.disconnect();
      })
    );
  }

  getBooks(): Observable<Book[]> {
    return this.http.get<Book[]>(`${API_BASE_URL}/books`).pipe(
      map(books => books.map(withAverageRating))
    );
  }

  getBookById(id: number): Observable<Book> {
    return this.http.get<Book>(`${API_BASE_URL}/books/${id}`).pipe(map(withAverageRating));
  }

  searchBooks(query: string): Observable<Book[]> {
    return this.http.get<Book[]>(`${API_BASE_URL}/books/search?query=${encodeURIComponent(query)}`).pipe(
      map(books => books.map(withAverageRating))
    );
  }

  updateBook(id: number, updates: Partial<Book>): Observable<Book> {
    return this.http.put<Book>(`${API_BASE_URL}/books/${id}`, { ...updates, id });
  }

  deleteBook(id: number): Observable<void> {
    return this.http.delete<void>(`${API_BASE_URL}/books/${id}`);
  }

  addBook(book: Partial<Book>): Observable<Book> {
    return this.http.post<Book>(`${API_BASE_URL}/books`, book);
  }

  checkoutBook(bookId: number): Observable<{ message: string; dueDate: string }> {
    return this.http.post<{ message: string; dueDate: string }>(`${API_BASE_URL}/bookings`, { bookId });
  }

  returnBook(bookId: number): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${API_BASE_URL}/bookings/returns`, { bookId });
  }

  getAllBookings(): Observable<Booking[]> {
    return this.http.get<Booking[]>(`${API_BASE_URL}/bookings`);
  }

  submitReview(bookId: number, rating: number, message: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${API_BASE_URL}/reviews`, { bookId, rating, message });
  }

  getBookReviews(bookId: number): Observable<Review[]> {
    return this.http.get<Review[]>(`${API_BASE_URL}/reviews/${bookId}`);
  }
}
