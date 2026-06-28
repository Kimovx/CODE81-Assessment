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
  - [Auth](#auth--post-apiauth)
  - [Books](#books--apibooks)
  - [Authors](#authors--apiauthors)
  - [Categories](#categories--apicategories)
  - [Publishers](#publishers--apipublishers)
  - [Library Members](#library-members--apilibrarymembers)
  - [Borrowing](#borrowing--apiborrowing)
  - [System Users](#system-users--apiusers-admin-only)
  - [Activity Logs](#activity-logs--apiusersactivity-logs)
  - [Dashboard](#dashboard--apidashboard-admin--librarian)
  - [Error responses](#error-response-shape)
- [Authentication & Authorization](#authentication--authorization)
- [Security Design](#security-design)
- [Getting Started](#getting-started)
- [Default Credentials](#default-credentials)
- [Project Structure](#project-structure)
- [Design Decisions](#design-decisions)
- [Documentation](#documentation)

---

## Overview

A full-featured Library Management System REST API enabling librarians and staff to manage books, members, and borrowing workflows. The system implements role-based access control, JWT authentication with refresh token rotation, and a clean layered architecture following Clean Architecture principles.

Built as part of the **CODE81 Technical Assessment**, with production-grade design considerations.

---

## Architecture

The project follows **Clean Architecture** with four distinct layers that enforce a strict dependency rule — outer layers depend on inner ones, never the reverse.

```
┌─────────────────────────────────────────────────────┐
│                  Presentation Layer                  │
│              Controllers · Middlewares               │
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
- **Dashboard** — KPIs, trends, top lists, overdue alerts, and inventory insights for Admin and Librarian roles

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
  ├── UserActivityLogs     (activity log — every authenticated request)
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
| `UserActivityLogs` | Per-request activity log — action, IP address, user agent, success/failure |

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

### Activity Logs — `/api/users/activity-logs`

Provides visibility into **all authenticated user actions** across the system.  
Logs are automatically recorded via middleware for every request made by authenticated users.

Each log entry includes:

- HTTP method & endpoint
- Execution result (success / failure)
- IP address
- User agent (browser / client)
- Timestamp (UTC)
- User identity

---

### Endpoints

| Method | Endpoint | Role | Description |
|---|---|---|---|
| `GET` | `/api/users/activity-logs` | Admin only | Get all system activity logs (paginated) |
| `GET` | `/api/users/{id}/activity-logs` | Admin only | Get activity logs for a specific user |

---

### Query Parameters (Pagination)

| Param | Type | Default |
|---|---|---|
| `pageNumber` | int | 1 |
| `pageSize` | int | 10 |

---

### Example Response

```json
[
  {
    "id": 1,
    "logTime": "2026-06-28T04:38:19.6638478+00:00",
    "isSuccess": true,
    "ipAddress": "::1",
    "userAgent": "PostmanRuntime/7.54.0",
    "action": "GET /api/users/paginated",
    "userId": 1,
    "userName": "admin"
  }
]
```

### Notes

- Logs are generated automatically using `UserActivityMiddleware`
- Only authenticated users are tracked
- Requests are marked as successful if `statusCode < 400`
- Supports reverse proxy environments via `X-Forwarded-For`

---

### Dashboard — `/api/dashboard` *(Admin & Librarian)*

A set of read-only analytical endpoints that power management dashboards. All endpoints require the `Admin` or `Librarian` role.

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/dashboard/kpis` | All high-level KPI counts in a single call |
| `GET` | `/api/dashboard/monthly-activity` | Month-by-month borrow / return / overdue counts |
| `GET` | `/api/dashboard/top-books` | Most-borrowed books ranked by borrow count |
| `GET` | `/api/dashboard/top-members` | Most-active members ranked by borrow count |
| `GET` | `/api/dashboard/overdue` | All currently overdue transactions, ordered by days overdue |
| `GET` | `/api/dashboard/books-by-category` | Book count per category, ordered by count |
| `GET` | `/api/dashboard/inventory` | Availability rate and borrow rate as percentages |
| `GET` | `/api/dashboard/recent-transactions` | Most recent borrowing transactions |

#### KPIs — `GET /api/dashboard/kpis`

No query parameters. Returns a single object with counts across all entities.

```json
{
  "totalBooks": 120,
  "availableBooks": 95,
  "borrowedBooks": 25,
  "totalMembers": 340,
  "activeMembers": 310,
  "suspendedMembers": 20,
  "expiredMembers": 10,
  "totalTransactions": 870,
  "activeTransactions": 25,
  "returnedTransactions": 830,
  "overdueTransactions": 12,
  "lostTransactions": 3,
  "totalAuthors": 48,
  "totalCategories": 15,
  "totalPublishers": 9
}
```

#### Monthly activity — `GET /api/dashboard/monthly-activity`

| Query param | Type | Default | Max |
|---|---|---|---|
| `months` | int | `12` | `36` |

```json
[
  { "month": "2026-01", "borrowCount": 42, "returnCount": 38, "overdueCount": 4 },
  { "month": "2026-02", "borrowCount": 55, "returnCount": 50, "overdueCount": 5 }
]
```

#### Top borrowed books — `GET /api/dashboard/top-books`

| Query param | Type | Default | Max |
|---|---|---|---|
| `top` | int | `10` | `50` |

```json
[
  {
    "bookId": 1,
    "title": "Clean Code",
    "isbn": "9780132350884",
    "borrowCount": 34,
    "authors": ["Robert C. Martin"]
  }
]
```

#### Top active members — `GET /api/dashboard/top-members`

| Query param | Type | Default | Max |
|---|---|---|---|
| `top` | int | `10` | `50` |

```json
[
  {
    "memberId": 3,
    "fullName": "Ahmed Ali",
    "email": "ahmed@test.com",
    "borrowCount": 18
  }
]
```

#### Overdue transactions — `GET /api/dashboard/overdue`

No query parameters. Returns all active transactions past their due date.

```json
[
  {
    "transactionId": 7,
    "bookId": 3,
    "bookTitle": "Domain-Driven Design",
    "memberId": 3,
    "memberName": "Ahmed Ali",
    "memberEmail": "ahmed@test.com",
    "borrowDate": "2026-05-01T00:00:00Z",
    "dueDate": "2026-05-15T00:00:00",
    "daysOverdue": 43
  }
]
```

#### Books by category — `GET /api/dashboard/books-by-category`

No query parameters.

```json
[
  { "categoryId": 1, "categoryName": "Programming",        "bookCount": 45 },
  { "categoryId": 3, "categoryName": "Software Engineering","bookCount": 30 }
]
```

#### Inventory status — `GET /api/dashboard/inventory`

No query parameters.

```json
{
  "totalBooks": 120,
  "availableBooks": 95,
  "borrowedBooks": 25,
  "availabilityRate": 79.17,
  "borrowRate": 20.83
}
```

#### Recent transactions — `GET /api/dashboard/recent-transactions`

| Query param | Type | Default | Max |
|---|---|---|---|
| `top` | int | `10` | `100` |

```json
[
  {
    "transactionId": 42,
    "bookTitle": "Clean Code",
    "memberName": "Ahmed Ali",
    "status": "Active",
    "borrowDate": "2026-06-20T10:00:00Z",
    "dueDate": "2026-07-04T10:00:00",
    "returnDate": null,
    "createdBy": "admin"
  }
]
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
| View dashboard & analytics | ✅ | ✅ | ❌ |

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
│   │                       # RefreshToken, UserActivityLog, AppRole
│   └── Enums/              # BookStatus, MemberStatus, TransactionStatus
│
├── Application/
│   ├── Common/             # PaginatedResult<T>
│   ├── DTOs/
│   │   ├── ...             # Per-entity Create/Update/Response DTOs
│   │   └── Dashboard/      # DashboardDto.cs — all dashboard response types
│   ├── Exceptions/         # Typed HTTP exceptions (404, 409, 401, 400, 500)
│   ├── Interfaces/
│   │   ├── Repositories/   # IBookRepository, IAuthorRepository, IDashboardRepository, etc.
│   │   └── Services/       # IBookService, IAuthService, IDashboardService, etc.
│   └── Services/           # Business logic implementations (incl. DashboardService)
│
├── Infrastructure/
│   ├── AppDbContext.cs     # EF Core DbContext + Fluent API configuration
│   ├── DbSeeder.cs         # Seeds roles and default admin user on startup
│   ├── DependencyInjection.cs  # Service and repository registration (incl. Dashboard)
│   ├── Repositories/       # EF Core repository implementations (incl. DashboardRepository)
│   ├── Services/
│   │   ├── JwtService.cs         # Token generation (access + refresh)
│   │   └── FileStorageService.cs # Cover image upload to wwwroot/books/
│   └── UnitOfWork.cs       # Transaction coordination
│
├── Presentaion/
│   ├── Controllers/        # Auth, Books, Authors, Categories, Publishers,
│   │                       # LibraryMembers, Borrowing, Users, Dashboard
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

### Dashboard design — read-only, no caching

All dashboard queries run directly against the database using `AsNoTracking()` for performance. Each endpoint is a focused query (no heavy joins across unrelated tables) so response times stay acceptable at moderate data volumes. For high-traffic production use, a caching layer (e.g. Redis with a short TTL) should be added in front of the dashboard repository.

---

## Documentation

All supporting documentation lives in the `Docs/` folder:

| File | Description |
|---|---|
| [`Docs/ERD.png`](Docs/ERD.png) | Full Entity Relationship Diagram showing all tables, columns, and FK relationships |
| [`Docs/SQL_Script__Schema___Data_.sql`](Docs/SQL_Script__Schema___Data_.sql) | Complete schema creation script + sample data for all tables |
| [`Docs/CODE81_Assessment_postman_collection.json`](Docs/CODE81_Assessment_postman_collection.json) | Postman collection with requests across all resource groups, ready to import |

### Importing the Postman collection

1. Open Postman → **Import**
2. Select `Docs/CODE81_Assessment_postman_collection.json`
3. Update the `baseUrl` collection variable to match your local port (default: `https://localhost:7222`)
4. Run **Login** — the collection automatically saves the returned `accessToken` to a collection variable and attaches it as a `Bearer` token on all subsequent requests. No manual copy-paste needed.
