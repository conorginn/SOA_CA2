# SOA_CA2 - Library API (ASP.NET Core, REST)

## Overview
This project is a Service Oriented Architecture (SOA) style REST API built with **ASP.NET Core (.NET 8)**.  
It exposes CRUD services for a simple Library domain and includes **JWT authentication**.

---

## Architecture / Design Philosophy
The solution is structured using separation of concerns:

- **Library.Api**  
  ASP.NET Core Web API project (controllers, routing, Swagger/OpenAPI, authentication middleware, EF migrations).

- **Library.Application**  
  Interfaces and DTOs used by the API (contracts).  
  The API returns **DTOs** (not EF entities) to keep the client-facing model decoupled from persistence.

- **Library.Infrastructure**  
  EF Core persistence (DbContext, Entities) + concrete service implementations.

**ORM:** Entity Framework Core (EF Core) is used to map C# entity classes to SQL tables.  
**Routes:** REST endpoints under `/api/...`  
**Idempotent:** `GET`, `PUT`, and `DELETE` are intended to be idempotent (repeating them results in the same outcome).

---

## Services (CRUD)
Minimum 4 CRUD resource types are provided:

- **Authors** - CRUD
- **Books** - CRUD (ISBN uniqueness enforced)
- **Members** - CRUD (Email uniqueness enforced)
- **Loans** - CRUD + return behaviour

All resources are exposed via controllers under:
- `/api/Authors`
- `/api/Books`
- `/api/Members`
- `/api/Loans`

---

## Authentication / Identity (JWT)
The API uses **JWT Bearer authentication**.

Auth endpoints:
- `POST /api/Auth/register`
- `POST /api/Auth/login`

Once logged in, paste your token into Swagger’s **Authorize** button.

JWT config is stored in `appsettings.json` under `Jwt`.

---

## Persistent Storage
EF Core + SQL Server is used for persistent storage.

Tables/entities include:
- `Authors`
- `Books` (many-to-one with Authors)
- `Members`
- `Loans` (links Members and Books)
- `Users` (for authentication)

Relationships:
- One Author → Many Books
- One Member → Many Loans
- One Book → Many Loans (over time)

---

## Setup (Local Development)

### Prerequisites
- .NET SDK 8
- SQL Server
- Visual Studio 2022

### Configuration
Ensure `Library.Api/appsettings.json` contains a valid connection string:

```json
"ConnectionStrings": {
  "LibraryDb": "Server=(localdb)\\MSSQLLocalDB;Database=LibraryDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```

### Database Migrations
Migrations are stored in `Library.Api/Migrations`.

From Package Manager Console (Default Project: `Library.Api`):
- `Add-Migration <Name>`
- `Update-Database`

### Run
- Visual Studio: Run `Library.Api`

Swagger UI:
- `https://localhost:<port>/swagger`

---

## Use Cases & Acceptability Cases

### 1 - Register and Login
**As a user**, I want to register and login so I can access protected endpoints.

**Acceptability:**
- Registering a new username returns success
- Registering an existing username returns conflict
- Logging in with correct credentials returns OK with a JWT
- Logging in with wrong credentials returns Unauthorized

### 2 - Manage Authors (CRUD)
**As an authenticated user**, I want to create and manage authors.

**Acceptability:**
- GET returns a list of authors
- POST creates a new author
- PUT updates an existing author
- DELETE removes an author or restricted due to related books

### 3 - Manage Books (CRUD)
**As an authenticated user**, I want to create and manage books.

**Acceptability:**
- Books can be created with a valid `authorId`
- Creating a book with duplicate ISBN returns a conflict
- Creating a book with invalid authorId returns failure

### 4 - Manage Members (CRUD)
**As an authenticated user**, I want to create and manage members.

**Acceptability:**
- Member emails must be unique (duplicate returns conflict)
- Standard CRUD returns correct HTTP codes

### 5 - Loan a Book
**As an authenticated user**, I want to loan a book to a member.

**Acceptability:**
- A loan can be created for an existing book + member
- Loaning a book that is already on an active loan returns conflict
- Returning a loan allows the book to be loaned again

---

## Testing

### Swagger
Swagger UI was used for manual testing during development.

### Postman
A Postman collection is included in the repository under `postman/` and demonstrates:
- Register + Login (JWT)
- CRUD for Authors / Books / Members / Loans
- Negative tests (401 Unauthorized, 409 Conflict, invalid IDs)

**How to run:**
1. Import the collection: `postman/SOA_CA2.postman_collection.json`
2. (Optional) Import environment: `postman/SOA_CA2.postman_environment.json`
3. Set `baseUrl` in the environment to your API URL (e.g. `https://localhost:7089`)
4. Open the collection → **Run 1 iteration**

The login request stores the JWT into the environment variable `token` and all protected requests use `Bearer {{token}}`.

### Unit Tests
Unit tests are included in `Library.Tests` and cover key business rules (Auth + Loans).

**Run all tests:**
```bash
dotnet test
```

---

## Deployment (Cloud)

The API is deployed to Azure App Service.

- **Base URL:** `https://soa-c6aneqg6bda5facf.westeurope-01.azurewebsites.net`
- **Swagger UI:** `https://soa-c6aneqg6bda5facf.westeurope-01.azurewebsites.net/swagger/index.html`
