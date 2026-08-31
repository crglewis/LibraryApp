/**
 * Book model - mirrors the .NET Book entity
 */
export interface Book {
  id: number;
  title: string;
  author: string;
  description?: string;
  coverImage?: string;
  publisher?: string;
  publicationDate?: string;
  category?: string;
  isbn?: string;
  pageCount?: number;
  isAvailable: boolean;
  reviews?: Review[];
  averageRating?: number;
}

/**
 * User model - matches ASP.NET Identity ApplicationUser with library roles
 */
export interface User {
  id: string;
  email: string;
  role: 'Librarian' | 'Customer';
}

/**
 * Review model for customer book reviews
 */
export interface Review {
  id: number;
  bookId: number;
  userId: string;
  userName: string;
  message: string;
  rating: number; // 1-5 stars
  createdAt: string;
}

/**
 * Booking model for the book checkout system
 */
export interface Booking {
  id: number;
  bookId: number;
  userId: string;
  checkoutDate: string;
  dueDate: string;
  returnDate?: string | null;
  isReturned: boolean;
  book?: Book;
}
