Implementation Plan: Library Inventory System

 Project Overview

 A full-stack web application for a library to manage a book inventory.
 - Backend: .NET 8 Web API, Entity Framework Core, SQL Server, ASP.NET Core Identity.
 - Frontend: TypeScript, SPA Framework (React/Angular/Vue).

 Current Status

 - Backend Project: Initialized with .NET 8.
 - Database: SQL Server configured with EF Core, Migrations, and basic Seeding.
 - Models: Book, ApplicationUser, Booking, and Review models created.
 - Controllers: BooksController, BookingsController, ReviewsController, and AuthController implemented.
 - Infrastructure: Database schema is generated and initial seed data is installed.

 ────────────────────────────────────────────────────────────────────────────────

 Phase 1: Backend API Expansion (Identity & Core Logic) - COMPLETED

 ### 1. Identity & Authentication

 - User Model: Extended the Identity system to support the "Librarian" and "Customer" roles.
 - Authentication Flow: Registration and Login endpoints implemented.
 - Authorization: Applied [Authorize] attributes with Role-based access control.
 - Registration: Created Registration endpoint allowing role selection (Librarian/Customer).

 ### 2. Book Management & Interactions

 - Management (Librarian only):
     - POST /api/books: Create new book.
     - PUT /api/books/{id}: Update existing book details.
     - DELETE /api/books/{id}: Remove a book from the inventory.
 - Booking & Circulation:
     - POST /api/bookings: Customer can check out a book if IsAvailable is true.
     - Logic: Calculate 5-day expiry date and check for Unique book copies (only 1 available).
     - POST /api/returns: Librarian-only endpoint to mark a book as returned and update its IsAvailable status.
 - Search & Filtering:
     - Add GET /api/books/search?query={text}: Partial match on title.
     - Add filters for Author and Availability status.

 ### 3. Reviews & Details

 - Review Model: Created Review entity (Message, Rating, BookId, UserId).
 - Reviewing: POST /api/reviews allows customers to submit reviews.
 - Detailed View: Ensure GET /api/books/{id} returns all fields (Publisher, Publication Date, Category, ISBN, Page Count) and associated labels/reviews.

 ────────────────────────────────────────────────────────────────────────────────

 Phase 2: Frontend Development (UI/UX)

 ### 1. Setup & Authentication

 - Framework Setup: Initialize the SPA project (Angular).
 - Auth Pages:
     - Registration: Role-based toggle (Librarian/Customer).
     - Login/Logout: State management for user sessions.
 - Protected Routes: Ensure only logged-in users can see the Library content.

 ### 2. Customer Features

 - Home/Browse: Display "Featured Books" (Random list) with Title, Author, Description, Cover Image, and Average Rating.
 - Filters: UI components for filtering by Author and Availability.
 - Search Bar: Real-time or submission-based search by partial Title.
 - Book Detail Page: Detailed view of book metadata and a review section.
 - Checkout: A simple interaction to borrow a book and receive an "Available" or "Borrowed" confirmation.

 ### 3. Librarian Dashboard

 - Inventory Management: A dedicated interface for adding/editing/deleting books.
 - Returns Management: List of checked-out books and their due dates for the librarian to process returns.

 ────────────────────────────────────────────────────────────────────────────────

 Phase 3: Integration & Final Requirements

 ### 1. Data & Experience

 - Refined Seeding: Ensure enough variety of books are seeded using Bogus to test search/sort.
 - Image Handling: Ensure CoverImage URLs are correctly rendered.
 - Validation: Implement robust server-side and client-side validation (e.g., rating must be 1-5).

 ### 2. Technical Polish

 - Unit Testing: Create tests for the Book Management and Booking logic.
 - Swagger/OpenAPI: Ensure all endpoints are documented and testable via the Swagger UI.
 - Final Review: Ensure the application adheres to production readiness (clean code, proper error handling, and proper DI usage).
