# ToDoList API

A task-management backend built with **.NET 8** and **Clean Architecture**. It provides JWT
authentication and (in progress) to-do item management, backed by SQL Server via Entity
Framework Core.

## Features

- **Authentication** — user registration, login, JWT access tokens, and rotating refresh
  tokens (refresh / logout).
- **Secure passwords** — salted password hashing.
- **To-do items** — create, track, prioritize, and manage tasks with due dates, reminders,
  and soft delete. *(CRUD implementation in progress — see `plan.md`.)*
- **Planned** — validation, global error handling, AI-assisted task features, email
  notifications, reminders, and Redis caching.

## Tech Stack

| Area            | Technology                       |
| --------------- | -------------------------------- |
| Framework       | ASP.NET Core (.NET 8)            |
| Architecture    | Clean Architecture               |
| Database        | SQL Server + EF Core             |
| Auth            | JWT (access + refresh tokens)    |
| Caching (planned) | Redis                          |
| API docs        | Swagger / OpenAPI                |

## Project Structure

```
ToDoList.sln
src/
├── ToDoList.Domain/         # Entities & enums (no dependencies)
├── ToDoList.Application/    # Interfaces, DTOs, services, exceptions, settings
├── ToDoList.Persistence/    # EF Core: DbContext, repositories, migrations, implementations
└── ToDoList.Api/            # Web API: controllers, configuration, middleware, Program.cs
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local or remote)
- `dotnet-ef` tool: `dotnet tool install --global dotnet-ef`

### Setup

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd ToDoList
   ```
2. Configure `src/ToDoList.Api/appsettings.json` (or user-secrets) with your SQL Server
   connection string and JWT settings. **Do not commit real secrets.**
3. Restore and build:
   ```bash
   dotnet restore
   dotnet build
   ```
4. Apply database migrations:
   ```bash
   dotnet ef database update --project src/ToDoList.Persistence --startup-project src/ToDoList.Api
   ```
5. Run the API:
   ```bash
   dotnet run --project src/ToDoList.Api
   ```
6. Open Swagger UI (development) at `https://localhost:<port>/swagger`.

## API Endpoints

### Auth (`/api/v1/auth`)

| Method | Route            | Description                    |
| ------ | ---------------- | ------------------------------ |
| POST   | `/register`      | Register a new user            |
| POST   | `/login`         | Log in, receive tokens         |
| POST   | `/refresh-token` | Exchange a refresh token       |
| POST   | `/logout`        | Revoke a refresh token         |

### To-Do Items (`/api/v1/todoitems`)

CRUD endpoints are in progress. See `plan.md` for the roadmap.

## Configuration

Key settings in `appsettings.json`:

- `ConnectionStrings:DatabaseConnection` — SQL Server connection string.
- `ConnectionStrings:Redis` — Redis connection (planned).
- `Jwt` — `Issuer`, `Audience`, `SecurityKey`, `Lifetime`, `RefreshTokenLifetimeDays`.

> **Security note:** Move the JWT `SecurityKey`, database password, and any API keys out of
> `appsettings.json` into user-secrets or environment variables before deploying.

## Roadmap

See [`plan.md`](./plan.md) for the full A→Z task list to complete the backend.

## Contributor Notes

See [`CLAUDE.md`](./CLAUDE.md) for architecture conventions and development guidance.
