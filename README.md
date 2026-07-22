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
| **Domain** | Core | Entities (`Author`, `Book`, `Member`, `Loan`), `IHasId` interface, and custom exceptions |
| **Abstractions** | Core | Repository interfaces, service interfaces, paging contracts |
| **Services** | Application | Business logic, AutoMapper profiles, FluentValidation validators |
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

### 3. Start the database

```bash
docker compose up -d
```

This starts a **PostgreSQL 16** container on port `5432`.

### 4. Apply migrations

```bash
dotnet ef database update --project Persistence --startup-project LibraryManagement
```

### 5. Run the API

```bash
dotnet run --project LibraryManagement
```

Swagger UI will be available at `https://localhost:{port}/swagger` in Development mode.

## API Endpoints

### Authors — `/api/Author`

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/Author` | Get paginated list of authors |
| `GET` | `/api/Author/{id}` | Get author by ID |
| `POST` | `/api/Author` | Create a new author |
| `PUT` | `/api/Author/{id}` | Update an author |
| `DELETE` | `/api/Author/{id}` | Delete an author |

### Books — `/api/Book`

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/Book` | Get paginated list of books |
| `GET` | `/api/Book/{id}` | Get book by ID |
| `POST` | `/api/Book` | Create a new book |
| `PUT` | `/api/Book/{id}` | Update a book |
| `DELETE` | `/api/Book/{id}` | Delete a book (fails if copies are on loan) |

### Members — `/api/Member`

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/Member` | Get paginated list of members |
| `GET` | `/api/Member/{id}` | Get member by ID |
| `POST` | `/api/Member` | Create a new member |
| `PUT` | `/api/Member/{id}` | Update a member |
| `DELETE` | `/api/Member/{id}` | Delete a member |

### Loans — `/api/Loan`

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/Loan` | Get paginated list of loans |
| `GET` | `/api/Loan/{id}` | Get loan by ID |
| `POST` | `/api/Loan` | Create a new loan (decrements available copies) |
| `POST` | `/api/Loan/{id}/return` | Return a loan (increments available copies) |

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
│   ├── Entities/              #   Author, Book, Member, Loan, IHasId
│   └── Exceptions/            #   NotFoundException, BusinessRuleViolationException
├── Services/                  # Business logic
│   ├── Profiles/              #   AutoMapper profiles
│   └── Validators/            #   FluentValidation validators
├── Persistence/               # EF Core infrastructure
│   ├── Configurations/        #   Entity type configurations
│   └── Repositories/          #   Repository & UnitOfWork implementations
├── LibraryManagement/         # ASP.NET Core Web API
│   └── Api/
│       ├── Controllers/       #   Author, Book, Loan, Member controllers
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
