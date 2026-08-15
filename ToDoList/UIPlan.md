# ToDoList — Blazor UI Implementation Plan

Blazor **WebAssembly** SPA that consumes the existing `ToDoList.Api` (JWT auth + to-do CRUD).
API base URLs: `https://localhost:7050` / `http://localhost:5227`.
Blazor dev URLs: `https://localhost:7150` / `http://localhost:5150`.

---

## 0. Decisions
- [x] Blazor WebAssembly (standalone), added to the solution
- [x] Project: `src/ToDoList.BlazorUI`
- [x] Styling: Bootstrap 5 + Bootstrap Icons (via CDN)
- [x] Token storage: localStorage via `Blazored.LocalStorage`
- [x] Reuse `PriorityLevel` / `UserRole` enums from `ToDoList.Domain`; client DTOs for the rest

## 1. Project scaffolding
- [x] `ToDoList.BlazorUI.csproj` (net8.0 Blazor WASM) + packages
- [x] Added to `ToDoList.sln`
- [x] Project reference to `ToDoList.Domain`
- [x] `wwwroot/index.html`, `appsettings.json` (`ApiBaseUrl`), `css/app.css`, `Properties/launchSettings.json`
- [x] `Program.cs`, `App.razor`, `_Imports.razor`

## 2. Client models
- [x] `LoginRequest`, `RegisterRequest`, `LoginResponse`, `RefreshTokenRequest`
- [x] `ToDoItemDto`, `ToDoItemCreateRequest`, `ToDoItemUpdateRequest`, `TaskFormModel`
- [x] `PagedResult<T>`, `ToDoItemQuery` (+ `ToDoItemSortBy`), `ApiResult` / `ProblemDetailsResponse`, `PurgeResult`

## 3. HTTP & authentication infrastructure
- [x] `ITokenStore` / `TokenStore` (localStorage)
- [x] `JwtAuthenticationStateProvider` (parses JWT claims, normalises role/email)
- [x] `AuthHeaderHandler` — attaches Bearer + refresh-on-401 with request replay
- [x] `ApiClient` typed wrapper (GET/POST/PUT/PATCH/DELETE + ProblemDetails mapping)
- [x] DI wiring in `Program.cs`; `CascadingAuthenticationState` + `AuthorizeRouteView` in `App.razor`

## 4. Client services
- [x] `AuthClientService` (Register, Login, Logout, RefreshToken via handler, PurgeTokens)
- [x] `ToDoClientService` (GetAll, GetById, Create, Update, Delete, ToggleComplete)

## 5. Authentication UI
- [x] `Login.razor` + `Register.razor` (client validation, server-error display, redirects)
- [x] Logout in nav bar; redirect authenticated users away from login/register; `RedirectToLogin`

## 6. Layout & navigation
- [x] `MainLayout` (user name + role badge + logout)
- [x] `NavMenu` (role-gated Admin link via `AuthorizeView`)
- [x] `LoadingSpinner`, `ToastContainer` components

## 7. To-do list page (`/`, `/tasks`)
- [x] Table of the user's tasks with pagination (prev/next + page size)
- [x] Filters (status, priority, due-date range) and sorting (+ asc/desc toggle)
- [x] Inline complete/undo toggle, edit + delete (with confirm)
- [x] Priority color badges, overdue highlighting, empty state

## 8. Create / edit to-do
- [x] Shared `TaskForm` component
- [x] `TaskCreate` (`/tasks/new`) and `TaskEdit` (`/tasks/{id}/edit`)

## 9. Role-based / admin features
- [x] `AuthorizeView`-gated Admin link
- [x] `Admin.razor` with purge-expired-tokens button

## 10. UX polish
- [x] Toast notifications, ProblemDetails → friendly messages, loading indicators
- [x] Session expiry handled (handler clears tokens + auth state → redirect to login)

## 11. Backend adjustments
- [x] `Cors:AllowedOrigins` (Blazor origins) added to `appsettings.Development.json`
- [ ] `dotnet dev-certs https --trust` (run once on your machine)

---

## How to run (on your machine — no SDK in this environment)

```bash
# 1. Trust the dev HTTPS cert (once)
dotnet dev-certs https --trust

# 2. Start the API (terminal 1)
dotnet run --project src/ToDoList.Api          # https://localhost:7050

# 3. Start the Blazor UI (terminal 2)
dotnet run --project src/ToDoList.BlazorUI      # https://localhost:7150
```

Then open `https://localhost:7150` and sign in with a seeded account
(e.g. `akmal` / `Password@123` for super-admin, `nodira` / `Password@123` for a regular user).

Notes:
- If your API runs on a different port, update `ApiBaseUrl` in
  `src/ToDoList.BlazorUI/wwwroot/appsettings.json` and the matching origin in
  the API's `Cors:AllowedOrigins`.
- No SDK/build was available here, so build on your machine; tell me any compiler errors and I'll fix them.
