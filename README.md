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
| **Domain** | Core | Entities (`Author`, `Book`, `Member`, `Loan`, `User`, `RefreshToken`), `IHasId` interface, and custom exceptions |
| **Abstractions** | Core | Repository interfaces, service interfaces, paging contracts |
| **Services** | Application | Business logic, auth services, AutoMapper profiles, FluentValidation validators |
| **Persistence** | Infrastructure | EF Core DbContext, entity configurations, repository & Unit of Work implementations |
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
Author ──────< Book ──────< Loan >────── Member
  │              │                         │
  Id             Id                        Id
  FirstName      Title                     FirstName
  LastName       Isbn                      LastName
  Bio?           PublishedYear              Email
  DateOfBirth?   Publisher                 PhoneNumber?
                 TotalCopies               MembershipDate
                 AvailableCopies
                 AuthorId (FK)
                                    Loan
                                      Id
                                      BookId (FK)
                                      MemberId (FK)
                                      BorrowedAt
                                      DueAt
                                      ReturnedAt?
```

## Authentication & Authorization

The API uses **JWT Bearer authentication** with **role-based access control** (`User` / `Admin`).

- **Access tokens** are short-lived (15 minutes) and carry the user's `Id`, `Email`, and `Role` as claims.
- **Refresh tokens** are long-lived (7 days), stored server-side as a SHA-256 hash (never in plaintext), and rotated on every use — the previous refresh token is revoked when a new one is issued.
- Passwords are hashed with **BCrypt** before storage; plaintext passwords are never persisted.

### Role mapping

| Resource | `GET` (read) | `POST` / `PUT` / `DELETE` (write) |
|---|---|---|
| Authors, Books, Members | Any authenticated user | `Admin` only |
| Loans (create / return) | Any authenticated user | Any authenticated user |

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

## Business Rules

- **Book deletion** is blocked when copies are currently on loan (`AvailableCopies ≠ TotalCopies`).
- **Loan creation** requires available copies (`AvailableCopies > 0`).
- **Loan creation** validates that both the book and member exist.
- **Loan return** is rejected if the loan has already been returned.
- **Book update** cannot lower `TotalCopies` below the number of copies currently on loan.

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

### Integration Tests

Located in `IntegrationTests/` — test API endpoints using `WebApplicationFactory`.

## Project Structure

```
LibraryManagement/
├── Abstractions/              # Interfaces & contracts
│   ├── Paging/                #   PagedQuery, PagedResult, SortDirection
│   ├── Repositories/          #   IRepositoryBase, IUnitOfWork, etc.
│   └── Services/              #   IAuthorService, IBookService, etc.
├── Domain/                    # Core domain
│   ├── Entities/              #   Author, Book, Member, Loan, User, RefreshToken, IHasId
│   └── Exceptions/            #   NotFoundException, BusinessRuleViolationException,
│                              #   InvalidCredentialsException, InvalidRefreshTokenException,
│                              #   EmailAlreadyRegisteredException
├── Services/                  # Business logic
│   ├── Auth/                  #   AuthService, JwtTokenGenerator, PasswordHasher
│   ├── Mapping/               #   AutoMapper profiles
│   └── Validation/            #   FluentValidation validators
├── Persistence/               # EF Core infrastructure
│   ├── Configurations/        #   Entity type configurations
│   └── Repositories/          #   Repository & UnitOfWork implementations
├── LibraryManagement/         # ASP.NET Core Web API
│   └── Api/
│       ├── Controllers/       #   Author, Book, Loan, Member, Auth controllers
│       └── Exceptions/        #   GlobalExceptionHandler
├── Shared/                    # Cross-cutting concerns
│   ├── Dtos/                  #   Request & response DTOs
│   └── Paging/                #   PagedRequest, PagedResult
├── LoggingService/            # Logging configuration
├── UnitTests/                 # xUnit + Moq unit tests
├── IntegrationTests/          # Integration tests
├── docker-compose.yml         # PostgreSQL container
├── .env.example               # Environment variable template
└── LibraryManagement.slnx     # Solution file
```
