# Käpakodu – Animal Shelter

A system for managing a shelter's animals and adoptions. Staff add animals with a photo and a description.
Visitors browse the public list, filter and search it, and submit adoption applications.

School final project for the courses **Programming** and **Distributed Applications**.

## Architecture

```
React (Vite) client  ──HTTP/JSON──>  ASP.NET Core Web API  ──EF Core──>  SQLite
                                           ^
Worker Service  ──HTTP (Service account)───┘
```

| Project | Description |
|---------|-------------|
| `Varjupaik.Api` | ASP.NET Core Web API: controllers, JWT authentication, Swagger, CORS, photo uploads |
| `Varjupaik.Services` | Models, `AppDbContext`, migrations, repositories and business logic services |
| `Varjupaik.Frontend` | React + Vite web client (talks to the API only) |
| `Varjupaik.Worker` | Background service (separate process, uses the API with `HttpClient`, no `DbContext`) |
| `Varjupaik.Tests` | xUnit unit tests (Moq + EF Core InMemory) |

Only the Web API accesses the database. Project references: `Api -> Services`, `Tests -> Services`.

### Patterns used

- **Repository pattern** – `IAnimalRepository`, `IApplicationRepository`, `IUserRepository`
- **Service layer** – `AnimalService`, `ApplicationService`, `AuthService`
- **Dependency injection** – everything is registered in `Program.cs`
- **DTOs** – the API never returns database entities (e.g. no `PasswordHash` is exposed)

## Requirements

- .NET 10 SDK
- Node.js 20+

## How to run

### 1. API

```bash
dotnet run --project Varjupaik.Api --launch-profile http
```

- API: http://localhost:5156
- Swagger UI: http://localhost:5156/swagger

The SQLite database (`varjupaik.db`) is created automatically. Migrations are applied on startup
and seed data (users and 10 animals) is added.

### 2. Frontend

```bash
cd Varjupaik.Frontend
npm install
npm run dev
```

Open http://localhost:5173. The API address can be changed with the `VITE_API_URL` environment variable.

### 3. Tests

```bash
dotnet test
```

### Migrations

```bash
dotnet tool restore
dotnet tool run dotnet-ef migrations add <Name> --project Varjupaik.Services --startup-project Varjupaik.Api --output-dir Data/Migrations
```

## Test users

Seeded from `Varjupaik.Api/appsettings.Development.json`:

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@varjupaik.ee | Admin123! |
| Staff | staff@varjupaik.ee | Staff123! |
| Service | service@varjupaik.ee | Service123! |

A **Visitor** account can be created on the "Registreeru" (register) page.

## Roles

| Role | Can do |
|------|--------|
| Visitor | browse animals, submit adoption applications |
| Staff | add, edit and delete animals |
| Admin | same as Staff |
| Service | account for the background Worker |

## API endpoints

| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| POST | `/api/auth/register` | anonymous | Register a new visitor, returns a JWT |
| POST | `/api/auth/login` | anonymous | Log in, returns a JWT |
| GET | `/api/animals?species=&status=&search=&sortBy=&page=&pageSize=` | anonymous | Public list with filters, search, sorting and paging |
| GET | `/api/animals/{id}` | anonymous | One animal |
| POST | `/api/animals` | Admin, Staff | Add an animal (multipart/form-data, optional photo) |
| PUT | `/api/animals/{id}` | Admin, Staff | Update an animal |
| DELETE | `/api/animals/{id}` | Admin, Staff | Delete an animal |
| POST | `/api/animals/{id}/applications` | Visitor | Submit an adoption application |

`sortBy` values: `newest` (default), `oldest`, `name`, `age`.

Status codes used: `200`, `201`, `204`, `400`, `401`, `403`, `404`.

All requests can be tested with [`Varjupaik.Api/Varjupaik.Api.http`](Varjupaik.Api/Varjupaik.Api.http) or in Swagger
(use the **Authorize** button with the token from `/api/auth/login`).

## Data model

- **User** – Id, Email, Name, PasswordHash, Role
- **Animal** – Id, Name, Species, Age, Description, PhotoPath, Status (`Available`, `Reserved`, `Adopted`), CreatedAt
- **Application** – Id, AnimalId, ApplicantUserId, Message, Status (`New`, `Approved`, `Rejected`), CreatedAt

## Branches

- `main` – stable version
- `frontend` – React client development
