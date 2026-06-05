# .NET & Clean Architecture Rules

## 1. Architectural Layers & Responsibilities
- **DAKKN.Domain**: Pure domain logic. Entities, Value Objects, Domain Events, and Repository Interfaces. **Zero dependencies**.
- **DAKKN.Application**: Business logic orchestration. DTOs, Mappers, Use Cases (MediatR Handlers), and Application Interfaces. Depends only on `Domain`.
- **DAKKN.Infrastructure**: External service implementations (Email, File Storage). Depends on `Application`.
- **DAKKN.Persistence**: Data access implementation (EF Core). Depends on `Application` and `Domain`.
- **DAKKN.MVC (Presentation)**: UI and API. Orchestrates all layers. **Thin Controllers only**.

## 2. Presentation Layer & UI (DAKKN.MVC)
This project uses **Tailwind CSS** and **Stitch** for design-to-code workflows.

### 2.1 UI Component Patterns
- **Partial Views (`_*.cshtml`)**: Use for reusable markup without its own logic (e.g., `_ProductCard.cshtml`, `_Navbar.cshtml`). Place in `Views/Shared/Partials/`.
- **ViewComponents**: Use for complex UI widgets that require their own data fetching or logic (e.g., `CartSummary`, `UserProfile`). Classes must reside in a `ViewComponents/` folder.
- **Layouts**: Core layouts (e.g., `_LayoutLanding.cshtml`) must define the base document structure, Tailwind configuration, and global JS/CSS.

### 2.2 UI Sourcing & Design from Stitch
- **Sourcing Mandate**: All UI structures and designs must be sourced exclusively from the **Stitch MCP server** or derived from **custom scripts provided by the user**.
- **Design Tokens**: Map Stitch design tokens (colors, spacing, typography) directly to `wwwroot/js/tailwind.config.js`.
- **Screen Integration**: When implementing a screen:
    1. Fetch design context using Stitch MCP tools (e.g., `get_screen`, `generate_screen_from_text`) or follow the output of the user's custom script.
    2. Extract structural HTML into Razor Views.
    3. Apply Tailwind classes to match the design exactly.
    4. Refactor repeating elements into Partial Views immediately.

### 2.3 Styling with Tailwind CSS
- **Utility-First**: Use Tailwind utility classes exclusively. Avoid custom CSS unless for complex animations.
- **No Inline Styles**: Never use `style="..."` for design properties defined in Tailwind.
- **Responsiveness**: Always use Tailwind's responsive prefixes (`sm:`, `md:`, `lg:`) to ensure mobile-first design.
- **Interactive UI**: Use Tailwind's state variants (`hover:`, `focus:`, `group-hover:`, `dark:`).

### 2.4 Localization (i18n)
- **JSON-Based**: Use the existing JS-based i18n system in `wwwroot/js/i18n/`.
- **Implementation**: 
    - Every user-facing string must have a key in `en.json` and `ar.json`.
    - Use `data-i18n="key_name"` on HTML elements.
    - For dynamic content in JS, use the `currentTranslations` object.

### 2.5 JavaScript & Client-Side Logic
- **`landing.js`**: Central script for i18n, scroll animations, and slider engines.
- **Separation**: Keep DOM manipulation for UI enhancements (animations, toggles) separate from business logic (handled via Controller actions).
- **Initial Rendering**: Prefer server-side Razor rendering for initial page loads. Use JS for real-time interactivity.

## 3. General Development Conventions
- **Framework**: Use **.NET 8**.
- **Naming**: 
    - **PascalCase** for C# Classes, Methods, and Properties.
    - **camelCase** for JS Variables and Functions.
    - **kebab-case** for HTML `id` and `data-*` attributes.
- **Primary Constructors**: Use primary constructors for dependency injection in classes (e.g., `public class MyService(ILogger logger)`).
- **Nullable Context**: `Nullable` must be `enabled`. Fix all nullable warnings; do not use `!`.
- **Async/Await**: Always use `async` and `await` for I/O bound operations. Avoid `.Result`.

## 4. Data & Persistence
- **EF Core**: Use for all data access. No direct SQL.
- **Configurations**: Use `IEntityTypeConfiguration<T>` in the `Persistence` layer to keep Domain entities clean.
- **Repositories**: Follow the Repository pattern for data access. One repository per Aggregate Root.

## 5. Dependency Injection
- Each project must have a `DependencyInjection.cs` file with `IServiceCollection` extension methods.
- **Thin Controllers**: Controllers must only handle routing and model binding. All logic must reside in the Application layer.
