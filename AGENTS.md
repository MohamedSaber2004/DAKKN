# DAKKN — Agent Reference

## Stack
.NET 8, ASP.NET Core MVC, Clean Architecture, CQRS (MediatR), EF Core + SQL Server, Serilog, Tailwind (CDN).

## Solution quirks
- Solution file: `DAKKN.slnx`
- MVC project csproj: `DAKKN.Appearence.csproj` (folder is `DAKKN.MVC/`)
- No test projects. No CI/CD. No `.editorconfig` or `Directory.Build.props`.
- All appsettings (including secrets) are committed. `Development` / `Live` / `Production` all have real secrets inline.

## Build & run
```powershell
dotnet build DAKKN.slnx
dotnet run --project DAKKN.MVC\DAKKN.Appearence.csproj    # http://localhost:5218
dotnet watch run --project DAKKN.MVC\DAKKN.Appearence.csproj
```

## Migrations
Run from `DAKKN.Persistence`, referencing MVC as startup:
```powershell
dotnet ef migrations add <Name> --project DAKKN.Persistence --startup-project DAKKN.MVC\DAKKN.Appearence.csproj
dotnet ef database update --project DAKKN.Persistence --startup-project DAKKN.MVC\DAKKN.Appearence.csproj
```

## Architecture rules
- Strict dependency: `Domain ← Application ← Infrastructure / Persistence ← MVC`. No upward references.
- All business logic in `DAKKN.Application`. MVC controllers only call `_mediator.Send()`.
- CQRS triad per operation: `*Command.cs` / `*Query.cs` + `*Handler.cs` + `*Validator.cs` in `Features/{Domain}/Commands|Queries/{OperationName}/`.
- MediatR pipeline order: UnhandledException → Validation → Performance → Logging.

## Do's and don'ts
- **Use** `[RoleAuthorize]` not `[Authorize]`.
- **Use** `IUnitOfWork` (via `_unitOfWork.GetRepository<T>()` + `_unitOfWork.SaveChangesAsync()`), never inject `DbContext` directly.
- **Use** only 4 custom exceptions: `ValidationException`, `BadRequestException`, `NotFoundException`, `UnAuthorizedException`.
- **Use** `IStringLocalizer<Messages>` + `LocalizationKeys` for all user strings. Never hard-code Arabic/English in C#.
- **Use** `ApiResponse<TData>.Ok()` / `.Error()` for all API returns. Never raw `IActionResult`.
- **Soft-delete**: call entity `MarkAsDeleted(userId)` instead of repository `Delete()`.
- **Uploads**: `IImageValidator` handles validation (max 10 MB, specific extensions). Upload place is an int mapped via `UploadPaths.GetPath(place)`.

## Dual auth
Policy scheme `JWT_OR_COOKIE` — API paths (`/api/`) use JWT Bearer, MVC pages use Identity cookies.

## Localization
- JSON files in `DAKKN.Application/Localization/Resources/messages.{ar,en}.json`.
- `JsonLocalizationProvider.Initialize(contentRootPath)` called in `Program.cs` before anything else.
- Culture: `ar` (default), `en`. Order: QueryString → Cookie → Header.

## Key paths
| Path | Purpose |
|------|---------|
| `DAKKN.Application/Features/` | All CQRS handlers |
| `DAKKN.Application/Localization/LocalizationKeys.cs` | String key definitions |
| `DAKKN.Domain/Entities/` | Domain entities (extend `BaseEntity<Guid>`) |
| `DAKKN.MVC/Controllers/APIs/V1/` | API controllers (extend `BaseApiController`) |
| `DAKKN.MVC/Controllers/` | MVC controllers (extend `Controller`, use `[RoleAuthorize]`) |
| `DAKKN.Persistence/Configuration/` | EF entity configurations |
| `DAKKN.Persistence/Migrations/` | EF migrations |
| `DAKKN.MVC/wwwroot/uploads/` | Uploaded files destination |

## Launch profiles
| Profile | URL | Notes |
|---------|-----|-------|
| `http` | `http://localhost:5218` | Default, with hot reload |
| `https` | `https://localhost:7036` | Also http on 5218 |
