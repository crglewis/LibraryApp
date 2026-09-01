import { Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { roleGuard } from './guards/role.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'login', loadComponent: () => import('./pages/login/login.page').then(m => m.LoginPage) },
  { path: 'register', loadComponent: () => import('./pages/register/register.page').then(m => m.RegisterPage) },

  { path: 'home', loadComponent: () => import('./pages/home/home.page').then(m => m.HomePage), title: 'Library Home', canActivate: [AuthGuard] },
  { path: 'browse/books', loadComponent: () => import('./pages/browse-books/browse-books.page').then(m => m.BrowseBooksPage), title: 'Browse Books', canActivate: [AuthGuard] },
  { path: 'search', loadComponent: () => import('./pages/search/search.page').then(m => m.SearchPage), title: 'Search Books', canActivate: [AuthGuard] },
  { path: 'book/:id', loadComponent: () => import('./pages/book-detail/book-detail.page').then(m => m.BookDetailPage), title: 'Book Details', canActivate: [AuthGuard] },
  { path: 'checkout/:bookId', loadComponent: () => import('./pages/checkout/checkout.page').then(m => m.CheckoutPage), title: 'Checkout', canActivate: [AuthGuard, roleGuard], data: { role: 'Customer' } },

  // Librarian-only routes
  { path: 'admin', loadComponent: () => import('./pages/librarian-dashboard/librarian-dashboard.page').then(m => m.LibrarianDashboardPage), title: 'Librarian Dashboard', canActivate: [AuthGuard, roleGuard], data: { role: 'Librarian' } },
  { path: 'inventory', loadComponent: () => import('./pages/inventory/inventory.page').then(m => m.InventoryPage), title: 'Manage Inventory', canActivate: [AuthGuard, roleGuard], data: { role: 'Librarian' } },

  { path: '**', redirectTo: '/home' },
];
