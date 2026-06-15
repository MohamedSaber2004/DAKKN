# DAKKN Project — Architecture, Rules & Constraints

## 1. Project Overview

DAKKN is a premium vinyl sticker e-commerce platform built with **ASP.NET Core 8 MVC** following **Clean Architecture** principles. It supports bilingual (Arabic / English) content, role-based access (Admin / User), JWT authentication for the API layer, and ASP.NET Core Identity cookie authentication for the MVC layer.

---

## 2. Solution Structure

```
DAKKN.sln
├── DAKKN.Domain           # Entities, Enums, Repository Interfaces
├── DAKKN.Application      # Use Cases (CQRS via MediatR), DTOs, Interfaces, Validators, Localization
├── DAKKN.Infrastructure   # JWT, Email, Identity setup, Repository Implementations
├── DAKKN.Persistence      # EF Core DbContext, Migrations, Seeding, Audit Interceptor
└── DAKKN.MVC              # Controllers (MVC + API), Views (Razor), Layouts, wwwroot assets
```

**Dependency direction (strict):**
`Domain ← Application ← Infrastructure / Persistence ← MVC`  
No layer may reference a layer above it in this chain.

---

## 3. Architecture Patterns

### 3.1 Clean Architecture / CQRS
- All business logic lives in **`DAKKN.Application`**.
- Every operation is a **Command** or **Query** implementing `IRequest<T>` (MediatR).
- Each Command/Query has:
  - `*Command.cs` / `*Query.cs` — the request record
  - `*CommandHandler.cs` / `*QueryHandler.cs` — the handler
  - `*CommandValidator.cs` / `*QueryValidator.cs` — FluentValidation rules
- Folder layout: `Features/{Domain}/Commands|Queries/{OperationName}/`

### 3.2 MediatR Pipeline Behaviours (order matters)
Registered in `DAKKN.Application/DependencyInjection.cs` in this exact order:
1. `UnhandledExceptionBehaviour` — catches and logs all unhandled exceptions
2. `ValidationBehaviour` — runs FluentValidation; throws `ValidationException` on failure
3. `PerformanceBehaviour` — logs a warning for requests taking > 600 ms
4. `LoggingBehaviour` — logs request name and serialized body

### 3.3 Repository / Unit of Work
- `IGenericRepository<T, TKey>` in Domain; implemented by `GenericRepository<T, TKey>` in Infrastructure.
- `IUnitOfWork` wraps all repositories; use `_unitOfWork.GetRepository<T>()` — never inject `DbContext` directly into handlers.
- `SaveChangesAsync()` must be called through `IUnitOfWork`, not through `IApplicationDbContext`.

### 3.4 Dual Authentication
| Context | Scheme | Usage |
|---------|--------|-------|
| `/api/*` routes | JWT Bearer | Mobile / SPA clients |
| MVC pages | ASP.NET Identity Cookies | Browser sessions |

Policy scheme `"JWT_OR_COOKIE"` selects the scheme based on path prefix (`/api/` → JWT, else cookie).

---

## 4. Domain Layer Rules

### 4.1 Entities
- All entities extend `BaseEntity<TKey>` which provides: `Id`, `CreatedAt`, `UpdatedAt`, `DeletedAt`, `CreatedBy`, `UpdatedBy`, `DeletedBy`, `IsDeleted`, `IsActive`.
- **Soft-delete pattern**: mark `IsDeleted = true` via `MarkAsDeleted(userId)` — never hard-delete through the repository `Delete()` method unless intentional.
- Entity state should be changed only through **domain methods** (e.g., `user.Activate()`, `token.Revoke()`), not by setting properties directly from handlers.
- `ApplicationUser` extends `IdentityUser<Guid>` and also implements `IBaseEntity<Guid>`.

### 4.2 Enums
Located in `DAKKN.Domain/Enums/`:
- `UserType` — `User`, `Admin`
- `Gender` — `Male`, `Female`
- `LanguageCode` — `ar`, `en`

### 4.3 Repository Interfaces
Defined in `DAKKN.Domain/Repositories/Interfaces/` — never reference EF Core here.

---

## 5. Application Layer Rules

### 5.1 Exceptions
Use only the custom exceptions in `DAKKN.Application/Common/Exceptions/`:
| Exception | HTTP Equivalent |
|-----------|----------------|
| `ValidationException` | 400 |
| `BadRequestException` | 400 |
| `NotFoundException` | 404 |
| `UnAuthorizedException` | 401 |

**Never throw raw `Exception`** from a handler — always use one of the above.

### 5.2 API Response Wrapper
All API responses must be wrapped with `ApiResponse<TData>`:
```csharp
ApiResponse<TData>.Ok(data, message)       // 200
ApiResponse<TData>.Error(errors, message, statusCode)  // 4xx/5xx
```
The `ApiExceptionFilterAttribute` in the MVC project handles automatic wrapping of thrown exceptions.

### 5.3 Localization
- **All user-facing strings** come from `JsonLocalizationProvider` loaded from `Localization/Resources/messages.ar.json` and `messages.en.json`.
- String keys are defined as `KeyString` instances in `LocalizationKeys` static class — **never hard-code raw string keys in handlers or validators**.
- Use `_localizer[LocalizationKeys.SomeCategory.SomeKey.Value]` in handlers and validators.
- `JsonLocalizationProvider.GetLocalizedString(key)` resolves the current culture automatically (defaults to `ar` for unsupported cultures).

### 5.4 Pagination
Use the `PaginatorExtensions.AsPagginatedListAsync()` extension on any `IQueryable<T>`.  
Constants in `PagginatedResult<T>`:
- `DefaultPageNumber = 1`
- `DefaultPageSize = 20`
- `MaxPageSize = 100`

### 5.5 Image Uploads
- `IImageValidator` (implemented by `ImageValidator`) handles all file uploads.
- Allowed extensions: `.jpg`, `.jpeg`, `.webp`, `.bmp`, `.png`, `.gif`, `.jfif`
- Max file size: **10 MB**
- Upload place is an `int` that maps to a folder via `UploadPaths.GetPath(place)`:
  - `0` → Root, `1` → Products, `2` → Categories, `3` → Users, `4` → Invoices, `5` → Reviews
- File names are prefixed with the place index and include a timestamp (e.g., `1202506151230001.jpg`).
- The first digit of an existing image name **must match** its `UploadPlace` — validated by `UpdateImageCommandValidator`.

---

## 6. Infrastructure Layer Rules

### 6.1 JWT
- Access tokens include claims: `sub` (userId), `email`, `jti`, `FullName`, `role`.
- Refresh tokens are JWT tokens with `TokenType: RefreshToken` claim stored in the `UserRefreshTokens` table.
- Settings bound from `appsettings.json → JwtSettings`: `Secret`, `Issuer`, `Audience`, `ExpiryInMinutes`, `RefreshTokenExpiryDays`.

### 6.2 Email
- Uses `MailKit` / `MimeKit` via SMTP.
- Settings bound from `appsettings.json → EmailSettings`.
- Only `SendPasswordResetEmailAsync` is currently in `IEmailService` — extend the interface and implementation together.

### 6.3 Identity Configuration
- Identity options are bound from `appsettings.json → IdentityOptions`.
- `ApplicationUserClaimsPrincipalFactory` adds `FullName` as an extra claim to the cookie identity.
- Default admin seed: `admin@dakkn.com` / `Admin@123` — **change in production**.

---

## 7. Persistence Layer Rules

### 7.1 DbContext
- `DAKKNDbContext` extends `IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid, ...>`.
- Default schema: `dbo`.
- Entity configurations in `DAKKN.Persistence/Configuration/` using `IEntityTypeConfiguration<T>` — applied automatically via `ApplyConfigurationsFromAssembly`.

### 7.2 Audit Interceptor
`AuditInterceptor` (SaveChangesInterceptor) runs on every `SaveChanges`:
- `Added` → calls `MarkAsCreated(userId)`
- `Modified` → calls `MarkAsUpdated(userId)`
- `Deleted` → converts to `Modified` and calls `MarkAsDeleted(userId)` (soft-delete enforcement)

### 7.3 Migrations
Run from the `DAKKN.Persistence` project. Always create a named migration describing the change.

---

## 8. MVC / Presentation Layer Rules

### 8.1 API Controllers
- Extend `BaseApiController` (which extends `Controller`).
- Decorated with `[ApiController]` and `[ApiVersion("1.0")]`.
- Route prefix: `api/v{version:apiVersion}/{resource}` (see `ApiRoutes` static class).
- Return using helper methods: `Ok<T>(data)`, `Created(uri, data)`, `Accepted(data)`, `Deleted(data)`.
- Never return raw `IActionResult` with manual `new ObjectResult(...)`.

### 8.2 MVC Controllers
- Extend `Controller` directly (not `BaseApiController`).
- Use `[RoleAuthorize]` attribute instead of `[Authorize]` — it handles both API (JSON 401) and MVC (redirect) responses.
- Admin pages: `[RoleAuthorize(UserType.Admin)]`
- Customer pages: `[RoleAuthorize(UserType.User, UserType.Admin)]`
- Open pages: no attribute

### 8.3 Layouts
| Layout | Used By |
|--------|---------|
| `_LayoutLanding` | Public marketing pages (Home, Privacy, Terms, About) |
| `_LayoutAuth` | Auth pages (Login, Register, OTP, ForgotPassword, ResetPassword) |
| `_LayoutAdmin` | Admin dashboard pages |
| `_LayoutCustomer` | Authenticated customer pages |

Default layout set in `_ViewStart.cshtml` is `_LayoutLanding`.

### 8.4 Authorization Filter
`RoleAuthorizeAttribute` (implements `IAuthorizationFilter`):
- Unauthenticated + API path → JSON 401
- Unauthenticated + MVC path → `ChallengeResult` (redirects to `/auth/login`)
- Authenticated but wrong role + API → JSON 403
- Authenticated but wrong role + MVC → `ForbidResult` (redirects to `/auth/access-denied`)

### 8.5 Exception Handling
`ApiExceptionFilterAttribute` (registered globally in `AddControllersWithViews`):
- Converts all known exceptions to structured `ApiResponse<object>` JSON.
- Handles: `ValidationException`, `NotFoundException`, `BadRequestException`, `UnAuthorizedException`, `DbUpdateConcurrencyException`, invalid `ModelState`.
- Unknown exceptions → 500 with generic message.

---

## 9. Localization & i18n

### 9.1 Server-side
- `JsonStringLocalizerFactory` replaces the default ASP.NET localizer factory.
- `IStringLocalizer<Messages>` is injected everywhere — backed by `JsonLocalizationProvider`.
- Supported cultures: `ar` (default), `en`.
- Culture selection order (in `Program.cs`): QueryString → Cookie (`.AspNetCore.Culture`) → Accept-Language header.

### 9.2 Client-side
- `window.currentTranslations` is injected server-side into every layout as a JSON object.
- `applyLang(lang)` in `landing.js` fetches translations via `/api/v1/translations?lang=xx` when not pre-injected.
- Language change → sets cookie `.AspNetCore.Culture` → reloads page.
- RTL (`dir="rtl"`) is set on `<html>` for `ar`, LTR for `en`.

---

## 10. Frontend Rules (Tailwind / JS)

### 10.1 Tailwind
- Loaded via CDN: `https://cdn.tailwindcss.com`.
- Custom config in `wwwroot/js/tailwind.config.js` — **all custom colors, spacing, shadows defined there**.
- Dark mode: `class` strategy — toggled by adding/removing `dark` class on `<html>`.
- Theme is persisted in `localStorage` key `dakkn_theme`.

### 10.2 CSS Files
| File | Scope |
|------|-------|
| `landing.css` | Shared across all layouts (animations, glass panels, navbar, mobile nav) |
| `admin.css` | Admin layout only (token system, status badges, sidebar, cards) |
| `auth.css` | Auth layout only (CSS variables for auth form theming) |
| `site.css` | Legacy/minor global overrides |

### 10.3 JavaScript
- `landing.js` — boot sequence, theme/lang management, scroll animations, category slider, product grid rendering.
- `tailwind.config.js` — Tailwind design token definitions.
- `attachment-compression.js` — client-side image/file compression utility (uses `browser-image-compression` and `fflate`).
- jQuery is loaded for MVC validation scripts only.
- **No browser storage (localStorage/sessionStorage) in Artifacts.**

### 10.4 Material Symbols
Icons use Google Material Symbols Outlined font. Filled variant via CSS:
```css
font-variation-settings: 'FILL' 1, 'wght' 400, 'GRAD' 0, 'opsz' 24;
```
Apply class `filled-icon` or inline style to activate filled style.

---

## 11. Security Rules

- **CSRF**: Anti-forgery tokens on all POST/PATCH/DELETE forms — `@Html.AntiForgeryToken()` in every form.
- **Rate Limiting**: Global limiter + stricter `auth` policy (configured via `appsettings.json → RateLimiting`).
- **CSP**: Applied via NWebsec middleware — inline scripts allowed in dev, stricter in production. External scripts only from approved CDNs.
- **Security Headers**: `X-Content-Type-Options`, `X-Frame-Options: DENY`, `X-XSS-Protection`, `Referrer-Policy: no-referrer`.
- **HSTS**: Enabled in production (365 days, includeSubdomains, preload).
- **Password Reset**: OTP is a 6-digit number, stored as SHA-256 hash in the user record with an expiry timestamp.
- **Refresh Tokens**: Stored in the database; single-use (revoked on each refresh cycle); new token issued on each valid refresh.

---

## 12. Configuration & Secrets

All secrets are in environment-specific `appsettings.{Environment}.json`:

| Key | Description |
|-----|-------------|
| `ConnectionStrings:DakknConnection` | SQL Server connection string |
| `JwtSettings:Secret` | JWT signing key (min 32 chars) |
| `JwtSettings:ExpiryInMinutes` | Access token lifetime |
| `JwtSettings:RefreshTokenExpiryDays` | Refresh token lifetime |
| `EmailSettings:*` | SMTP credentials |
| `IdentityOptions:*` | Password/lockout policy |
| `UploadPaths:*` | Physical upload folder names under `wwwroot` |
| `RateLimiting:Global/Auth:*` | Per-policy rate limit values |

**Never commit real secrets.** Use User Secrets in development (`appsettings.Development.json` is `.gitignored` in production environments).

---

## 13. Naming Conventions

| Artifact | Convention | Example |
|----------|-----------|---------|
| Commands | `{Verb}{Entity}Command` | `SignInCommand`, `UploadImageCommand` |
| Handlers | `{CommandName}Handler` | `SignInCommandHandler` |
| Validators | `{CommandName}Validator` | `SignInCommandValidator` |
| API Routes | `PascalCase` in `ApiRoutes` static class | `ApiRoutes.Auth.Login` |
| DTOs | `{Entity}Dto` or `{Operation}ResponseDto` | `AuthResponseDto`, `ProductDto` |
| ViewModels | `{Page}ViewModel` | `LoginViewModel`, `OrderListViewModel` |
| Interfaces | `I{Name}` | `ICurrentUserService`, `IJwtTokenService` |
| Localization Keys | `LocalizationKeys.{Category}.{Key}` | `LocalizationKeys.AuthMessages.InvalidCredentials` |

---

## 14. Constraints & Anti-Patterns to Avoid

1. **Do not** inject `DAKKNDbContext` directly into Application handlers — use `IUnitOfWork` or `IApplicationDbContext`.
2. **Do not** add business logic in MVC Controllers — dispatch via `_mediator.Send()` only.
3. **Do not** create new exception types — use the four defined in `Application/Common/Exceptions/`.
4. **Do not** hard-code localization strings — always go through `LocalizationKeys` and `IStringLocalizer`.
5. **Do not** use `localStorage` or `sessionStorage` inside Razor views for sensitive tokens beyond what the existing auth flow already does.
6. **Do not** reference `DAKKN.MVC` or `DAKKN.Infrastructure` from `DAKKN.Application` or `DAKKN.Domain`.
7. **Do not** call `context.SaveChangesAsync()` on `DbContext` inside handlers — always go through `IUnitOfWork.SaveChangesAsync()`.
8. **Do not** add new upload paths by integer — update `UploadPaths` class and `UploadPathsOptions` together.
9. **Do not** use `[Authorize]` — use `[RoleAuthorize]` instead for unified API/MVC behaviour.
10. **Do not** add migrations from the MVC project — always from `DAKKN.Persistence`.
11. **Do not** serialize `IFormFile` in logging — `LoggingBehaviour` handles this safely via try/catch.
12. **Do not** use Arabic or English text directly in C# strings intended for the user — always use a localization key.

---

## 15. Adding a New Feature — Checklist

1. **Domain**: Add entity to `DAKKN.Domain/Entities/` extending `BaseEntity<Guid>`. Add repository interface if needed.
2. **Application**:
   - Create `Features/{Domain}/Commands/{Operation}/` folder.
   - Add `*Command.cs`, `*CommandHandler.cs`, `*CommandValidator.cs`.
   - Add localization keys to `LocalizationKeys.cs` and both JSON files.
   - Add DTO to `DTOs/` if needed.
3. **Infrastructure**: Implement any new service interface from Application.
4. **Persistence**: Add `IEntityTypeConfiguration<T>` in `Configuration/`. Create and apply migration.
5. **MVC**:
   - Add API endpoint in the appropriate `Controllers/APIs/V1/` controller (or create new one extending `BaseApiController`).
   - Add MVC action + View if UI is required.
   - Use `[RoleAuthorize]` as appropriate.
   - Register route in `ApiRoutes.cs`.
6. **Tests**: Validate against existing patterns before submitting.