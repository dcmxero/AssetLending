# Asset Lending System

Full-stack application for managing internal asset lending — tracking loans and reservations of company equipment (laptops, tools, monitors, etc.). Includes a REST API backend and two interchangeable frontends — the original Angular SPA and a
standalone React SPA. Both talk to the same API and render the same screens; pick whichever one you
want to run.

## Prerequisites

Before you begin, make sure you have the following installed:

| Tool | Version | Download | Verify |
|------|---------|----------|--------|
| **.NET 10 SDK** | 10.0+ | https://dotnet.microsoft.com/download/dotnet/10.0 | `dotnet --version` |
| **SQL Server LocalDB** | Included with Visual Studio, or install separately | https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb | `sqllocaldb info` |
| **Node.js** | 22.22.3+ or 24.15+ | https://nodejs.org/ | `node -v` |
| **npm** | Comes with Node.js | — | `npm -v` |
| **Visual Studio** *(optional)* | 2026+ — see the IDE note below | https://visualstudio.microsoft.com/ | Help → About |

> **Tip:** If you have Visual Studio 2022 installed, SQL Server LocalDB is likely already available. If not, install "SQL Server Express LocalDB" from the link above.

### A note on IDEs

**No IDE is required, and the CLI is the supported path.** Everything in this README runs on the
`dotnet` CLI and npm: `dotnet build`, `dotnet run` and `dotnet test` cover the whole server, and
the two frontends are plain npm projects. Nothing here depends on Visual Studio.

Opening `AssetLending.sln` is a different matter: **.NET 10 projects require Visual Studio 2026 or
newer.** Visual Studio 2022 caps out at .NET 9 and reports "load failed" for every project under
`Server/` — this is a Visual Studio limitation, not a problem with the solution. If you are on
VS 2022 and would rather not install VS 2026, use the CLI, or open the repository in Rider or
VS Code with the C# Dev Kit; all three build and test the server without the solution file.

## Setup & Run

### 1. Clone the repository

```bash
git clone https://github.com/dcmxero/AssetLending.git
cd AssetLending
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

### 4. Run a frontend (in a new terminal)

Two frontends ship with the project. They are independent applications with the same
functionality — run either one, or both at once, since they use different ports.

**Angular** — http://localhost:4200

```bash
cd ClientAngular
npm install
npm start
```

**React** — http://localhost:4300

```bash
cd ClientReact
npm install
npm start
```

> First run of `npm install` may take a few minutes. For the Angular client you can also use
> `ng serve` if you have the Angular CLI installed globally (`npm install -g @angular/cli`);
> both commands do the same thing.

Both clients call the API at `https://localhost:7197/api` by default. The React client reads
`VITE_API_BASE_URL` if you need to point it somewhere else.

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
├── Application     Services, mappers, persistence and query contracts (refs: Domain, DTOs)
├── Infrastructure  EF Core DbContext, repositories, queries, Unit of Work, seeds (refs: Application, Domain, DTOs)
├── WebApi          ASP.NET Core Web API — controllers, DI, Swagger (refs: Application, Infrastructure, DTOs)
└── WebApi.Tests    Domain, controller, query, integration and architecture tests
ClientAngular/      Angular SPA (standalone components, routing)
ClientReact/        React SPA (Vite, React Router, TanStack Query)
```

The solution file covers the `Server/` projects only. The two frontends have their own npm
toolchains and are not part of the MSBuild build — open them as folders, not through the solution.
See the IDE note under Prerequisites for the Visual Studio version the solution requires.

**Key patterns:**
- **Read/write split** — writes go through repositories that return aggregates, reads go through
  queries that project straight to DTOs (see the Architecture Decisions table for the reasoning)
- **Repository pattern** — one interface per aggregate, holding only the operations that aggregate
  supports; no generic base class
- **Unit of Work** — the application layer's only route to persistence, and the boundary where
  provider-specific failures are translated into `ConcurrencyConflictException`
- **Dependency inversion** — `Application/Abstractions` owns the contracts, `Infrastructure`
  implements them; architecture tests fail the build if the direction is reversed
- **Result pattern** — explicit success/failure returns instead of exceptions for business rule
  violations; each failure carries a `ResultErrorKind` so callers branch on the category rather than
  on the wording of the message
- **DDD domain methods** — entities guard their own invariants (e.g., `Asset.Checkout()` returns failure if not available)
- **Manual mappers** — extension methods used on the write path, where the entity is already in
  memory; the read path projects to DTOs in the query instead
- **Optimistic concurrency** — RowVersion on Asset entity prevents simultaneous conflicting operations
- **Injected clock** — nothing reads `DateTime.UtcNow`; the domain is told the instant, and services
  and queries take it from `TimeProvider`, so overdue and expiry rules can be tested at a chosen time
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
| GET | `/api/loans/{id}` | Get a loan by ID |
| GET | `/api/loans/active` | List all active loans |
| GET | `/api/loans/overdue` | List overdue loans (active, past due date) |
| POST | `/api/loans` | Checkout an asset (create loan) |
| PUT | `/api/loans/{id}/return` | Return a loaned asset |

### Reservations
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/reservations?page=1&pageSize=10` | List all reservations (paginated, includes cancelled) |
| GET | `/api/reservations/active` | List reservations that are neither cancelled nor run out |
| GET | `/api/reservations/{id}` | Get a reservation by ID |
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
| **Error classification** | Kind on the result | A failure used to carry only a message, so the API layer picked between 404 and 409 by testing whether that message contained the words "not found". Rewording a message would then have changed an endpoint's status code with nothing to catch it. `ResultErrorKind` names the three cases the callers care about — rule violation, missing entity, lost race — and the message goes back to being for humans only. |
| **Validation flow** | Result pattern | Business rule violations are expected outcomes, not exceptional states. Throwing exceptions for validation is expensive and obscures control flow. `Result<T>` makes success/failure explicit without performance overhead. |
| **Mapping** | Manual extension methods | AutoMapper introduces runtime reflection and implicit mapping conventions. With 5 entities, explicit `ToDto()` methods are more transparent, have zero runtime cost, and make it immediately clear what gets mapped. |
| **Concurrency** | RowVersion on Asset | Two users can attempt to checkout the same asset simultaneously. Optimistic concurrency via SQL Server `rowversion` detects conflicts at save time without database-level locks, keeping the system responsive. |
| **Soft delete** | IsActive flag | Hard-deleting assets would break FK integrity on historical loans. Deactivation preserves audit trail while hiding the asset from active operations. |
| **Enum storage** | String conversion | Storing enums as strings (`"Available"`, `"Loaned"`) instead of integers makes the database human-readable and query-debuggable at negligible storage cost. |
| **ID type** | int (auto-increment) | Simpler than GUIDs for a single-database system. No clustered index fragmentation, smaller FK footprint, easier to reference in conversations and debugging. |
| **Data access** | Split reads from writes | Entity Framework Core already is a repository and a unit of work, so a second layer that forwards `GetById` and `Add` adds indirection without abstraction. The split keeps the part that pays for itself and drops the part that does not. Writes load whole aggregates, because `Asset.Checkout()` and `Loan.MarkReturned()` need an entity to guard their invariants. Reads never construct an aggregate, so they project to DTOs inside the SQL query and fetch only the columns the response contains. |
| **Query placement** | Same project as the repositories | A separate `Infrastructure.Queries` project would still have to reference the project holding `ApplicationDbContext`, so the boundary would be one-directional and unenforced. Folders plus architecture tests give the same separation without a third project and without complicating `dotnet ef`. Worth revisiting if the read side ever moves to Dapper, a read replica, or a denormalized read model. |
| **Contract placement** | `Application/Abstractions` | Interfaces that live in the infrastructure project make the application layer depend on infrastructure, which is the dependency the repository pattern exists to remove. With the contracts in the application layer, `Application` compiles without EF Core at all. |
| **Time** | Injected `TimeProvider` | Overdue loans and expired reservations are the rules most worth testing and were the hardest, because the code read the system clock at the point of decision. A test could only choose dates relative to the real clock and hope it did not run near a boundary. With the instant supplied, expiry is tested by moving a `FakeTimeProvider` rather than by rewriting data. It also makes each operation use one reading, so a request spanning midnight cannot stamp one date and be judged against another. |
| **Notification** | Interface + console impl | Defines the contract (`INotificationService`) now so it can be swapped for SMTP/SendGrid in production without touching business logic. Follows the Dependency Inversion Principle. |

## Frontend

The Angular SPA provides:
- **Dashboard** — system statistics overview (total assets, active loans, overdue, most borrowed asset)
- **Assets** — paginated list with status/category filters, create new asset, asset detail with checkout/reserve actions and loan history
- **Loans** — active/overdue tabs with return functionality
- **Users** — list with inline creation form

## Technologies

**Backend:**
- .NET 10 / ASP.NET Core Web API
- Entity Framework Core 10 (SQL Server)
- Swashbuckle (Swagger/OpenAPI)
- xUnit + Moq (unit tests)
- NetArchTest (layer boundary tests)
- Microsoft.Extensions.TimeProvider.Testing (controllable clock in tests)

**Frontend (Angular):**
- Angular (standalone components)
- TypeScript
- Angular Router

**Frontend (React):**
- React 19
- TypeScript
- Vite
- React Router
- TanStack Query
