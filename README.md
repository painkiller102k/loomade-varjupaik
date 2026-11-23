# Loomade varjupaik – Animal Shelter

A system for managing a shelter's animals and adoptions. Staff add animals with a photo and a description.
Visitors browse the public list, filter and search it, and submit adoption applications.

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

### 3. Worker (background service)

Reminds staff about upcoming vaccinations. Every day at 08:00 (local time) it logs in to the API as the `Service`
account, calls `GET /api/vaccinations/upcoming?days=7` and sends one `POST /api/notifications` per vaccination.
It talks to the API with `HttpClient` only and never touches the database. The business logic stays in the API.

```bash
dotnet run --project Varjupaik.Worker
```

Settings are in `Varjupaik.Worker/appsettings.json` (`Api` and `Worker` sections). To try it without waiting until 08:00:

```bash
Worker__RunOnStartup=true dotnet run --project Varjupaik.Worker
```

The API must be running first. The notifications appear for logged-in staff on the website.

### 4. Tests

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
| POST | `/api/animals/{id}/applications` | Visitor | Submit an adoption application (animal becomes `Reserved`) |
| GET | `/api/applications?status=` | Admin, Staff | List applications, optional filter by status |
| GET | `/api/applications/my` | Visitor | The visitor's own applications |
| GET | `/api/vaccinations/upcoming?days=7` | Admin, Staff, Service | Vaccinations due in the next N days (or overdue), adopted animals excluded |
| GET | `/api/animals/{id}/vaccinations` | Admin, Staff | Vaccination history of an animal |
| POST | `/api/animals/{id}/vaccinations` | Admin, Staff | Add a vaccination |
| GET | `/api/statistics/average-stay` | anonymous | Average days in the shelter per species |
| POST | `/api/notifications` | Service | Create a notification (also pushed to staff over WebSocket) |
| GET | `/api/notifications` | Admin, Staff | Latest 20 notifications |
| GET | `/api/animals/{id}/vet-visits` | Admin, Staff | Vet visit history of an animal, newest first |
| POST | `/api/animals/{id}/vet-visits` | Admin, Staff | Add a vet visit |
| PATCH | `/api/applications/{id}/status` | Admin, Staff | Approve or reject an application; the animal status changes in the same transaction |

`sortBy` values: `newest` (default), `oldest`, `name`, `age`.

Status codes used: `200`, `201`, `204`, `400`, `401`, `403`, `404`.

All errors are returned as `application/problem+json` (ProblemDetails) by one global exception handler, and
important events (logins, applications, status changes, errors) are logged with `ILogger`.

All requests can be tested with [`Varjupaik.Api/Varjupaik.Api.http`](Varjupaik.Api/Varjupaik.Api.http) or in Swagger
(use the **Authorize** button with the token from `/api/auth/login`).

## Real-time notifications (WebSocket)

Staff see new applications immediately. Connect to `ws://localhost:5156/ws/applications?access_token=<JWT>`
(browsers cannot send an `Authorization` header over WebSocket, so the token goes in the query string).
Only `Admin` and `Staff` may connect; others get `401` / `403`.

After a visitor's application is saved, the services layer sends this message to every connected staff member:

```json
{ "type": "ApplicationReceived", "data": { "id": 1, "animalName": "Reks", "applicantName": "Mari", "status": "New" }, "sentAt": "..." }
```

## Application processing

`New` → `Approved` or `Rejected`. The animal status changes automatically, in the same transaction:

- a new application: `Available` → `Reserved`
- an application is approved: the animal becomes `Adopted` and all other new applications for it are rejected
- an application is rejected and no other new applications exist: `Reserved` → `Available`

## Data model

- **User** – Id, Email, Name, PasswordHash, Role
- **Animal** – Id, Name, Species, Age, Description, PhotoPath, Status (`Available`, `Reserved`, `Adopted`), CreatedAt, AdoptedAt
- **Application** – Id, AnimalId, ApplicantUserId, Message, Status (`New`, `Approved`, `Rejected`), CreatedAt
- **VetVisit** – Id, AnimalId, VisitDate, Reason, Notes, Vet
- **Vaccination** – Id, AnimalId, Name, VaccinationDate, NextDueDate
- **Notification** – Id, Title, Message, CreatedAt

Deleting an animal also deletes its vet visits and vaccinations.

## Branches

| Branch | What it contains |
|--------|------------------|
| `main` | The final version of the project, everything is merged here |
| `frontend` | React (Vite) client: pages, design, forms and live notifications |
| `backend` | Web API and services layer: controllers, business logic, repositories, EF Core migrations and tests |
| `worker` | Background service (Worker Service) that sends vaccination reminders every day at 08:00 |

The work on each part was done in its own branch and then merged into `main`. The branches are kept to show how the project progressed.
