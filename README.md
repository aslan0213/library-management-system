# 📚 Library Management System

A RESTful API for managing a library's books, authors, members, and loans — built with **ASP.NET Core (.NET 10)** following **Clean Architecture** principles.

## Architecture

The solution is organized into layers with strict dependency rules — inner layers never depend on outer layers.

```
┌──────────────────────────────────────────────┐
│                    API                       │
│           (LibraryManagement)                │
├──────────────────────────────────────────────┤
│  Services          │  Persistence            │
│  (Use Cases)       │  (Infrastructure)       │
├──────────────────────────────────────────────┤
│              Abstractions                    │
│        (Interfaces & Contracts)              │
├──────────────────────────────────────────────┤
│                 Domain                       │
│          (Entities & Exceptions)             │
└──────────────────────────────────────────────┘
```

| Project | Layer | Responsibility |
|---|---|---|
| **Domain** | Core | Entities (`Author`, `Book`, `Member`, `Loan`, `User`, `RefreshToken`, `Publisher`, `Category`, `Reservation`, `Notification`), `IHasId` interface, and custom exceptions |
| **Abstractions** | Core | Repository interfaces, service interfaces, specification contracts, paging contracts |
| **Services** | Application | Business logic, auth services, reservation subsystem, specification implementations, AutoMapper profiles, FluentValidation validators, background jobs |
| **Persistence** | Infrastructure | EF Core DbContext, entity configurations, repository & Unit of Work implementations, specification evaluator |
| **LibraryManagement** | Presentation | ASP.NET Core Web API controllers, global exception handler, Swagger |
| **Shared** | Cross-cutting | DTOs (request/response models) and paging models |
| **LoggingService** | Cross-cutting | Logging configuration (references Abstractions) |

## Tech Stack

- **.NET 10** — target framework
- **ASP.NET Core** — Web API
- **Entity Framework Core** — ORM
- **PostgreSQL 16** — database (via Npgsql)
- **AutoMapper** — object mapping
- **FluentValidation** — request validation
- **Swagger / Swashbuckle** — API documentation
- **JWT Bearer Authentication** — stateless auth via `Microsoft.AspNetCore.Authentication.JwtBearer`
- **BCrypt.Net-Next** — password hashing
- **Docker Compose** — database containerization
- **xUnit + Moq** — unit & integration testing

## Domain Model

```
Author ──────< Book >──────< Loan >────── Member
  │              │                          │
  Id             Id                         Id
  FirstName      Title                      FirstName
  LastName       Isbn                       LastName
  Bio?           PublishedYear              Email
  DateOfBirth?   TotalCopies                PhoneNumber?
                 AvailableCopies            MembershipDate
                 AuthorId (FK)                  │
                 PublisherId (FK)               │
                     │                          ├──────< Reservation
                     │                          │          Id
               Publisher                        │          BookId (FK)
                 Id                             │          MemberId (FK)
                 Name                           │          ReservedAt
                 Country?                       │          Status (enum)
                     │                          │          HeldUntil?
                     │                          │
               Book >──< Category              └──────< Notification
              (BookCategories)                            Id
                        Id                                MemberId (FK)
                        Name                              Message
                                                          SentAt
                                                          IsRead

         Loan
           Id
           BookId (FK)
           MemberId (FK)
           BorrowedAt
           DueAt
           ReturnedAt?
```

**Relationships summary:**

| Relationship | Type | Delete Behavior |
|---|---|---|
| Author → Book | One-to-Many | Restrict |
| Publisher → Book | One-to-Many | Restrict |
| Book → Loan | One-to-Many | Restrict |
| Member → Loan | One-to-Many | Restrict |
| Book → Reservation | One-to-Many | Restrict |
| Member → Reservation | One-to-Many | Restrict |
| Member → Notification | One-to-Many | Cascade |
| Book ↔ Category | Many-to-Many | via `BookCategories` join table (no explicit entity) |
| User → RefreshToken | One-to-Many | Cascade |

## Advanced Relationships & Search

### Publisher & Category relationships

In Week 1 the `Book` entity had a plain `Publisher` string property. Week 3 replaced this with a proper `Publisher` entity (`Id`, `Name`, `Country?`) linked via `PublisherId` FK — so publisher data is normalized and we can filter/sort books by publisher properly.

Similarly, a `Category` entity (`Id`, `Name`, unique index on name) was introduced with a **many-to-many** relationship to `Book`. EF Core handles this via `.HasMany().WithMany().UsingEntity(j => j.ToTable("BookCategories"))` — so there's no explicit `BookGenre` or `BookCategory` join entity in the domain layer. Navigation properties exist on both sides (`Book.Categories`, `Category.Books`).

### Dynamic book search (Specification pattern)

For basic paged/sorted listing, every entity uses `IRepositoryBase<T>.GetPagedAsync()` with `PagedQuery` (page, size, sort field, direction). That's enough for Author, Member, Loan etc.

For **Book search** specifically, we needed composable multi-field filtering. Instead of piling optional parameters into the repository, I built a hand-rolled **Specification pattern**:

- `ISpecification<T>` (in Abstractions) — defines `Criteria` (expression), `Includes` (eager loading), `OrderBy`/`OrderByDescending`
- `SpecificationBase<T>` (in Abstractions) — base class with helper methods (`AddInclude`, `AddOrderBy`, etc.)
- `BookSearchSpecification` (in Services) — concrete spec that builds a dynamic `WHERE` clause from optional filters
- `SpecificationEvaluator` (in Persistence) — applies a spec to an `IQueryable<T>`, translating it into a single SQL query

**`GET /api/Book/search`** accepts these query parameters (all optional):

| Parameter | Type | Filter |
|---|---|---|
| `title` | `string` | Title contains (case-sensitive per DB collation) |
| `authorId` | `Guid` | Exact author match |
| `publisherId` | `Guid` | Exact publisher match |
| `categoryId` | `Guid` | Book belongs to category (M:N, uses `.Any()`) |
| `minYear` | `int` | Published year ≥ |
| `maxYear` | `int` | Published year ≤ |
| `onlyAvailable` | `bool` | `AvailableCopies > 0` |
| `pageNumber` | `int` | Default 1 |
| `pageSize` | `int` | Default 10 |

Omitted parameters are ignored — the spec only adds predicates for non-null values.

## Reservations & Holds

This is a genuinely new subsystem I built on top of the CRUD layer — it wasn't explicitly required by the assignment, but I wanted a realistic scenario to demonstrate multi-table transactions and rollback properly.

### Lifecycle

1. **Reserve** — A member calls `POST /api/Reservation` with a `bookId` and `memberId`. This is only allowed when `AvailableCopies == 0` (if copies are available, the API tells you to borrow directly). A `Notification` is also created to confirm the reservation.

2. **Fulfill** — When a borrowed copy is returned (`POST /api/Loan/{id}/return`), the system checks for pending reservations on that book. The **oldest** pending reservation (FIFO by `ReservedAt`) gets marked `Fulfilled`, and the copy is held for that member for **48 hours** (`HeldUntil`). The held copy is *not* counted in `AvailableCopies` — nobody else can borrow it.

3. **Borrow the held copy** — The member calls `POST /api/Loan` within the 48-hour window. The reservation is marked `Completed` and the loan is created normally.

4. **Expiration** — If the member doesn't pick up within 48 hours, a **background service** (`ReservationExpirationBackgroundService`, polling every 1 minute) marks the reservation `Expired` and either fulfills the next pending reservation in queue, or releases the copy back into `AvailableCopies`.

5. **Cancel** — `POST /api/Reservation/{id}/cancel` cancels a pending or fulfilled reservation. If it was fulfilled (copy was being held), the same fulfill-next-or-release logic runs.

### Constraints

- A member cannot have more than one active (`Pending` or `Fulfilled`) reservation for the same book.
- `ReservationStatus` is stored as a string in the DB (enum → string conversion via EF Core).

## Transactions & Rollback

### Explicit transactions

`ReservationService.CreateReservationAsync` wraps two logically separate writes in an explicit database transaction:

1. Insert a `Reservation` row → `SaveChangesAsync()`
2. Insert a `Notification` row → `SaveChangesAsync()`
3. If both succeed → `CommitTransactionAsync()`
4. If the second write fails → `RollbackTransactionAsync()` — neither the reservation nor the notification persists

This uses `IUnitOfWork.BeginTransactionAsync()` / `CommitTransactionAsync()` / `RollbackTransactionAsync()`, which wrap EF Core's `IDbContextTransaction`.

`ReservationExpirationService.ProcessExpiredReservationsAsync` also uses explicit transactions — each expired reservation is processed in its own transaction (expire reservation + fulfill next or release copy).

### Implicit transactions

Other multi-write operations like `LoanService.CreateLoanAsync` (which updates `Book.AvailableCopies` and inserts a `Loan`) rely on EF Core's built-in per-`SaveChangesAsync()` transaction — since everything is flushed in one call, it's already atomic without explicit `Begin`/`Commit`.

### Test coverage

`UnitTests/ReservationServiceRollbackTests.cs` covers this:
- **RollsBack** test: mocks `SaveChangesAsync` to succeed on the first call and throw on the second (simulating a notification write failure). Asserts `BeginTransactionAsync` called once, `RollbackTransactionAsync` called once, `CommitTransactionAsync` never called.
- **Commits** test: both writes succeed. Asserts `CommitTransactionAsync` called once, `RollbackTransactionAsync` never called.

## N+1 Prevention

The app **disables EF Core lazy loading entirely** — no `virtual` navigation properties, no lazy-loading proxies. Instead, each repository overrides a `IncludeRelated()` method to declare exactly which navigations to eager-load:

- `BookRepository` → `.Include(b => b.Author).Include(b => b.Publisher).Include(b => b.Categories)` (3-way join in one SQL query)
- `LoanRepository` → `.Include(l => l.Book).ThenInclude(b => b.Author).Include(l => l.Member)` (nested join)
- `ReservationRepository` → `.Include(r => r.Book).Include(r => r.Member)`

Both `GetByIdAsync` and `GetPagedAsync` in `RepositoryBase<T>` call `IncludeRelated()` — so every read query loads its related data in a single SQL round-trip instead of N+1.

The `BookSearchSpecification` also adds its own includes (`Author`, `Publisher`, `Categories`) which the `SpecificationEvaluator` applies via `.Include()`.

All read queries additionally use `.AsNoTracking()` to skip change-tracking overhead.

## Authentication & Authorization

The API uses **JWT Bearer authentication** with **role-based access control** (`User` / `Admin`).

- **Access tokens** are short-lived (15 minutes) and carry the user's `Id`, `Email`, and `Role` as claims.
- **Refresh tokens** are long-lived (7 days), stored server-side as a SHA-256 hash (never in plaintext), and rotated on every use — the previous refresh token is revoked when a new one is issued.
- Passwords are hashed with **BCrypt** before storage; plaintext passwords are never persisted.

### Role mapping

| Resource | `GET` (read) | `POST` / `PUT` / `DELETE` (write) |
|---|---|---|
| Authors, Books, Members | Any authenticated user | `Admin` only |
| Publishers, Categories | Any authenticated user | `Admin` only |
| Loans (create / return) | Any authenticated user | Any authenticated user |
| Reservations (create) | Any authenticated user | `Admin` only |
| Reservations (cancel) | Any authenticated user | Any authenticated user |
| Notifications (read / mark-read) | Any authenticated user | Any authenticated user |

A request with no token returns `401 Unauthorized`. A request with a valid token but an insufficient role returns `403 Forbidden`.

### Bootstrapping the first Admin

New registrations always default to the `User` role — there is no way to self-register as `Admin` (by design, to prevent privilege escalation). To create the first Admin:

1. Register a normal user via `POST /api/auth/register`.
2. Promote that user directly in the database:
```sql
UPDATE "Users" SET "Role" = 'Admin' WHERE "Email" = 'your-email@example.com';
```
3. Log in again via `POST /api/auth/login` to receive a new token reflecting the `Admin` role (existing tokens keep whatever role they were issued with until reissued).

Once at least one `Admin` exists, further promotions can go through `PUT /api/auth/users/{userId}/promote` (`Admin`-only).

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (for PostgreSQL)

### 1. Clone the repository

```bash
git clone https://github.com/aslan0213/library-management-system.git
cd library-management-system
```

### 2. Configure environment variables

Copy the example file and fill in your values:

```bash
cp .env.example .env
```

```env
POSTGRES_USER=your_user
POSTGRES_PASSWORD=your_password
POSTGRES_DB=your_database
```

### 3. Configure JWT secrets

The JWT signing key is kept out of source control via .NET user-secrets, not `appsettings.json`.

```bash
cd LibraryManagement
dotnet user-secrets init
dotnet user-secrets set "Jwt:Secret" "<a base64-encoded random key, at least 32 bytes>"
dotnet user-secrets set "Jwt:Issuer" "LibraryManagementApi"
dotnet user-secrets set "Jwt:Audience" "LibraryManagementClient"
cd ..
```

Generate a random key (PowerShell):
```powershell
[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Maximum 256 }))
```

### 4. Start the database

```bash
docker compose up -d
```

This starts a **PostgreSQL 16** container on port `5432`.

### 5. Apply migrations

```bash
dotnet ef database update --project Persistence --startup-project LibraryManagement
```

### 6. Run the API

```bash
dotnet run --project LibraryManagement
```

Swagger UI will be available at `https://localhost:{port}/swagger` in Development mode.

## API Endpoints

### Auth — `/api/auth`

| Method | Endpoint | Auth Required | Description |
|--------|----------|---------------|-------------|
| `POST` | `/api/auth/register` | None | Register a new user (defaults to `User` role) |
| `POST` | `/api/auth/login` | None | Authenticate, returns access + refresh token |
| `POST` | `/api/auth/refresh` | None | Exchange a valid refresh token for a new token pair |
| `POST` | `/api/auth/logout` | Any authenticated user | Revoke a refresh token |
| `PUT` | `/api/auth/users/{userId}/promote` | `Admin` | Promote a user to the `Admin` role |

### Authors — `/api/Author`

| Method | Endpoint | Auth Required | Description |
|--------|----------|---------------|-------------|
| `GET` | `/api/Author` | Any authenticated user | Get paginated list of authors |
| `GET` | `/api/Author/{id}` | Any authenticated user | Get author by ID |
| `POST` | `/api/Author` | `Admin` | Create a new author |
| `PUT` | `/api/Author/{id}` | `Admin` | Update an author |
| `DELETE` | `/api/Author/{id}` | `Admin` | Delete an author |

### Books — `/api/Book`

| Method | Endpoint | Auth Required | Description |
|--------|----------|---------------|-------------|
| `GET` | `/api/Book` | Any authenticated user | Get paginated list of books |
| `GET` | `/api/Book/{id}` | Any authenticated user | Get book by ID |
| `GET` | `/api/Book/search` | None (open) | Dynamic filtering by title, author, publisher, category, year range, availability |
| `POST` | `/api/Book` | `Admin` | Create a new book |
| `PUT` | `/api/Book/{id}` | `Admin` | Update a book |
| `DELETE` | `/api/Book/{id}` | `Admin` | Delete a book (fails if copies are on loan) |

### Members — `/api/Member`

| Method | Endpoint | Auth Required | Description |
|--------|----------|---------------|-------------|
| `GET` | `/api/Member` | Any authenticated user | Get paginated list of members |
| `GET` | `/api/Member/{id}` | Any authenticated user | Get member by ID |
| `POST` | `/api/Member` | `Admin` | Create a new member |
| `PUT` | `/api/Member/{id}` | `Admin` | Update a member |
| `DELETE` | `/api/Member/{id}` | `Admin` | Delete a member |

### Loans — `/api/Loan`

| Method | Endpoint | Auth Required | Description |
|--------|----------|---------------|-------------|
| `GET` | `/api/Loan` | Any authenticated user | Get paginated list of loans |
| `GET` | `/api/Loan/{id}` | Any authenticated user | Get loan by ID |
| `POST` | `/api/Loan` | Any authenticated user | Create a new loan (decrements available copies) |
| `POST` | `/api/Loan/{id}/return` | Any authenticated user | Return a loan (increments available copies) |

### Publishers — `/api/Publisher`

| Method | Endpoint | Auth Required | Description |
|--------|----------|---------------|-------------|
| `GET` | `/api/Publisher` | Any authenticated user | Get paginated list of publishers |
| `GET` | `/api/Publisher/{id}` | Any authenticated user | Get publisher by ID |
| `POST` | `/api/Publisher` | `Admin` | Create a new publisher |
| `PUT` | `/api/Publisher/{id}` | `Admin` | Update a publisher |
| `DELETE` | `/api/Publisher/{id}` | `Admin` | Delete a publisher |

### Categories — `/api/Category`

| Method | Endpoint | Auth Required | Description |
|--------|----------|---------------|-------------|
| `GET` | `/api/Category` | Any authenticated user | Get paginated list of categories |
| `GET` | `/api/Category/{id}` | Any authenticated user | Get category by ID |
| `POST` | `/api/Category` | `Admin` | Create a new category |
| `PUT` | `/api/Category/{id}` | `Admin` | Update a category |
| `DELETE` | `/api/Category/{id}` | `Admin` | Delete a category |

### Reservations — `/api/Reservation`

| Method | Endpoint | Auth Required | Description |
|--------|----------|---------------|-------------|
| `GET` | `/api/Reservation` | Any authenticated user | Get paginated list of reservations |
| `GET` | `/api/Reservation/{id}` | Any authenticated user | Get reservation by ID |
| `POST` | `/api/Reservation` | `Admin` | Reserve an unavailable book |
| `POST` | `/api/Reservation/{id}/cancel` | Any authenticated user | Cancel a pending or fulfilled reservation |

### Notifications — `/api/Notification`

| Method | Endpoint | Auth Required | Description |
|--------|----------|---------------|-------------|
| `GET` | `/api/Notification` | Any authenticated user | Get paginated list of notifications |
| `GET` | `/api/Notification/{id}` | Any authenticated user | Get notification by ID |
| `PUT` | `/api/Notification/{id}/read` | Any authenticated user | Mark a notification as read |

## Business Rules

- **Book deletion** is blocked when copies are currently on loan (`AvailableCopies ≠ TotalCopies`).
- **Loan creation** requires available copies (`AvailableCopies > 0`), unless the member has a fulfilled reservation holding a copy for them.
- **Loan creation** validates that both the book and member exist.
- **Loan return** is rejected if the loan has already been returned.
- **Book update** cannot lower `TotalCopies` below the number of copies currently on loan.
- **Reservation** is only allowed when no copies are available — if copies exist, the API tells you to borrow directly.
- **Duplicate reservation** is blocked — a member cannot have more than one active (`Pending`/`Fulfilled`) reservation for the same book.
- **Held copy** is excluded from `AvailableCopies` until the reservation is completed, expired, or cancelled.
- **Eager loading** — `BookRepository`, `LoanRepository`, and `ReservationRepository` eager-load related entities via `.Include()` / `.ThenInclude()` to prevent N+1 queries.

## Error Handling

The API uses a global exception handler that returns [RFC 7807](https://datatracker.ietf.org/doc/html/rfc7807) `ProblemDetails` responses:

| Exception | HTTP Status |
|---|---|
| `NotFoundException` | `404 Not Found` |
| `ValidationException` | `400 Bad Request` (with error details) |
| `BusinessRuleViolationException` | `409 Conflict` |
| `InvalidCredentialsException` | `401 Unauthorized` |
| `InvalidRefreshTokenException` | `401 Unauthorized` |
| `EmailAlreadyRegisteredException` | `409 Conflict` |
| Unhandled exceptions | `500 Internal Server Error` |

## Testing

### Run all tests

```bash
dotnet test
```

### Unit Tests

Located in `UnitTests/` — test service-layer logic using **Moq** to mock repositories.

- `AuthorServiceTests`
- `BookServiceTests`
- `LoanServiceTests`
- `MemberServiceTests`
- `ReservationServiceRollbackTests` — verifies transaction rollback on mid-write failure
- `NPlusOneProofTests` — uses in-memory SQLite with EF Core query logging to assert ≤ 2 SELECT statements for paged book queries with all navigations touched

### Integration Tests

Located in `IntegrationTests/` — scaffolded with `WebApplicationFactory` but no test cases implemented yet. Planned for a future week.

## Project Structure

```
LibraryManagement/
├── Abstractions/              # Interfaces & contracts
│   ├── Paging/                #   PagedQuery, PagedResult, SortDirection
│   ├── Repositories/          #   IRepositoryBase, IUnitOfWork, etc.
│   ├── Services/              #   IAuthorService, IBookService, IReservationService, etc.
│   └── Specifications/        #   ISpecification<T>, SpecificationBase<T>
├── Domain/                    # Core domain
│   ├── Entities/              #   Author, Book, Member, Loan, User, RefreshToken,
│   │                          #   Publisher, Category, Reservation, Notification, IHasId
│   └── Exceptions/            #   NotFoundException, BusinessRuleViolationException,
│                              #   InvalidCredentialsException, InvalidRefreshTokenException,
│                              #   EmailAlreadyRegisteredException
├── Services/                  # Business logic
│   ├── Applications/          #   App-layer services (BookAppService, ReservationAppService, etc.)
│   ├── Auth/                  #   AuthService, JwtTokenGenerator, PasswordHasher
│   ├── BackgroundJobs/        #   ReservationExpirationBackgroundService
│   ├── Mapping/               #   AutoMapper profiles
│   ├── Reservations/          #   ReservationService, ReservationExpirationService
│   ├── Specifications/        #   BookSearchSpecification
│   └── Validation/            #   FluentValidation validators
├── Persistence/               # EF Core infrastructure
│   ├── Configurations/        #   Entity type configurations
│   ├── Repositories/          #   Repository & UnitOfWork implementations
│   └── Specifications/        #   SpecificationEvaluator
├── LibraryManagement/         # ASP.NET Core Web API
│   └── Api/
│       ├── Controllers/       #   Author, Book, Loan, Member, Auth, Publisher,
│       │                      #   Category, Reservation, Notification controllers
│       └── Exceptions/        #   GlobalExceptionHandler
├── Shared/                    # Cross-cutting concerns
│   ├── Dtos/                  #   Request & response DTOs
│   └── Paging/                #   PagedRequest, PagedResult
├── LoggingService/            # Logging configuration
├── UnitTests/                 # xUnit + Moq unit tests
├── IntegrationTests/          # Integration tests (scaffolded, not yet implemented)
├── docker-compose.yml         # PostgreSQL container
├── .env.example               # Environment variable template
└── LibraryManagement.slnx     # Solution file
```
