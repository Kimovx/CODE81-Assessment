<div align="center">

# 📚 Library Management System

### A production-ready RESTful API built with ASP.NET Core 10 & SQL Server

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com)
[![EF Core](https://img.shields.io/badge/EF_Core-10.0-512BD4?style=flat-square&logo=dotnet)](https://docs.microsoft.com/en-us/ef/core/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?style=flat-square&logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![JWT](https://img.shields.io/badge/Auth-JWT_+_Refresh_Tokens-000000?style=flat-square&logo=jsonwebtokens)](https://jwt.io)
[![Swagger](https://img.shields.io/badge/Docs-Swagger_UI-85EA2D?style=flat-square&logo=swagger)](https://swagger.io)

</div>

---

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Features](#features)
- [Database Schema](#database-schema)
- [API Reference](#api-reference)
- [Authentication & Authorization](#authentication--authorization)
- [Security Design](#security-design)
- [Getting Started](#getting-started)
- [Default Credentials](#default-credentials)
- [Project Structure](#project-structure)
- [Design Decisions](#design-decisions)
- [Documentation](#documentation)
- [Known Limitations](#known-limitations)

---

## Overview

A full-featured Library Management System REST API enabling librarians and staff to manage books, members, and borrowing workflows. The system implements role-based access control, JWT authentication with refresh token rotation, and a clean layered architecture following Clean Architecture principles.

Built as part of the **CODE81 Technical Assessment**.

---

## Architecture

The project follows **Clean Architecture** with four distinct layers that enforce a strict dependency rule — outer layers depend on inner ones, never the reverse.

```
┌─────────────────────────────────────────────────────┐
│                  Presentation Layer                  │
│         Controllers · Middlewares · Filters          │
├─────────────────────────────────────────────────────┤
│                  Application Layer                   │
│    Services · DTOs · Interfaces · Exceptions         │
├─────────────────────────────────────────────────────┤
│                 Infrastructure Layer                 │
│   Repositories · EF DbContext · JWT · File Storage   │
├─────────────────────────────────────────────────────┤
│                    Domain Layer                      │
│            Entities · Enums · Constants              │
└─────────────────────────────────────────────────────┘
```

### Key patterns used

| Pattern | Purpose |
|---|---|
| Repository Pattern | Abstracts data access behind interfaces; each entity has its own repository |
| Unit of Work | Coordinates multiple repository operations in a single atomic transaction |
| Service Layer | Encapsulates all business logic; controllers are thin orchestrators only |
| DTO separation | Distinct `CreateDto`, `UpdateDto`, and response `Dto` types per entity |
| Global Exception Middleware | Centralises error handling; translates domain exceptions to HTTP responses |
| Activity Logging Middleware | Logs every authenticated request with path, method, IP, user agent, and outcome |

---

## Tech Stack

| Concern | Technology |
|---|---|
| Framework | ASP.NET Core 10 Web API |
| ORM | Entity Framework Core 10 |
| Database | SQL Server (LocalDB for dev) |
| Authentication | ASP.NET Core Identity + JWT Bearer |
| Password hashing | BCrypt.Net-Next |
| API docs | Swagger / Swashbuckle |
| File storage | Local filesystem (`wwwroot/`) |

---

## Features

### Core

- **Books** — Full CRUD with cover image upload, multi-author support, hierarchical categories, publisher info, and `Available / Borrowed` status tracking
- **Authors** — Managed independently; many-to-many relationship with books
- **Categories** — Self-referencing hierarchy (parent → child subcategories) via `ParentCategoryId`. Example: `Programming → Software Engineering → Design Patterns`
- **Publishers** — Full contact info (name, country, email, phone); one-to-many with books
- **Library Members** — Borrower records with membership lifecycle status (`Active`, `Suspended`, `Expired`)
- **Borrowing Transactions** — Full borrow and return workflow; records which staff member issued and which processed the return
- **System Users** — Staff accounts with role assignment, managed separately from library members

### Auth & Security

- JWT access tokens (configurable expiry via `appsettings.json`, default 60 min)
- Refresh tokens stored hashed (BCrypt) in the database with rotation
- **Reuse detection** — if a revoked refresh token is presented, a `RefreshTokenReuseDetectedException` is thrown
- Refresh token delivered via `HttpOnly`, `Secure`, `SameSite=None` cookie — not exposed to JavaScript
- Role-based access control enforced at the endpoint level with `[Authorize(Roles = "...")]`
- User activity logging middleware — every authenticated API request is logged with method, path, IP address, user agent, and success/failure status

### Infrastructure

- Pagination on all list endpoints (`pageNumber`, `pageSize` query params)
- Concurrency conflict handling on book borrow — uses EF Core `[Timestamp]` row-version + `DbUpdateConcurrencyException` to prevent two staff members double-booking the same book simultaneously
- Global exception middleware translates typed domain exceptions to consistent JSON error responses
- `DeleteBehavior.Restrict` configured globally to prevent accidental cascading deletes

---

## Database Schema

### Entity Relationship Overview

```
AppUser (Identity)
  ├── UserLoginLogs        (activity log — every authenticated request)
  ├── BorrowingTransactions (as CreatedBy  — staff who issued the book)
  └── BorrowingTransactions (as ReturnedBy — staff who processed the return)

Book
  ├── Publisher            (many-to-one)
  ├── Authors              (many-to-many → BooksAuthors join table)
  ├── Categories           (many-to-many → BooksCategories join table)
  └── BorrowingTransactions

Category
  └── ParentCategory       (self-referencing FK for hierarchical structure)

LibraryMember
  └── BorrowingTransactions

RefreshToken
  └── AppUser
```

> 📎 See [`Docs/ERD.png`](Docs/ERD.png) for the full visual Entity Relationship Diagram.

### Key tables

| Table | Description |
|---|---|
| `AspNetUsers` | System staff accounts (ASP.NET Core Identity) |
| `AspNetRoles` | `Admin`, `Librarian`, `Staff` |
| `Books` | Core book records — title, ISBN, year, language, edition, summary, status, cover image URL |
| `Authors` | Author records |
| `BooksAuthors` | Many-to-many join — books ↔ authors |
| `Categories` | Hierarchical categories with `ParentCategoryId` self-ref FK |
| `BooksCategories` | Many-to-many join — books ↔ categories |
| `Publishers` | Publisher records with country, email, and phone |
| `LibraryMembers` | Borrower/patron records |
| `BorrowingTransactions` | Borrow history — borrow date, due date, return date, status, issuing staff, returning staff |
| `RefreshTokens` | Hashed refresh tokens with expiry (`ExpiresAt`) and revocation (`RevokedAt`) timestamps |
| `UserLoginLogs` | Per-request activity log — action, IP address, user agent, success/failure |

### Status enumerations

**Book status**

| Value | Meaning |
|---|---|
| `0` — Available | Book is on the shelf and can be borrowed |
| `1` — Borrowed | Book is currently checked out |

**Transaction status**

| Value | Meaning |
|---|---|
| `0` — Active | Currently borrowed, not yet returned |
| `1` — Returned | Successfully returned |
| `2` — Overdue | Past due date, not yet returned |
| `3` — Lost | Reported lost |

**Member status**

| Value | Meaning |
|---|---|
| `0` — Active | Membership in good standing |
| `1` — Suspended | Temporarily suspended |
| `2` — Expired | Membership lapsed |

---

## API Reference

All endpoints require a `Bearer` token in the `Authorization` header unless noted. Import [`Docs/CODE81_Assessment_postman_collection.json`](Docs/CODE81_Assessment_postman_collection.json) into Postman to try all endpoints immediately.

### Auth — `POST /api/auth`

| Method | Endpoint | Auth required | Description |
|---|---|---|---|
| `POST` | `/api/auth` | No | Login — returns access token; sets refresh token as `HttpOnly` cookie |
| `POST` | `/api/auth/refresh` | No | Rotate refresh token — reads from cookie, returns new access token |

**Login request**
```json
{ "username": "admin", "password": "Admin@123456" }
```

**Login response**
```json
{ "accessToken": "<jwt>" }
```
> The refresh token is set as an `HttpOnly` cookie named `rt`. It does not appear in the response body.

---

### Books — `/api/books`

| Method | Endpoint | Role required | Description |
|---|---|---|---|
| `GET` | `/api/books` | Admin, Librarian, Staff | Paginated list |
| `GET` | `/api/books/{id}` | Admin, Librarian, Staff | Single book |
| `POST` | `/api/books` | Admin, Librarian | Create (`multipart/form-data` for image upload) |
| `PUT` | `/api/books/{id}` | Admin, Librarian | Update |
| `DELETE` | `/api/books/{id}` | Admin only | Delete |

**Create book — form fields**

| Field | Type | Required |
|---|---|---|
| `title` | string | Yes |
| `isbn` | string | Yes |
| `publicationYear` | int | Yes |
| `language` | string | Yes |
| `edition` | string | Yes |
| `summary` | string | Yes |
| `publisherId` | int | Yes |
| `coverImage` | file | No |
| `authorIds` | int[] | Yes |
| `categoryIds` | int[] | Yes |

**Pagination:** `?pageNumber=1&pageSize=10`

---

### Authors — `/api/authors`

| Method | Endpoint | Role |
|---|---|---|
| `GET` | `/api/authors` | Admin, Librarian, Staff |
| `GET` | `/api/authors/{id}` | Admin, Librarian, Staff |
| `POST` | `/api/authors` | Admin, Librarian |
| `PUT` | `/api/authors/{id}` | Admin, Librarian |
| `DELETE` | `/api/authors/{id}` | Admin only |

---

### Categories — `/api/categories`

| Method | Endpoint | Role |
|---|---|---|
| `GET` | `/api/categories` | Admin, Librarian, Staff |
| `GET` | `/api/categories/{id}` | Admin, Librarian, Staff |
| `POST` | `/api/categories` | Admin, Librarian |
| `PUT` | `/api/categories/{id}` | Admin, Librarian |
| `DELETE` | `/api/categories/{id}` | Admin only |

> To create a subcategory, include `"parentCategoryId": <id>` in the request body. Omit it for a top-level category.

---

### Publishers — `/api/publishers`

| Method | Endpoint | Role |
|---|---|---|
| `GET` | `/api/publishers` | Admin, Librarian, Staff |
| `GET` | `/api/publishers/{id}` | Admin, Librarian, Staff |
| `POST` | `/api/publishers` | Admin, Librarian |
| `PUT` | `/api/publishers/{id}` | Admin, Librarian |
| `DELETE` | `/api/publishers/{id}` | Admin only |

---

### Library Members — `/api/librarymembers`

| Method | Endpoint | Role |
|---|---|---|
| `GET` | `/api/librarymembers` | Admin, Librarian, Staff |
| `GET` | `/api/librarymembers/{id}` | Admin, Librarian, Staff |
| `POST` | `/api/librarymembers` | Admin, Librarian |
| `PUT` | `/api/librarymembers/{id}` | Admin, Librarian |
| `DELETE` | `/api/librarymembers/{id}` | Admin only |

---

### Borrowing — `/api/borrowing`

| Method | Endpoint | Role | Description |
|---|---|---|---|
| `GET` | `/api/borrowing` | Admin, Librarian | Paginated transaction history |
| `GET` | `/api/borrowing/{id}` | Admin, Librarian | Single transaction |
| `POST` | `/api/borrowing/borrow` | Admin, Librarian, Staff | Issue a book to a member |
| `POST` | `/api/borrowing/return` | Admin, Librarian, Staff | Process a return |

**Borrow request**
```json
{
  "memberId": 1,
  "bookId": 3,
  "dueDate": "2025-08-01T00:00:00Z",
  "notes": "Optional staff note"
}
```

**Return request**
```json
{
  "transactionId": 7,
  "notes": "Returned in good condition"
}
```

---

### System Users — `/api/users` *(Admin only)*

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/users` | List all system users |
| `GET` | `/api/users/{id}` | Get user by ID |
| `POST` | `/api/users` | Create a new system user |
| `PUT` | `/api/users/{id}` | Update a user |
| `DELETE` | `/api/users/{id}` | Delete a user |

**Create user request**
```json
{
  "userName": "jane.librarian",
  "email": "jane@library.com",
  "password": "SecurePass@123",
  "roleId": 2
}
```

---

### Error response shape

```json
{
  "statusCode": 404,
  "messageEn": "Book not found"
}
```

| Status | Scenario |
|---|---|
| `400` | Validation failure or bad input |
| `401` | Missing/expired/invalid token or credentials |
| `403` | Valid token, insufficient role |
| `404` | Entity not found |
| `409` | Concurrency conflict (e.g. book already borrowed by a concurrent request) |
| `500` | Unexpected internal error |

---

## Authentication & Authorization

### Flow

```
1. POST /api/auth           → access token (JSON body) + refresh token (HttpOnly cookie)
2. Authorization: Bearer <accessToken> on all protected requests
3. Access token expires     → POST /api/auth/refresh  (cookie sent automatically by browser)
4. New access token + rotated refresh token returned; old token is revoked
```

### Role matrix

| Action | Admin | Librarian | Staff |
|---|:---:|:---:|:---:|
| Read books, authors, categories, publishers, members | ✅ | ✅ | ✅ |
| Create / update books, authors, categories, publishers, members | ✅ | ✅ | ❌ |
| Delete any entity | ✅ | ❌ | ❌ |
| Issue (borrow) a book | ✅ | ✅ | ✅ |
| Process a return | ✅ | ✅ | ✅ |
| View borrowing transaction history | ✅ | ✅ | ❌ |
| Manage system users | ✅ | ❌ | ❌ |

---

## Security Design

### Refresh token architecture

Tokens use a **split token** pattern for defense in depth:

1. A random `tokenId` (GUID) and `tokenValue` (64 random bytes, base64) are generated separately
2. Only the `tokenValue` is BCrypt-hashed and stored — `tokenId` is stored in plaintext as a lookup key
3. The combined string delivered to the client is `"{tokenId}.{tokenValue}"`
4. On refresh: `tokenId` locates the DB record in O(1); BCrypt verifies `tokenValue` against the stored hash
5. If the record already has `RevokedAt` set when presented → `RefreshTokenReuseDetectedException` (potential token theft indicator)

### Key `appsettings.json` settings

```json
{
  "ConnectionStrings": {
    "Default": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CODE81_Library;Integrated Security=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "Look into appsettings.json",
    "Issuer": "CODE81",
    "Audience": "CODE81Users",
    "AccessTokenMinutes": 60,
    "RefreshTokenDays": 30
  }
}
```

> **Important:** Replace `Jwt.Key` before any non-local deployment. Never commit secrets to source control.

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server or SQL Server LocalDB (included with Visual Studio)

### Option A — EF Core migrations (recommended)

```bash
# 1. Clone the repo
git clone https://github.com/kimovx/CODE81-Assessment.git
cd CODE81-Assessment

# 2. Update the connection string in appsettings.json

# 3. Apply migrations — creates the database and all tables
dotnet ef database update

# 4. Run the API
dotnet run
```

### Option B — SQL script

```sql
-- Run Docs/SQL_Script__Schema___Data_.sql against a SQL Server instance
-- This creates all tables, constraints, indexes, and seeds sample data
```

### Access Swagger UI

```
https://localhost:7222/swagger
```

1. Call `POST /api/auth` with the admin credentials below
2. Copy the `accessToken`
3. Click **Authorize** → enter `Bearer <accessToken>`

---

## Default Credentials

The application seeds these accounts automatically on first startup:

| Username | Password | Role |
|---|---|---|
| `admin` | `Admin@123456` | Administrator |

> Change this password before deploying to any environment beyond local development.

### Sample data (loaded via SQL script)

| Type | Entries |
|---|---|
| Publishers | O'Reilly, No Starch Press |
| Authors | Robert C. Martin, Martin Fowler, Eric Evans, Jon Skeet, Andrew Hunt |
| Categories | Programming → Software Engineering → Design Patterns (3-level hierarchy) |
| Books | Clean Code, Refactoring, Domain-Driven Design, C# in Depth, The Pragmatic Programmer |
| Members | Ahmed Ali (Cairo), Sara Mohamed (Giza) |
| Transactions | 1 active borrow, 2 completed returns |

---

## Project Structure

```
CODE81 Assessment/
│
├── Domain/
│   ├── Entities/           # AppUser, Book, Author, Category, Publisher,
│   │                       # LibraryMember, BorrowingTransaction,
│   │                       # RefreshToken, UserLoginLog, AppRole
│   └── Enums/              # BookStatus, MemberStatus, TransactionStatus
│
├── Application/
│   ├── Common/             # PaginatedResult<T>
│   ├── DTOs/               # Per-entity Create/Update/Response DTOs
│   ├── Exceptions/         # Typed HTTP exceptions (404, 409, 401, 400, 500)
│   ├── Interfaces/
│   │   ├── Repositories/   # IBookRepository, IAuthorRepository, etc.
│   │   └── Services/       # IBookService, IAuthService, IJwtService, etc.
│   └── Services/           # Business logic implementations
│
├── Infrastructure/
│   ├── AppDbContext.cs     # EF Core DbContext + Fluent API configuration
│   ├── DbSeeder.cs         # Seeds roles and default admin user on startup
│   ├── DependencyInjection.cs  # Service and repository registration
│   ├── Repositories/       # EF Core repository implementations
│   ├── Services/
│   │   ├── JwtService.cs         # Token generation (access + refresh)
│   │   └── FileStorageService.cs # Cover image upload to wwwroot/books/
│   └── UnitOfWork.cs       # Transaction coordination
│
├── Presentaion/
│   ├── Controllers/        # Auth, Books, Authors, Categories, Publishers,
│   │                       # LibraryMembers, Borrowing, Users
│   └── Middlewares/
│       ├── GlobalExceptionMiddleware.cs   # Centralised error → JSON response
│       └── UserActivityMiddleware.cs      # Per-request activity logging
│
├── Migrations/             # EF Core migration history
├── Program.cs              # App bootstrap, JWT config, middleware pipeline
├── appsettings.json        # Connection string, JWT settings
│
└── Docs/
    ├── ERD.png                              # Entity Relationship Diagram
    ├── SQL_Script__Schema___Data_.sql       # Full schema + sample data
    └── CODE81_Assessment_postman_collection.json  # Postman API collection
```

---

## Design Decisions

### Why Clean Architecture?

Clean Architecture enforces a dependency rule that keeps business logic (Application/Domain) free of infrastructure concerns. Services are independently testable and the ORM, database, or file storage can be swapped without touching business rules.

### Why Repository + Unit of Work?

Repositories abstract EF Core from the service layer — services depend on `IBookRepository`, not `AppDbContext`. Unit of Work groups multiple repository operations into a single `SaveChangesAsync` call, ensuring atomicity. The borrowing flow uses an explicit `BeginTransactionAsync / CommitAsync / RollbackAsync` cycle for safe, race-condition-resilient operations.

### Why BCrypt for refresh tokens?

Refresh tokens are long-lived credentials. BCrypt hashing at rest means a database breach doesn't expose valid tokens. The split `{tokenId}.{tokenValue}` format allows O(1) DB lookup by `tokenId` without needing to hash-compare every stored token row on each request.

### Why a `[Timestamp]` / RowVersion on Book?

The `RowVersion` column on `Book` enables EF Core optimistic concurrency. If two staff members attempt to borrow the same book simultaneously, `DbUpdateConcurrencyException` is caught and the API returns `409 Conflict` instead of silently allowing a double-booking.

### Why `DeleteBehavior.Restrict` globally?

Set globally in `OnModelCreating` to prevent accidental cascade deletes (e.g., deleting a publisher silently removing all its books). Every delete must be explicit and intentional. Related data must be reassigned or deleted separately first.

### Refresh token in cookie vs. response body

The refresh token is sent as an `HttpOnly; Secure; SameSite=None` cookie instead of the JSON body. This prevents JavaScript access, mitigating XSS-based token theft. The access token is short-lived (60 min default) and kept only in application memory on the client.

### Cover image storage

Cover images are saved to `wwwroot/books/` with a GUID filename to avoid collisions. The relative URL (e.g. `/books/<guid>.jpg`) is stored on the `Book` entity and served as a static file. For production, this should be replaced with Azure Blob Storage or S3.

---

## Documentation

All supporting documentation lives in the `Docs/` folder:

| File | Description |
|---|---|
| [`Docs/ERD.png`](Docs/ERD.png) | Full Entity Relationship Diagram showing all tables, columns, and FK relationships |
| [`Docs/SQL_Script__Schema___Data_.sql`](Docs/SQL_Script__Schema___Data_.sql) | Complete schema creation script + sample data for all tables |
| [`Docs/CODE81_Assessment_postman_collection.json`](Docs/CODE81_Assessment_postman_collection.json) | Postman collection with 30 requests across all 8 resource groups, ready to import |

### Importing the Postman collection

1. Open Postman → **Import**
2. Select `Docs/CODE81_Assessment_postman_collection.json`
3. Set the `baseUrl` environment variable to `https://localhost:7222`
4. Run **Login** first to get your access token, then use it in the `Authorization` header for subsequent requests

---

