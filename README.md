# Asset Lending System

Full-stack application for managing internal asset lending — tracking loans and reservations of company equipment (laptops, tools, monitors, etc.). Includes a REST API backend and an Angular frontend.

## Prerequisites

Before you begin, make sure you have the following installed:

| Tool | Version | Download | Verify |
|------|---------|----------|--------|
| **.NET 9 SDK** | 9.0+ | https://dotnet.microsoft.com/download/dotnet/9.0 | `dotnet --version` |
| **SQL Server LocalDB** | Included with Visual Studio, or install separately | https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb | `sqllocaldb info` |
| **Node.js** | 18+ | https://nodejs.org/ | `node -v` |
| **npm** | Comes with Node.js | — | `npm -v` |

> **Tip:** If you have Visual Studio 2022 installed, SQL Server LocalDB is likely already available. If not, install "SQL Server Express LocalDB" from the link above.

## Setup & Run

### 1. Clone the repository

```bash
git clone <repo-url>
cd M2MTask
```

### 2. Configure connection string (if needed)

Default in `Server/WebApi/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AssetLendingDb;Trusted_Connection=True;"
}
```
Adjust `Data Source=` if your SQL Server instance name differs.

### 3. Run the API (backend)

```bash
dotnet run --project Server/WebApi
```

The API will:
- Automatically create and migrate the database on first run
- Seed sample data (3 users, 3 asset categories, 5 assets)
- Start listening on:
  - **HTTP:** `http://localhost:5057`
  - **HTTPS:** `https://localhost:7197`

> **Keep this terminal open** — the API must be running for the frontend to work.

### 4. Run the Angular frontend (in a new terminal)

```bash
cd Client
npm install
npm start
```

Open **http://localhost:4200** in the browser.

> First run of `npm install` may take a few minutes. Alternatively, you can use `ng serve` if you have Angular CLI installed globally (`npm install -g @angular/cli`). Both commands do the same thing.

### 5. Open Swagger UI (API documentation)

Navigate to **https://localhost:7197/swagger** in the browser while the API is running.

### 6. Run tests

```bash
dotnet test
```

## Troubleshooting

### SQL Server LocalDB is not installed
If `sqllocaldb info` returns an error, install LocalDB:
- **With Visual Studio Installer:** Modify your installation → Individual Components → search "LocalDB" → check "SQL Server Express LocalDB"
- **Standalone:** Download from https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb

### HTTPS certificate not trusted
If the browser shows a certificate warning when accessing `https://localhost:7197`:
```bash
dotnet dev-certs https --trust
```

### Frontend cannot reach the API (CORS error)
Make sure the API is running on port 5057/7197 before starting the frontend. The API is configured to accept requests from `http://localhost:4200`.

### Database migration errors
If you see migration errors, delete the database and let it recreate:
```bash
sqllocaldb stop MSSQLLocalDB
sqllocaldb delete MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
dotnet run --project Server/WebApi
```

---

## Architecture

The project follows **Clean Architecture** with **DDD** principles:

```
Server/
├── Domain          Pure domain model — entities, enums, business rules (no dependencies)
├── DTOs            Data Transfer Objects shared across layers
├── Application     Application services, mappers (refs: Domain, DTOs, Infrastructure)
├── Infrastructure  EF Core DbContext, repositories, Unit of Work, seeds (refs: Domain, DTOs)
├── WebApi          ASP.NET Core Web API — controllers, DI, Swagger (refs: Application, Infrastructure, DTOs)
└── WebApi.Tests    Unit tests (xUnit + Moq)
Client/             Angular 20 SPA (standalone components, routing)
```

**Key patterns:**
- **Repository pattern** — generic base + specialized repositories
- **Unit of Work** — wraps `DbContext.SaveChangesAsync()` for atomic operations
- **Result pattern** — explicit success/failure returns instead of exceptions for business rule violations
- **DDD domain methods** — entities guard their own invariants (e.g., `Asset.Checkout()` returns failure if not available)
- **Manual mappers** — extension methods for entity <-> DTO conversion
- **Optimistic concurrency** — RowVersion on Asset entity prevents simultaneous conflicting operations
- **Global exception handler** — middleware catches unhandled exceptions and returns standardized error responses

## Domain Model

| Entity | Description |
|--------|-------------|
| **User** | Person who borrows/reserves assets (FirstName, LastName, Email, FullName) |
| **AssetCategory** | Category of assets (Name, Description) — e.g. Electronics, Tools, Office Equipment |
| **Asset** | Equipment being lent (Name, Description, SerialNumber, Status, IsActive, Category, RowVersion) |
| **Loan** | Borrowing record (BorrowedBy -> User, BorrowedAt, DueDate, ReturnedAt, Status) |
| **Reservation** | Reserved asset for a user (ReservedBy -> User, ReservedUntil, IsCancelled, IsExpired) |

## Business Rules

- An asset can be **Available**, **Loaned**, or **Reserved** at any time
- Only **active** assets (IsActive = true) can be loaned or reserved
- A **loan** tracks who borrowed which asset, when, and until when (DueDate)
- Upon **return**, the asset status changes back to Available
- A **reservation** holds an asset for a specific user until a given date
- If a reservation **expires** (ReservedUntil < now), it is auto-cancelled on the next loan/reservation attempt
- A user can **checkout from their own reservation** — the reservation is auto-cancelled and a loan is created
- **Overdue loans** (active loans past DueDate) can be queried via a dedicated endpoint
- **Soft delete** — assets can be deactivated/reactivated instead of hard-deleted
- **Pagination** — list endpoints support page/pageSize query parameters (default: page=1, pageSize=10, max: 100)

## API Endpoints

### Users
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/users?page=1&pageSize=10` | List users (paginated) |
| GET | `/api/users/{id}` | Get user by ID |
| POST | `/api/users` | Create a new user |

### Asset Categories
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/assetcategories` | List all categories |
| GET | `/api/assetcategories/{id}` | Get category by ID |
| POST | `/api/assetcategories` | Create a new category |

### Assets
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/assets?status=Available&categoryId=1&page=1&pageSize=10` | List assets (paginated, optional filters) |
| GET | `/api/assets/{id}` | Get asset by ID |
| GET | `/api/assets/{id}/loans?page=1&pageSize=10` | Get loan history for an asset |
| POST | `/api/assets` | Create a new asset |
| PUT | `/api/assets/{id}` | Update an asset |
| PATCH | `/api/assets/{id}/deactivate` | Deactivate an asset (soft delete) |
| PATCH | `/api/assets/{id}/activate` | Reactivate an asset |

### Loans
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/loans?page=1&pageSize=10` | List all loans (paginated) |
| GET | `/api/loans/active` | List all active loans |
| GET | `/api/loans/overdue` | List overdue loans (active, past due date) |
| POST | `/api/loans` | Checkout an asset (create loan) |
| PUT | `/api/loans/{id}/return` | Return a loaned asset |

### Reservations
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/reservations` | Reserve an asset |
| PUT | `/api/reservations/{id}/cancel` | Cancel a reservation |

### Statistics
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/statistics` | Aggregated system statistics (counts, most borrowed asset, most active user) |

## Architecture Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| **API style** | Controllers | Better demonstrates layered architecture than Minimal API; each controller maps to an aggregate, making the structure self-documenting. |
| **Validation flow** | Result pattern | Business rule violations are expected outcomes, not exceptional states. Throwing exceptions for validation is expensive and obscures control flow. `Result<T>` makes success/failure explicit without performance overhead. |
| **Mapping** | Manual extension methods | AutoMapper introduces runtime reflection and implicit mapping conventions. With 5 entities, explicit `ToDto()` methods are more transparent, have zero runtime cost, and make it immediately clear what gets mapped. |
| **Concurrency** | RowVersion on Asset | Two users can attempt to checkout the same asset simultaneously. Optimistic concurrency via SQL Server `rowversion` detects conflicts at save time without database-level locks, keeping the system responsive. |
| **Soft delete** | IsActive flag | Hard-deleting assets would break FK integrity on historical loans. Deactivation preserves audit trail while hiding the asset from active operations. |
| **Enum storage** | String conversion | Storing enums as strings (`"Available"`, `"Loaned"`) instead of integers makes the database human-readable and query-debuggable at negligible storage cost. |
| **ID type** | int (auto-increment) | Simpler than GUIDs for a single-database system. No clustered index fragmentation, smaller FK footprint, easier to reference in conversations and debugging. |
| **Notification** | Interface + console impl | Defines the contract (`INotificationService`) now so it can be swapped for SMTP/SendGrid in production without touching business logic. Follows the Dependency Inversion Principle. |

## Frontend

The Angular SPA provides:
- **Dashboard** — system statistics overview (total assets, active loans, overdue, most borrowed asset)
- **Assets** — paginated list with status/category filters, create new asset, asset detail with checkout/reserve actions and loan history
- **Loans** — active/overdue tabs with return functionality
- **Users** — list with inline creation form

## Technologies

**Backend:**
- .NET 9 / ASP.NET Core Web API
- Entity Framework Core 9 (SQL Server)
- Swashbuckle (Swagger/OpenAPI)
- xUnit + Moq (unit tests)

**Frontend:**
- Angular 20 (standalone components)
- TypeScript
- Angular Router
