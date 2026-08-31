# Library Demo App - Full Stack Book Management System

A complete full-stack web application for managing a library book inventory system with Angular frontend and .NET 8 backend.

## 📋 Overview

This project demonstrates proficiency in:
- .NET 8 Web API development
- ASP.NET Core Identity & Role-based access control
- Entity Framework Core with SQL Server
- Angular SPA (Single Page Application) development
- Customer and Librarian roles with different permissions

## 🎯 Features

### Customer Features:
- ✅ Browse all available books
- ✅ Search books by title (partial match)
- ✅ Filter books by author and availability status
- ✅ View detailed book information (Publisher, Publication Date, Category, ISBN, Page Count)
- ✅ Check out books for 5-day loan period
- ✅ Leave customer reviews with star ratings
- ✅ Track personal book history

### Librarian Features:
- ✅ Add new books to inventory
- ✅ Edit existing book information
- ✅ Remove/delete books from inventory
- ✅ Mark books as returned
- ✅ View all checked-out books and due dates
- ✅ Manage return requests

### Bonus Features:
- ✅ Librarian dashboard listing all checked-out books and due dates (see Librarian Features above)
- ✅ Angular Material used on the Inventory Management page (form fields, buttons, table, snackbar notifications)
- ✅ xUnit unit tests for the API (`LibraryApp.Tests`) covering book CRUD/search and checkout/return business logic
- ✅ [Database diagram](docs/database-diagram.md) (Mermaid ERD)
- ✅ Real-time book availability via SignalR — when a book is checked out or returned, every connected client's Browse and Book Detail pages update instantly without a refresh

## 🚀 Technology Stack

### Backend (.NET 8)
- .NET 8.0 Web API
- ASP.NET Core Identity for authentication
- Entity Framework Core ORM (Code First approach)
- SQL Server database with migrations
- Bogus library for test data seeding
- Role-based authorization middleware
- SignalR for real-time book availability broadcasts (`/hubs/books`)
- xUnit + EF Core InMemory provider for API unit tests (`LibraryApp.Tests`)

### Frontend (Angular)
- Angular Standalone Components (Latest version)
- TypeScript
- RxJS for reactive programming
- HttpClient with interceptors
- Angular Material (used on the Inventory Management page)
- `@microsoft/signalr` client for live book availability updates
- Pure CSS elsewhere (no framework-specific stylesheets)
- Responsive design with modern aesthetics

## 📁 Project Structure

```
LibraryDemoApp/
│
├── LibraryApp/                  # .NET Backend API
│   ├── Controllers/             # API endpoints (Books, Auth, Bookings, Reviews)
│   ├── Data/                   # Entity Framework DbContext and configurations
│   ├── Hubs/                  # SignalR hub (BookHub) for live availability updates
│   ├── Migrations/            # Database schema changes
│   ├── Models/                # Domain models (Book, User, Review, etc.)
│   ├── appsettings.json       # Application configuration
│   └── Program.cs             # Application entry point with DI setup
│
├── LibraryApp.Tests/            # xUnit tests for the API (Books/Bookings controllers)
│
├── docs/
│   └── database-diagram.md      # Mermaid ER diagram of the SQL Server schema
│
├── LibraryAppWebClient/        # Angular Frontend
│   ├── src/
│   │   ├── app/
│   │   │   ├── components/    # Reusable UI components (ReviewForm, etc.)
│   │   │   ├── services/      # API service classes
│   │   │   ├── guards/        # Route guards for auth
│   │   │   ├── pages/         # Page-level components
│   │   │   ├── routing.ts     # Angular routes configuration
│   │   │   ├── models.ts      # TypeScript interfaces matching backend entities
│   │   │   └── app.ts         # Root component
│   │   ├── styles.css          # Global styles
│   │   ├── main.ts             # Entry point
│   │   └── index.html          # HTML template
│   └── public/                 # Static assets (images, fonts, etc.)
│
├── requirements.md              # Full technical requirements document
├── implementationplan.md        # Implementation roadmap and phases
└── README.md                    # This file
```

## ⚙️ Setup & Installation

### Prerequisites
- [.NET SDK 8.0+](https://dotnet.microsoft.com/download) (developed against .NET 10)
- [Node.js](https://nodejs.org/) 18+ and npm
- SQL Server or SQL Server Express, reachable as the local default instance (`.`)

### 1. Database credentials (required — not a standard .NET setting)

The API reads its SQL Server login from two environment variables and will **fail to start** without them:

```bash
export sqluser='your_sql_server_username'
export sqlpass='your_sql_server_password'
```

These must be valid credentials for a SQL Server login with rights to create/access a database named `LibraryDB` on the local default instance (`Server=.`). The connection string is built from these at startup — see `LibraryApp/Program.cs`. Encryption is disabled (`Encrypt=False`) for local development to avoid SSL certificate issues.

### 2. Run the backend API

```bash
cd LibraryApp
dotnet run
```

On startup the app automatically applies EF Core migrations (creating `LibraryDB` if it doesn't exist) and seeds the database with 30 randomly generated books via Bogus. The API listens on `http://localhost:5196` (see `LibraryApp/Properties/launchSettings.json`). In development, interactive API docs are available at `http://localhost:5196/swagger`.

### 3. Run the frontend

In a separate terminal:

```bash
cd LibraryAppWebClient
npm install
npm start
```

This serves the Angular app at `http://localhost:4200` and proxies any `/api/*` or `/hubs/*` request to the backend at `http://localhost:5196` (see `LibraryAppWebClient/proxy.conf.json`). Open `http://localhost:4200` in a browser, register an account (choosing Customer or Librarian), and log in.

### 4. Run the API unit tests

```bash
dotnet test LibraryApp.Tests
```

No database or running server is required — the tests use EF Core's InMemory provider against the real controllers.
