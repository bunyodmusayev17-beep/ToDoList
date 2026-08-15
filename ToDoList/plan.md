# ToDoList Backend — Completion Plan (A → Z)

## 1. Cleanup & Housekeeping
- [x] Delete placeholder files `Api/Middlewares/a.cs` and `Application/Validators/a.cs`
- [x] Fix `Program.cs`: replace `if (true || app.Environment.IsDevelopment())` with a proper environment check
- [x] Add `.gitignore` to exclude `bin/`, `obj/`, and other build artifacts
- [x] Standardize namespaces — chose `ToDoList.Infrastructure` as the canonical assembly/namespace (already consistent across the Persistence project)
- [x] Add a `README.md` with setup, run, and migration instructions

## 2. ToDoItem CRUD (core feature — currently empty)
- [x] Create `ToDoItemCreateDto` and `ToDoItemUpdateDto`
- [x] Add a `ToDoItemConverter` mapper for ToDoItem ↔ DTOs
- [x] Define full `IToDoItemService` interface (Create, GetById, GetAll, Update, Delete, ToggleComplete)
- [x] Implement `ToDoItemService` with repository + `ICurrentUserService` (scoped to the logged-in user)
- [x] Implement soft delete (uses `IsDeleted` / `DeletedAt` fields)
- [x] Set `CompletedAt` when an item is marked complete
- [x] Build out `ToDoItemsController`: POST, GET (list), GET by id, PUT, DELETE, PATCH complete
- [x] Add `[Authorize]` to the controller and enforce ownership on every action

## 3. Querying, Filtering & Pagination
- [x] Add pagination (page/pageSize, clamped to a max) to the ToDoItem list endpoint
- [x] Add filtering (by IsCompleted, Priority, DueDate range)
- [x] Add sorting (by CreatedAt, DueDate, Priority, Title)
- [x] Create a reusable `PagedResult<T>` DTO

## 4. Validation
- [x] FluentValidation packages (already referenced in the csproj)
- [x] Validator for `RegisterDto` (email format, password strength, username rules)
- [x] Validators for `LoginDto`, `RefreshTokenRequestDto`
- [x] Validators for `ToDoItemCreateDto` / `ToDoItemUpdateDto`
- [x] Register validators in DI and enable automatic validation via a global `ValidationFilter`

## 5. Error Handling
- [x] Implement a global exception-handling middleware
- [x] Map custom exceptions to proper HTTP status codes (400/401/404/409/500)
- [x] Return a consistent error response model (ProblemDetails / ValidationProblemDetails)
- [x] Replace raw `throw new Exception(...)` in `RegisterAsync` with `EmailAlreadyExistsException`
- [x] Standardize on `UnauthorizedException` in `LoginAsync`
- [x] Register the middleware in `Program.cs`
- [x] Fix `ValidationException`, `EmailAlreadyExistsException`, `UserNotFoundException` to derive from `Exception`

## 6. API Response Consistency
- [x] Wrap controller returns in `ActionResult<T>` with correct status codes (201 create, 204 delete, etc.)
- [x] Standardize auth endpoint responses (Register now returns 201 with `{ userId }`)

## 7. Authorization & Security
- [x] Apply role-based authorization (`[Authorize(Roles = "Admin,SuperAdmin")]` on the token-purge endpoint)
- [~] Secrets: config reads env-var overrides by default; seed credentials are override-able. Values left in `appsettings.json` per your note — move to user-secrets/env before production.
- [x] Use `DateTime.UtcNow` consistently (AuthService, TokenService, RefreshToken)
- [x] Add a service method to purge expired/revoked refresh tokens (+ admin endpoint)
- [x] Configure CORS policy (reads `Cors:AllowedOrigins`, falls back to permissive in absence)
- [x] Add rate limiting on auth endpoints (fixed window, 10/min)

## 8. Repository Enhancements
- [x] Add `GetByIdAsync` to `IBaseRepository` / `BaseRepository`; `SaveChangesAsync` now returns affected-row count
- [x] Add a global query filter for soft-deleted ToDoItems

## 9. Logging & Observability
- [x] Add structured logging (Serilog + Console sink)
- [x] Log requests (`UseSerilogRequestLogging`), errors (in middleware), and key auth events

## 10. Database & Migrations
- [x] Migrations applied manually (auto-migration removed by request)
- [x] Add a one-time sample data seeder: 10 users (1 super-admin, 2 admins, 7 users) + 30 to-do items, Uzbek names/titles (`DbSeeder.SeedSampleDataAsync`, called from `Program.cs` — comment out after first run)
- [x] Indexes confirmed on UserId, Token, Email (existing mappings)

## 11. Final Verification
- [ ] `dotnet build` the full solution — **run on your machine** (no .NET SDK available in this environment)
- [ ] `dotnet restore` to pull the new Serilog packages

---

## Notes — steps to run on your machine (SDK/network not available here)

The following couldn't be executed in this environment (no .NET SDK, Microsoft/GitHub network
blocked, and git can't run reliably on this mounted folder). Run them locally:

```bash
# 1. Restore (pulls the newly added Serilog packages) and build
dotnet restore
dotnet build

# 2. Apply the schema manually (auto-migration was removed):
dotnet ef database update --project src/ToDoList.Persistence --startup-project src/ToDoList.Api

# 3. Run ONCE to seed 10 users + 30 to-do items, then comment out the
#    SeedSampleDataAsync(...) call in Program.cs. Default password for every
#    seeded user is: Password@123
dotnet run --project src/ToDoList.Api

# 3. Git (this folder is not yet a repo here):
git checkout -b feature/finish-backend
git add -A
git commit -m "Implement ToDoItem CRUD, validation, error handling, security, logging, seeding"
git push -u origin feature/finish-backend
```

No new EF migration is required — the only model change (soft-delete query filter) does not
alter the schema.
