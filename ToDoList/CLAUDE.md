# CLAUDE.md

Guidance for Claude Code (and other AI assistants) when working in this repository.

## Project Overview

**ToDoList** is a task-management backend built with **.NET 8** using **Clean Architecture**.
It exposes a REST API for user authentication (JWT) and to-do item management, backed by
**SQL Server** via **Entity Framework Core**. AI assistance, email notifications, and Redis
caching are planned/partially scaffolded.

## Architecture

The solution follows Clean Architecture with dependencies pointing inward
(`Api → Application → Domain`, `Persistence → Application → Domain`):

- **`ToDoList.Domain`** — Entities and enums. No dependencies.
  - Entities: `User`, `ToDoItem`, `RefreshToken`. Enums: `UserRole`, `PriorityLevel`.
- **`ToDoList.Application`** — Business logic, interfaces (abstractions), DTOs, services,
  exceptions, settings, converters. Depends only on Domain.
  - Abstractions: `IBaseRepository<T>`, `ICurrentUserService`, `ITokenService`,
    `IPasswordHasherService`, `IAIService`, `INotificationService`.
  - Services: `AuthService` (complete), `ToDoItemService` (stub — not yet implemented).
- **`ToDoList.Persistence`** — EF Core implementation. **Note:** the project folder is named
  `ToDoList.Persistence` but the assembly and namespaces are `ToDoList.Infrastructure`.
  Contains `AppDbContext`, `BaseRepository<T>`, entity mappings, migrations, and
  implementations (`TokenService`, `PasswordHasherService`, and stubbed AI/email services).
- **`ToDoList.Api`** — ASP.NET Core Web API. Controllers, DI/JWT configuration, middleware,
  `Program.cs`. Depends on Application and Persistence.

## Key Conventions

- **DI registration** lives in three extension methods, all called from `Program.cs`:
  - `ConfigureInfrastructure` (Persistence/Configurations.cs) — DbContext, repositories, token/hash services.
  - `ConfigureApplication` (Application/Configurations.cs) — application services.
  - `ConfigureDI` (Api/Configurations/DIConfigurations.cs) — API-layer services (e.g. `CurrentUserService`).
- **Repository pattern:** use the generic `IBaseRepository<T>`. `GetAllQuery()` returns
  `IQueryable<T>` for composing EF queries; remember to `SaveChangesAsync()` after mutations.
- **Auth:** JWT access tokens + rotating refresh tokens. Settings bind from the `Jwt` section.
  Passwords are salted and hashed via `IPasswordHasherService`.
- **Errors:** throw the custom exceptions in `Application/Exceptions`
  (`NotFoundException`, `UnauthorizedException`, `ValidationException`, etc.). A global
  exception-handling middleware to map these to HTTP status codes is still TODO.
- **User scoping:** to-do items belong to a user (`ToDoItem.UserId`). Use `ICurrentUserService`
  to resolve the logged-in user and always scope/authorize by it.
- Prefer `DateTime.UtcNow` for all timestamps (existing code mixes `Now` and `UtcNow` — treat
  `UtcNow` as the target).

## Common Commands

Run from the repository root.

```bash
# Restore & build
dotnet restore
dotnet build

# Run the API (from the API project)
dotnet run --project src/ToDoList.Api

# EF Core migrations (run from the API project so config is resolved)
dotnet ef migrations add <Name> --project src/ToDoList.Persistence --startup-project src/ToDoList.Api
dotnet ef database update --project src/ToDoList.Persistence --startup-project src/ToDoList.Api
```

Swagger UI is served in development at `/swagger`.

## Configuration

- `src/ToDoList.Api/appsettings.json` holds connection strings (`DatabaseConnection`, `Redis`)
  and the `Jwt` section. **Do not commit real secrets** — move the JWT `SecurityKey`, DB
  password, and any API keys to user-secrets or environment variables.
- Default DB: SQL Server (`ToDoListPro`). Redis is configured but not yet wired up.

## Current State (what's done vs. TODO)

- **Working:** Auth flow (register/login/refresh/logout), JWT, password hashing, EF Core +
  migrations, generic repository.
- **Stubbed / TODO:** `ToDoItemsController` and `ToDoItemService` (CRUD), validation
  (FluentValidation), global exception middleware, AI services (`OpenAIService`,
  `AntropicService`), `EmailNotificationService`, Redis caching, background reminders,
  email confirmation, tests. See `plan.md` for the full task list.

## Working Agreements

- Keep the Clean Architecture dependency direction intact — Domain and Application must not
  reference Persistence or Api.
- When adding a feature, wire it through the correct layer: interface in Application,
  implementation in Persistence/Api, registration in the matching `Configure*` method.
- Delete the placeholder files (`Api/Middlewares/a.cs`, `Application/Validators/a.cs`) when
  replacing them with real implementations.
- Refer to `plan.md` for the roadmap and keep it updated as tasks are completed.
