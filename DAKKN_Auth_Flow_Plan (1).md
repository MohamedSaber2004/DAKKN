# DAKKN Auth Flow — UI Implementation Plan (DAKKN.MVC)

> **Scope:** Presentation layer only — `DAKKN.MVC`  
> **UI Source:** Stitch Project "Remix of Direct Connect Marketplace" (ID: 8739468593735996218)  
> **Framework:** .NET 8 MVC · Tailwind CSS · i18n  
> **Backend:** Mock data for now — real Application/Persistence wiring deferred  
> **Date:** June 2026

---

## Table of Contents

1. [Overview & Current Scope](#1-overview--current-scope)
2. [Stitch Screens Mapping](#2-stitch-screens-mapping)
3. [Mock Data Strategy](#3-mock-data-strategy)
4. [Controller — `AuthController.cs`](#4-controller--authcontrollercs)
5. [ViewModels](#5-viewmodels)
6. [Views](#6-views)
7. [Shared Partials](#7-shared-partials)
8. [Layout — `_LayoutAuth.cshtml`](#8-layout--_layoutauthcshtml)
9. [Tailwind Design Tokens](#9-tailwind-design-tokens)
10. [JavaScript](#10-javascript)
11. [Localization (i18n)](#11-localization-i18n)
12. [File & Folder Structure](#12-file--folder-structure)
13. [Implementation Checklist](#13-implementation-checklist)

---

## 1. Overview & Current Scope

### What this plan covers RIGHT NOW
This plan is scoped to building the full auth UI inside `DAKKN.MVC` with **mock data only**.
No real database, no email, no JWT — just the views, navigation flow, and visual fidelity
against the Stitch designs. The goal is a fully clickable auth experience that can be
handed off to backend wiring in the next phase.

### Flows Covered

| Flow | Screens | Views |
|---|---|---|
| **Login** | Screens 1 & 2 | `Login.cshtml` |
| **Register** | Screens 1 & 2 (tab toggle) | `Register.cshtml` |
| **OTP Verification** | Screen 3 | `Otp.cshtml` (shared) |
| **Forgot Password — Step 1/3** | Screen 5 | `ForgotPassword.cshtml` |
| **Forgot Password — Step 2/3** | Screen 3 | `Otp.cshtml` (reused, purpose-aware) |
| **Forgot Password — Step 3/3** | Screen 4 | `ResetPassword.cshtml` |

### What is deferred to later phases
- `DAKKN.Domain` entities and interfaces — deferred
- `DAKKN.Application` commands/handlers/validators — deferred
- `DAKKN.Infrastructure` JWT, email, password hashing — deferred
- `DAKKN.Persistence` EF Core, repositories, migrations — deferred

> When those layers are ready, the controller's mock logic is replaced with
> `await _sender.Send(command)` calls. The views and partials need zero changes.

---

## 2. Stitch Screens Mapping

### Stitch Integration Steps (per DOTNET.md §2.2)
For each screen below:
1. Call `get_screen` on the Stitch MCP server with the screen ID.
2. Extract the HTML structure into the corresponding Razor `.cshtml` file.
3. Map every color, spacing, and font token → `wwwroot/js/tailwind.config.js`.
4. Any element that repeats across screens → extract immediately to a `Partials/_*.cshtml`.

---

### Screen 1 & 2 — Login (Community Driven)
- **IDs:** `c57ea1d0287c409499c25765c2303b05` / `2542b2655a13496dba11df7e53be8ee3`
- **Notes:** Two variants — extract both. Pick the dark-themed one as primary. The second
  variant drives the Register tab (same card, toggled form content).
- **Razor Views:** `Views/Auth/Login.cshtml` · `Views/Auth/Register.cshtml`
- **Shared wrapper:** `Views/Shared/Partials/_AuthCard.cshtml`

### Screen 3 — OTP (تأكيد الرمز - داكن)
- **ID:** `d39e784e635d4958be142ceb71d8c4bf`
- **Notes:** 6-box OTP input in RTL context. Auto-advance between boxes. Resend link.
  A hidden `Purpose` field tells the view whether it's post-Register or post-ForgotPassword.
- **Razor View:** `Views/Auth/Otp.cshtml`

### Screen 4 — Forgot Password Step 3/3 (نسيت كلمة المرور 3/3)
- **ID:** `700d811e07ab4f5489bd3c099c5b1b48`
- **Notes:** Two password fields (New + Confirm). Show/hide toggle on each.
- **Razor View:** `Views/Auth/ResetPassword.cshtml`

### Screen 5 — Forgot Password Step 1/3 (نسيت كلمة المرور 1/3)
- **ID:** `c45299961a474f049421d54b6cb1f607`
- **Notes:** Single email input. Back link to Login.
- **Razor View:** `Views/Auth/ForgotPassword.cshtml`

---

## 3. Mock Data Strategy

All backend operations are faked inside the controller using in-memory static state.
No service interfaces, no DI beyond what MVC provides by default.

### 3.1 Static Mock Store

Create `DAKKN.MVC/Mock/AuthMockStore.cs`:

```csharp
/// <summary>
/// Temporary in-memory store for UI prototyping.
/// Replace with real Application layer commands when ready.
/// </summary>
public static class AuthMockStore
{
    // Seed one known user so login works out of the box
    public static readonly List<MockUser> Users =
    [
        new MockUser
        {
            Id          = Guid.NewGuid(),
            FullName    = "Ahmed Hassan",
            Email       = "ahmed@dakkn.com",
            Password    = "Password123",   // plain text — mock only
            IsVerified  = true
        }
    ];

    // Last generated OTP per email (plain, no hashing in mock)
    public static readonly Dictionary<string, string> PendingOtps = new();

    // Emails that have passed OTP and are cleared to reset password
    public static readonly HashSet<string> ResetApproved = new();

    public static MockUser? FindByEmail(string email) =>
        Users.FirstOrDefault(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

    public static string IssueOtp(string email)
    {
        var code = Random.Shared.Next(100000, 999999).ToString();
        PendingOtps[email.ToLower()] = code;
        return code;   // In real flow this gets emailed; here just stored
    }

    public static bool ValidateOtp(string email, string code)
    {
        var key = email.ToLower();
        return PendingOtps.TryGetValue(key, out var stored) && stored == code;
    }
}

public class MockUser
{
    public Guid   Id        { get; set; }
    public string FullName  { get; set; } = string.Empty;
    public string Email     { get; set; } = string.Empty;
    public string Password  { get; set; } = string.Empty;
    public bool   IsVerified { get; set; }
}
```

### 3.2 Mock OTP for Development
During UI prototyping, the OTP is printed to the **browser console** via a
`ViewBag.DevOtp` value so the developer can complete the flow without email.
This flag is only set in `Development` environment:

```csharp
// In controller, after IssueOtp():
if (_env.IsDevelopment())
    ViewBag.DevOtp = AuthMockStore.IssueOtp(email);
```

In the view, add a dev-only banner:
```razor
@if (ViewBag.DevOtp is string devOtp)
{
    <div class="bg-yellow-500/20 border border-yellow-400 text-yellow-300
                rounded-lg px-4 py-2 text-sm text-center mb-4 font-mono">
        DEV — OTP: <strong>@devOtp</strong>
    </div>
}
```

---

## 4. Controller — `AuthController.cs`

Location: `DAKKN.MVC/Controllers/AuthController.cs`

The controller is thin and will stay thin. Right now it talks to `AuthMockStore`.
Later it will talk to `ISender` (MediatR). The action signatures and redirect logic
**do not change** between phases.

```csharp
[Route("auth")]
public class AuthController(IWebHostEnvironment env) : Controller
{
    // ──────────────────────────────────────────────
    // LOGIN
    // ──────────────────────────────────────────────

    [HttpGet("login")]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost("login")]
    public IActionResult Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = AuthMockStore.FindByEmail(vm.Email);

        if (user is null || user.Password != vm.Password)
        {
            ModelState.AddModelError(string.Empty, "invalid_credentials");
            return View(vm);
        }

        if (!user.IsVerified)
        {
            // Kick to OTP to finish verification
            TempData["OtpEmail"]   = vm.Email;
            TempData["OtpPurpose"] = "VerifyEmail";
            return RedirectToAction(nameof(Otp));
        }

        // TODO (Phase 2): issue JWT / cookie here
        TempData["SuccessMessage"] = "login_success";
        return RedirectToAction("Index", "Home");
    }

    // ──────────────────────────────────────────────
    // REGISTER
    // ──────────────────────────────────────────────

    [HttpGet("register")]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost("register")]
    public IActionResult Register(RegisterViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        if (AuthMockStore.FindByEmail(vm.Email) is not null)
        {
            ModelState.AddModelError(nameof(vm.Email), "email_taken");
            return View(vm);
        }

        AuthMockStore.Users.Add(new MockUser
        {
            Id       = Guid.NewGuid(),
            FullName = vm.FullName,
            Email    = vm.Email,
            Password = vm.Password,
            IsVerified = false
        });

        TempData["OtpEmail"]   = vm.Email;
        TempData["OtpPurpose"] = "VerifyEmail";

        if (env.IsDevelopment())
            TempData["DevOtp"] = AuthMockStore.IssueOtp(vm.Email);
        else
            AuthMockStore.IssueOtp(vm.Email);

        return RedirectToAction(nameof(Otp));
    }

    // ──────────────────────────────────────────────
    // OTP  (shared — VerifyEmail + ForgotPassword)
    // ──────────────────────────────────────────────

    [HttpGet("otp")]
    public IActionResult Otp()
    {
        var email   = TempData.Peek("OtpEmail") as string;
        var purpose = TempData.Peek("OtpPurpose") as string;

        if (string.IsNullOrEmpty(email))
            return RedirectToAction(nameof(Login));

        return View(new OtpViewModel
        {
            Email   = email,
            Purpose = purpose ?? "VerifyEmail"
        });
    }

    [HttpPost("otp")]
    public IActionResult Otp(OtpViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        if (!AuthMockStore.ValidateOtp(vm.Email, vm.Code))
        {
            ModelState.AddModelError(string.Empty, "invalid_otp");
            return View(vm);
        }

        if (vm.Purpose == "VerifyEmail")
        {
            var user = AuthMockStore.FindByEmail(vm.Email);
            if (user is not null) user.IsVerified = true;

            TempData["SuccessMessage"] = "email_verified";
            return RedirectToAction(nameof(Login));
        }

        // ForgotPassword — advance to step 3
        AuthMockStore.ResetApproved.Add(vm.Email.ToLower());
        TempData["ResetEmail"] = vm.Email;
        return RedirectToAction(nameof(ResetPassword));
    }

    [HttpPost("resend-otp")]
    public IActionResult ResendOtp(string email, string purpose)
    {
        TempData["OtpEmail"]   = email;
        TempData["OtpPurpose"] = purpose;

        if (env.IsDevelopment())
            TempData["DevOtp"] = AuthMockStore.IssueOtp(email);
        else
            AuthMockStore.IssueOtp(email);

        return RedirectToAction(nameof(Otp));
    }

    // ──────────────────────────────────────────────
    // FORGOT PASSWORD  Step 1/3
    // ──────────────────────────────────────────────

    [HttpGet("forgot-password")]
    public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

    [HttpPost("forgot-password")]
    public IActionResult ForgotPassword(ForgotPasswordViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        // Always show success (anti-enumeration) — but only issue OTP if user exists
        var user = AuthMockStore.FindByEmail(vm.Email);
        if (user is not null)
        {
            TempData["OtpEmail"]   = vm.Email;
            TempData["OtpPurpose"] = "ForgotPassword";

            if (env.IsDevelopment())
                TempData["DevOtp"] = AuthMockStore.IssueOtp(vm.Email);
            else
                AuthMockStore.IssueOtp(vm.Email);
        }
        else
        {
            // Fake TempData so the redirect still lands on OTP
            // (UI shows "check your email" regardless)
            TempData["OtpEmail"]   = vm.Email;
            TempData["OtpPurpose"] = "ForgotPassword";
        }

        return RedirectToAction(nameof(Otp));
    }

    // ──────────────────────────────────────────────
    // RESET PASSWORD  Step 3/3
    // ──────────────────────────────────────────────

    [HttpGet("reset-password")]
    public IActionResult ResetPassword()
    {
        var email = TempData.Peek("ResetEmail") as string;

        // Guard: cannot reach this page without passing OTP
        if (string.IsNullOrEmpty(email) ||
            !AuthMockStore.ResetApproved.Contains(email.ToLower()))
            return RedirectToAction(nameof(ForgotPassword));

        return View(new ResetPasswordViewModel { Email = email });
    }

    [HttpPost("reset-password")]
    public IActionResult ResetPassword(ResetPasswordViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        if (!AuthMockStore.ResetApproved.Contains(vm.Email.ToLower()))
            return RedirectToAction(nameof(ForgotPassword));

        var user = AuthMockStore.FindByEmail(vm.Email);
        if (user is not null) user.Password = vm.NewPassword;

        AuthMockStore.ResetApproved.Remove(vm.Email.ToLower());
        TempData["SuccessMessage"] = "password_reset";
        return RedirectToAction(nameof(Login));
    }

    // ──────────────────────────────────────────────
    // LOGOUT
    // ──────────────────────────────────────────────

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // TODO (Phase 2): clear cookie/JWT
        return RedirectToAction(nameof(Login));
    }
}
```

---

## 5. ViewModels

Location: `DAKKN.MVC/ViewModels/Auth/`

These stay in MVC permanently (they are the binding models for the views).
In Phase 2 the controller will map these to Application-layer Commands;
the ViewModels themselves don't change.

### `LoginViewModel.cs`
```csharp
public class LoginViewModel
{
    [Required(ErrorMessage = "email_required")]
    [EmailAddress(ErrorMessage = "email_invalid")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "password_required")]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}
```

### `RegisterViewModel.cs`
```csharp
public class RegisterViewModel
{
    [Required(ErrorMessage = "name_required")]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "email_required")]
    [EmailAddress(ErrorMessage = "email_invalid")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "password_required")]
    [MinLength(8, ErrorMessage = "password_min_length")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "confirm_required")]
    [Compare(nameof(Password), ErrorMessage = "passwords_mismatch")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
```

### `OtpViewModel.cs`
```csharp
public class OtpViewModel
{
    public string Email   { get; set; } = string.Empty;
    public string Purpose { get; set; } = "VerifyEmail"; // "VerifyEmail" | "ForgotPassword"

    // Individual digit inputs — joined in the Code property
    [Required] public string Digit1 { get; set; } = string.Empty;
    [Required] public string Digit2 { get; set; } = string.Empty;
    [Required] public string Digit3 { get; set; } = string.Empty;
    [Required] public string Digit4 { get; set; } = string.Empty;
    [Required] public string Digit5 { get; set; } = string.Empty;
    [Required] public string Digit6 { get; set; } = string.Empty;

    public string Code => Digit1 + Digit2 + Digit3 + Digit4 + Digit5 + Digit6;
}
```

### `ForgotPasswordViewModel.cs`
```csharp
public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "email_required")]
    [EmailAddress(ErrorMessage = "email_invalid")]
    public string Email { get; set; } = string.Empty;
}
```

### `ResetPasswordViewModel.cs`
```csharp
public class ResetPasswordViewModel
{
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "password_required")]
    [MinLength(8, ErrorMessage = "password_min_length")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "confirm_required")]
    [Compare(nameof(NewPassword), ErrorMessage = "passwords_mismatch")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
```

> **Error messages** use i18n keys (e.g. `"email_required"`) — the view resolves them
> via the `currentTranslations` object, not hardcoded strings.

---

## 6. Views

### 6.1 `Login.cshtml`

```razor
@model LoginViewModel
@{
    Layout = "_LayoutAuth";
    ViewData["Title"] = "login";
}

@await Html.PartialAsync("Partials/_AuthAlert")

<h1 class="text-2xl font-bold text-center mb-1 text-[var(--color-text-primary)]"
    data-i18n="auth.login.title"></h1>
<p class="text-sm text-center text-[var(--color-text-muted)] mb-8"
   data-i18n="auth.login.subtitle"></p>

@* Dev OTP hint *@
@if (TempData["DevOtp"] is string devOtp)
{
    <div class="bg-yellow-500/20 border border-yellow-400 text-yellow-300
                rounded-lg px-4 py-2 text-sm text-center mb-4 font-mono">
        DEV — OTP: <strong>@devOtp</strong>
    </div>
}

<form method="post" asp-action="Login" asp-controller="Auth" novalidate>
    @Html.AntiForgeryToken()

    <div class="space-y-4">
        <div>
            <label asp-for="Email" class="block text-sm text-[var(--color-text-muted)] mb-1"
                   data-i18n="auth.login.email"></label>
            <input asp-for="Email" type="email" autocomplete="email"
                   class="w-full px-4 py-3 rounded-xl border border-[var(--color-border)]
                          bg-[var(--color-input-bg)] text-[var(--color-text-primary)]
                          placeholder-[var(--color-text-muted)]
                          focus:border-[var(--color-primary)] focus:outline-none
                          transition-colors" />
            <span asp-validation-for="Email" class="text-xs text-red-400 mt-1 block"></span>
        </div>

        @await Html.PartialAsync("Partials/_PasswordInput",
            new PasswordInputModel { For = "Password", LabelKey = "auth.login.password" })

        <div class="flex items-center justify-between text-sm">
            <label class="flex items-center gap-2 text-[var(--color-text-muted)] cursor-pointer">
                <input asp-for="RememberMe" type="checkbox"
                       class="w-4 h-4 rounded border-[var(--color-border)]
                              accent-[var(--color-primary)]" />
                <span data-i18n="auth.login.remember"></span>
            </label>
            <a asp-action="ForgotPassword" asp-controller="Auth"
               class="text-[var(--color-primary)] hover:underline"
               data-i18n="auth.login.forgot"></a>
        </div>

        <div asp-validation-summary="ModelOnly" class="text-xs text-red-400"></div>

        <button type="submit"
                class="w-full py-3 rounded-xl bg-[var(--color-primary)]
                       text-white font-semibold hover:bg-[var(--color-primary-hover)]
                       active:scale-[0.98] transition-all"
                data-i18n="auth.login.submit">
        </button>
    </div>
</form>

<p class="mt-6 text-center text-sm text-[var(--color-text-muted)]">
    <span data-i18n="auth.login.no_account"></span>
    <a asp-action="Register" asp-controller="Auth"
       class="text-[var(--color-primary)] hover:underline ms-1"
       data-i18n="auth.login.register"></a>
</p>
```

---

### 6.2 `Register.cshtml`

```razor
@model RegisterViewModel
@{
    Layout = "_LayoutAuth";
    ViewData["Title"] = "register";
}

@await Html.PartialAsync("Partials/_AuthAlert")

<h1 class="text-2xl font-bold text-center mb-1 text-[var(--color-text-primary)]"
    data-i18n="auth.register.title"></h1>
<p class="text-sm text-center text-[var(--color-text-muted)] mb-8"
   data-i18n="auth.register.subtitle"></p>

<form method="post" asp-action="Register" asp-controller="Auth" novalidate>
    @Html.AntiForgeryToken()

    <div class="space-y-4">
        <div>
            <label asp-for="FullName" class="block text-sm text-[var(--color-text-muted)] mb-1"
                   data-i18n="auth.register.name"></label>
            <input asp-for="FullName" type="text" autocomplete="name"
                   class="w-full px-4 py-3 rounded-xl border border-[var(--color-border)]
                          bg-[var(--color-input-bg)] text-[var(--color-text-primary)]
                          focus:border-[var(--color-primary)] focus:outline-none
                          transition-colors" />
            <span asp-validation-for="FullName" class="text-xs text-red-400 mt-1 block"></span>
        </div>

        <div>
            <label asp-for="Email" class="block text-sm text-[var(--color-text-muted)] mb-1"
                   data-i18n="auth.login.email"></label>
            <input asp-for="Email" type="email" autocomplete="email"
                   class="w-full px-4 py-3 rounded-xl border border-[var(--color-border)]
                          bg-[var(--color-input-bg)] text-[var(--color-text-primary)]
                          focus:border-[var(--color-primary)] focus:outline-none
                          transition-colors" />
            <span asp-validation-for="Email" class="text-xs text-red-400 mt-1 block"></span>
        </div>

        @await Html.PartialAsync("Partials/_PasswordInput",
            new PasswordInputModel { For = "Password", LabelKey = "auth.login.password" })

        @await Html.PartialAsync("Partials/_PasswordInput",
            new PasswordInputModel { For = "ConfirmPassword", LabelKey = "auth.register.confirm_password" })

        <div asp-validation-summary="ModelOnly" class="text-xs text-red-400"></div>

        <button type="submit"
                class="w-full py-3 rounded-xl bg-[var(--color-primary)]
                       text-white font-semibold hover:bg-[var(--color-primary-hover)]
                       active:scale-[0.98] transition-all"
                data-i18n="auth.register.submit">
        </button>
    </div>
</form>

<p class="mt-6 text-center text-sm text-[var(--color-text-muted)]">
    <span data-i18n="auth.register.has_account"></span>
    <a asp-action="Login" asp-controller="Auth"
       class="text-[var(--color-primary)] hover:underline ms-1"
       data-i18n="auth.register.login"></a>
</p>
```

---

### 6.3 `Otp.cshtml`

```razor
@model OtpViewModel
@{
    Layout = "_LayoutAuth";
    ViewData["Title"] = "otp";
    bool isForgot = Model.Purpose == "ForgotPassword";
}

@await Html.PartialAsync("Partials/_AuthAlert")

@* Dev OTP hint *@
@if (TempData["DevOtp"] is string devOtp)
{
    <div class="bg-yellow-500/20 border border-yellow-400 text-yellow-300
                rounded-lg px-4 py-2 text-sm text-center mb-4 font-mono">
        DEV — OTP: <strong>@devOtp</strong>
    </div>
}

<h1 class="text-2xl font-bold text-center mb-1 text-[var(--color-text-primary)]"
    data-i18n="otp.title"></h1>
<p class="text-sm text-center text-[var(--color-text-muted)] mb-2"
   data-i18n="otp.subtitle"></p>
<p class="text-sm text-center font-medium text-[var(--color-primary)] mb-8">
    @Model.Email
</p>

<form method="post" asp-action="Otp" asp-controller="Auth" novalidate>
    @Html.AntiForgeryToken()
    <input type="hidden" asp-for="Email" />
    <input type="hidden" asp-for="Purpose" />

    @await Html.PartialAsync("Partials/_OtpInput", Model)

    <div asp-validation-summary="All" class="text-xs text-red-400 text-center mt-3"></div>

    <button type="submit"
            class="w-full mt-6 py-3 rounded-xl bg-[var(--color-primary)]
                   text-white font-semibold hover:bg-[var(--color-primary-hover)]
                   active:scale-[0.98] transition-all"
            data-i18n="otp.submit">
    </button>
</form>

<div class="mt-5 text-center text-sm text-[var(--color-text-muted)]">
    <span data-i18n="otp.no_code"></span>
    <form method="post" asp-action="ResendOtp" asp-controller="Auth" class="inline">
        @Html.AntiForgeryToken()
        <input type="hidden" name="email"   value="@Model.Email" />
        <input type="hidden" name="purpose" value="@Model.Purpose" />
        <button type="submit"
                class="text-[var(--color-primary)] underline hover:no-underline ms-1"
                data-i18n="otp.resend">
        </button>
    </form>
</div>
```

---

### 6.4 `ForgotPassword.cshtml`

```razor
@model ForgotPasswordViewModel
@{
    Layout = "_LayoutAuth";
    ViewData["Title"] = "forgot_password";
}

@await Html.PartialAsync("Partials/_AuthAlert")

<h1 class="text-2xl font-bold text-center mb-1 text-[var(--color-text-primary)]"
    data-i18n="forgot.title"></h1>
<p class="text-sm text-center text-[var(--color-text-muted)] mb-8"
   data-i18n="forgot.subtitle"></p>

<form method="post" asp-action="ForgotPassword" asp-controller="Auth" novalidate>
    @Html.AntiForgeryToken()

    <div class="space-y-4">
        <div>
            <label asp-for="Email" class="block text-sm text-[var(--color-text-muted)] mb-1"
                   data-i18n="auth.login.email"></label>
            <input asp-for="Email" type="email" autocomplete="email"
                   class="w-full px-4 py-3 rounded-xl border border-[var(--color-border)]
                          bg-[var(--color-input-bg)] text-[var(--color-text-primary)]
                          focus:border-[var(--color-primary)] focus:outline-none
                          transition-colors" />
            <span asp-validation-for="Email" class="text-xs text-red-400 mt-1 block"></span>
        </div>

        <button type="submit"
                class="w-full py-3 rounded-xl bg-[var(--color-primary)]
                       text-white font-semibold hover:bg-[var(--color-primary-hover)]
                       active:scale-[0.98] transition-all"
                data-i18n="forgot.submit">
        </button>
    </div>
</form>

<p class="mt-6 text-center text-sm">
    <a asp-action="Login" asp-controller="Auth"
       class="text-[var(--color-primary)] hover:underline"
       data-i18n="forgot.back"></a>
</p>
```

---

### 6.5 `ResetPassword.cshtml`

```razor
@model ResetPasswordViewModel
@{
    Layout = "_LayoutAuth";
    ViewData["Title"] = "reset_password";
}

@await Html.PartialAsync("Partials/_AuthAlert")

<h1 class="text-2xl font-bold text-center mb-1 text-[var(--color-text-primary)]"
    data-i18n="reset.title"></h1>
<p class="text-sm text-center text-[var(--color-text-muted)] mb-8"
   data-i18n="reset.subtitle"></p>

<form method="post" asp-action="ResetPassword" asp-controller="Auth" novalidate>
    @Html.AntiForgeryToken()
    <input type="hidden" asp-for="Email" />

    <div class="space-y-4">
        @await Html.PartialAsync("Partials/_PasswordInput",
            new PasswordInputModel { For = "NewPassword", LabelKey = "reset.new_password" })

        @await Html.PartialAsync("Partials/_PasswordInput",
            new PasswordInputModel { For = "ConfirmPassword", LabelKey = "reset.confirm_password" })

        <div asp-validation-summary="ModelOnly" class="text-xs text-red-400"></div>

        <button type="submit"
                class="w-full py-3 rounded-xl bg-[var(--color-primary)]
                       text-white font-semibold hover:bg-[var(--color-primary-hover)]
                       active:scale-[0.98] transition-all"
                data-i18n="reset.submit">
        </button>
    </div>
</form>
```

---

## 7. Shared Partials

Location: `Views/Shared/Partials/`

---

### `_AuthCard.cshtml`
Wraps all auth pages. Used from `_LayoutAuth.cshtml`.

```razor
<div class="min-h-screen flex items-center justify-center
            bg-[var(--color-bg-base)] px-4">
    <div class="w-full max-w-md bg-[var(--color-surface)]
                rounded-2xl p-8 shadow-2xl">

        {{-- Logo --}}
        <div class="mb-8 text-center">
            <img src="~/images/logo.svg" alt="DAKKN"
                 class="h-10 mx-auto" />
        </div>

        @RenderBody()
    </div>
</div>
```

> Note: `_AuthCard.cshtml` is used as a Layout-level wrapper (via `_LayoutAuth`), not
> called with `Html.PartialAsync`. It frames the entire page.

---

### `_OtpInput.cshtml`
Model: `OtpViewModel`

```razor
@model OtpViewModel

<div class="flex gap-3 justify-center" dir="ltr" id="otp-box-group">
    @foreach (var i in Enumerable.Range(1, 6))
    {
        <input type="text"
               name="Digit@(i)"
               id="otp-digit-@(i)"
               maxlength="1"
               inputmode="numeric"
               pattern="[0-9]"
               autocomplete="@(i == 1 ? "one-time-code" : "off")"
               class="w-12 h-14 text-center text-xl font-bold rounded-xl
                      border-2 border-[var(--color-border)]
                      bg-[var(--color-input-bg)]
                      text-[var(--color-text-primary)]
                      focus:border-[var(--color-primary)]
                      focus:outline-none focus:ring-0
                      transition-colors caret-transparent" />
    }
</div>
```

---

### `_PasswordInput.cshtml`
Model: `PasswordInputModel` (simple helper class in `ViewModels/Shared/`)

```csharp
// DAKKN.MVC/ViewModels/Shared/PasswordInputModel.cs
public class PasswordInputModel
{
    public string For      { get; set; } = string.Empty;  // e.g. "Password"
    public string LabelKey { get; set; } = string.Empty;  // i18n key
}
```

```razor
@model PasswordInputModel

<div class="relative group">
    <label for="@Model.For"
           class="block text-sm text-[var(--color-text-muted)] mb-1"
           data-i18n="@Model.LabelKey"></label>

    <input type="password"
           id="@Model.For"
           name="@Model.For"
           autocomplete="@(Model.For == "Password" ? "current-password" : "new-password")"
           class="w-full px-4 py-3 pe-12 rounded-xl
                  border border-[var(--color-border)]
                  bg-[var(--color-input-bg)]
                  text-[var(--color-text-primary)]
                  focus:border-[var(--color-primary)] focus:outline-none
                  transition-colors"
           data-pw-field />

    <button type="button"
            class="absolute end-3 top-[38px]
                   text-[var(--color-text-muted)] hover:text-[var(--color-text-primary)]
                   transition-colors"
            data-pw-toggle
            tabindex="-1"
            aria-label="toggle password visibility">
        {{-- Eye icon — swap via JS --}}
        <svg data-eye-open class="w-5 h-5" fill="none" stroke="currentColor"
             viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943
                     9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
        </svg>
        <svg data-eye-closed class="w-5 h-5 hidden" fill="none" stroke="currentColor"
             viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943
                     -9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243
                     M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29
                     M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943
                     9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21" />
        </svg>
    </button>
</div>
```

---

### `_AuthAlert.cshtml`
Reads `TempData["SuccessMessage"]` and `ViewData["ErrorMessage"]`.

```razor
@if (TempData["SuccessMessage"] is string successKey)
{
    <div class="flex items-center gap-3 bg-green-500/15 border border-green-500/40
                text-green-400 rounded-xl px-4 py-3 mb-6 text-sm"
         role="alert">
        <svg class="w-5 h-5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd"
                  d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0
                     00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2
                     2a1 1 0 001.414 0l4-4z"
                  clip-rule="evenodd" />
        </svg>
        <span data-i18n="alert.@successKey"></span>
    </div>
}

@if (ViewData["ErrorMessage"] is string errorKey)
{
    <div class="flex items-center gap-3 bg-red-500/15 border border-red-500/40
                text-red-400 rounded-xl px-4 py-3 mb-6 text-sm"
         role="alert">
        <svg class="w-5 h-5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd"
                  d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0
                     012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z"
                  clip-rule="evenodd" />
        </svg>
        <span data-i18n="alert.@errorKey"></span>
    </div>
}
```

---

## 8. Layout — `_LayoutAuth.cshtml`

Location: `Views/Shared/_LayoutAuth.cshtml`

Dedicated layout for all auth pages — no navbar, no footer, minimal head.

```razor
<!DOCTYPE html>
<html lang="en" dir="ltr" id="html-root">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] — DAKKN</title>

    {{-- Tailwind (CDN play version for dev; swap to build output in prod) --}}
    <script src="https://cdn.tailwindcss.com"></script>
    <script src="~/js/tailwind.config.js"></script>

    <style>
        /* CSS custom properties — populated from tailwind.config.js tokens */
        :root {
            --color-primary:       #6C63FF;
            --color-primary-hover: #5A52E0;
            --color-surface:       #1A1A2E;
            --color-bg-base:       #0F0F1A;
            --color-border:        #2E2E4A;
            --color-input-bg:      #242438;
            --color-text-primary:  #FFFFFF;
            --color-text-muted:    #9090B0;
        }
    </style>
</head>
<body class="bg-[var(--color-bg-base)] text-[var(--color-text-primary)]">

    {{-- Auth Card wrapper --}}
    <div class="min-h-screen flex items-center justify-center px-4">
        <div class="w-full max-w-md bg-[var(--color-surface)]
                    rounded-2xl p-8 shadow-2xl">

            <div class="mb-8 text-center">
                <a asp-action="Login" asp-controller="Auth">
                    <img src="~/images/logo.svg" alt="DAKKN" class="h-10 mx-auto" />
                </a>
            </div>

            @RenderBody()

        </div>
    </div>

    {{-- i18n + shared JS --}}
    <script src="~/js/i18n/en.json" type="application/json" id="i18n-en"></script>
    <script src="~/js/i18n/ar.json" type="application/json" id="i18n-ar"></script>
    <script src="~/js/landing.js"></script>

    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

---

## 9. Tailwind Design Tokens

Location: `wwwroot/js/tailwind.config.js`

> **Important:** The values below are placeholders. Replace every hex with the
> exact values extracted from Stitch via `get_screen` → inspect computed styles.

```javascript
tailwind.config = {
  theme: {
    extend: {
      colors: {
        // ── Replace with exact Stitch values ──
        primary: {
          DEFAULT: '#6C63FF',
          hover:   '#5A52E0',
        },
        surface:       '#1A1A2E',
        'bg-base':     '#0F0F1A',
        border:        '#2E2E4A',
        'input-bg':    '#242438',
        'text-primary':'#FFFFFF',
        'text-muted':  '#9090B0',
      },
      fontFamily: {
        // Replace with Stitch font stack
        sans: ['Inter', 'Cairo', 'sans-serif'],
      },
      borderRadius: {
        xl:   '12px',
        '2xl':'16px',
      },
      boxShadow: {
        auth: '0 25px 50px -12px rgba(0, 0, 0, 0.6)',
      }
    }
  }
}
```

---

## 10. JavaScript

All auth JS lives in `wwwroot/js/landing.js` per DOTNET.md §2.5.
Add these blocks to the existing file under clearly marked sections.

### 10.1 OTP Auto-Advance

```javascript
// ── OTP Box Auto-Advance ─────────────────────────────
(function initOtpBoxes() {
    const boxes = [...document.querySelectorAll('#otp-box-group input')];
    if (!boxes.length) return;

    boxes.forEach((box, idx) => {
        box.addEventListener('input', e => {
            const val = e.target.value.replace(/\D/g, '');
            e.target.value = val.slice(-1);            // keep only last digit
            if (val && idx < boxes.length - 1) boxes[idx + 1].focus();
        });

        box.addEventListener('keydown', e => {
            if (e.key === 'Backspace' && !box.value && idx > 0) {
                boxes[idx - 1].focus();
                boxes[idx - 1].value = '';
            }
            if (e.key === 'ArrowLeft'  && idx > 0) boxes[idx - 1].focus();
            if (e.key === 'ArrowRight' && idx < boxes.length - 1) boxes[idx + 1].focus();
        });

        // Handle paste on first box — distribute digits
        box.addEventListener('paste', e => {
            if (idx !== 0) return;
            const text = (e.clipboardData || window.clipboardData).getData('text');
            const digits = text.replace(/\D/g, '').slice(0, 6);
            digits.split('').forEach((d, i) => { if (boxes[i]) boxes[i].value = d; });
            const last = Math.min(digits.length, boxes.length - 1);
            boxes[last].focus();
            e.preventDefault();
        });
    });

    // Auto-focus first box on load
    boxes[0]?.focus();
})();
```

### 10.2 Password Show/Hide Toggle

```javascript
// ── Password Show / Hide ──────────────────────────────
(function initPasswordToggles() {
    document.querySelectorAll('[data-pw-toggle]').forEach(btn => {
        btn.addEventListener('click', () => {
            const wrapper  = btn.closest('.relative');
            const input    = wrapper?.querySelector('[data-pw-field]');
            const eyeOpen  = btn.querySelector('[data-eye-open]');
            const eyeClosed= btn.querySelector('[data-eye-closed]');
            if (!input) return;

            const isHidden = input.type === 'password';
            input.type = isHidden ? 'text' : 'password';
            eyeOpen?.classList.toggle('hidden', isHidden);
            eyeClosed?.classList.toggle('hidden', !isHidden);
        });
    });
})();
```

### 10.3 OTP Countdown Timer (UI Only)

```javascript
// ── OTP Expiry Countdown (10-minute visual timer) ─────
(function initOtpCountdown() {
    const el = document.getElementById('otp-countdown');
    if (!el) return;

    let remaining = 10 * 60;  // 600 seconds

    const tick = () => {
        const m = String(Math.floor(remaining / 60)).padStart(2, '0');
        const s = String(remaining % 60).padStart(2, '0');
        el.textContent = `${m}:${s}`;

        if (remaining-- <= 0) {
            clearInterval(timer);
            el.textContent = '00:00';
            el.classList.add('text-red-400');
            document.getElementById('otp-resend-section')
                    ?.classList.remove('hidden');
        }
    };

    tick();
    const timer = setInterval(tick, 1000);
})();
```

Add to `Otp.cshtml` view where the timer should appear:
```razor
<p class="text-xs text-center text-[var(--color-text-muted)] mt-3">
    <span data-i18n="otp.expires"></span>
    <span id="otp-countdown" class="font-mono text-[var(--color-primary)] ms-1">10:00</span>
</p>
```

---

## 11. Localization (i18n)

Per DOTNET.md §2.4 — every string must exist in both `en.json` and `ar.json`.
Add these keys to the existing files.

### New keys for `en.json`
```json
{
  "auth.login.title":                   "Welcome Back",
  "auth.login.subtitle":                "Sign in to your account",
  "auth.login.email":                   "Email address",
  "auth.login.password":                "Password",
  "auth.login.remember":                "Remember me",
  "auth.login.forgot":                  "Forgot password?",
  "auth.login.submit":                  "Sign In",
  "auth.login.no_account":              "Don't have an account?",
  "auth.login.register":                "Register",

  "auth.register.title":                "Create Account",
  "auth.register.subtitle":             "Join the DAKKN community",
  "auth.register.name":                 "Full name",
  "auth.register.confirm_password":     "Confirm password",
  "auth.register.submit":               "Create Account",
  "auth.register.has_account":          "Already have an account?",
  "auth.register.login":                "Sign In",

  "otp.title":                          "Verify Your Code",
  "otp.subtitle":                       "Enter the 6-digit code sent to",
  "otp.submit":                         "Verify",
  "otp.no_code":                        "Didn't receive a code?",
  "otp.resend":                         "Resend",
  "otp.expires":                        "Code expires in",

  "forgot.title":                       "Forgot Password",
  "forgot.subtitle":                    "Enter your email to receive a reset code",
  "forgot.submit":                      "Send Reset Code",
  "forgot.back":                        "← Back to Login",

  "reset.title":                        "Set New Password",
  "reset.subtitle":                     "Choose a strong password",
  "reset.new_password":                 "New password",
  "reset.confirm_password":             "Confirm new password",
  "reset.submit":                       "Reset Password",

  "alert.login_success":                "Logged in successfully",
  "alert.email_verified":               "Email verified! You can now log in.",
  "alert.password_reset":               "Password reset successfully. Please log in.",
  "alert.invalid_credentials":          "Invalid email or password.",
  "alert.email_taken":                  "This email is already registered.",
  "alert.invalid_otp":                  "The code is incorrect or expired.",
  "alert.email_required":               "Email is required.",
  "alert.email_invalid":                "Please enter a valid email.",
  "alert.password_required":            "Password is required.",
  "alert.password_min_length":          "Password must be at least 8 characters.",
  "alert.confirm_required":             "Please confirm your password.",
  "alert.passwords_mismatch":           "Passwords do not match.",
  "alert.name_required":                "Full name is required."
}
```

### New keys for `ar.json`
```json
{
  "auth.login.title":                   "مرحباً بعودتك",
  "auth.login.subtitle":                "تسجيل الدخول إلى حسابك",
  "auth.login.email":                   "البريد الإلكتروني",
  "auth.login.password":                "كلمة المرور",
  "auth.login.remember":                "تذكرني",
  "auth.login.forgot":                  "نسيت كلمة المرور؟",
  "auth.login.submit":                  "تسجيل الدخول",
  "auth.login.no_account":              "ليس لديك حساب؟",
  "auth.login.register":                "إنشاء حساب",

  "auth.register.title":                "إنشاء حساب",
  "auth.register.subtitle":             "انضم إلى مجتمع داكن",
  "auth.register.name":                 "الاسم الكامل",
  "auth.register.confirm_password":     "تأكيد كلمة المرور",
  "auth.register.submit":               "إنشاء الحساب",
  "auth.register.has_account":          "لديك حساب بالفعل؟",
  "auth.register.login":                "تسجيل الدخول",

  "otp.title":                          "تأكيد الرمز",
  "otp.subtitle":                       "أدخل الرمز المكون من 6 أرقام المُرسل إلى",
  "otp.submit":                         "تحقق",
  "otp.no_code":                        "لم تستلم الرمز؟",
  "otp.resend":                         "إعادة الإرسال",
  "otp.expires":                        "ينتهي الرمز خلال",

  "forgot.title":                       "نسيت كلمة المرور",
  "forgot.subtitle":                    "أدخل بريدك الإلكتروني لاستلام رمز الاستعادة",
  "forgot.submit":                      "إرسال رمز الاستعادة",
  "forgot.back":                        "→ العودة لتسجيل الدخول",

  "reset.title":                        "تعيين كلمة مرور جديدة",
  "reset.subtitle":                     "اختر كلمة مرور قوية",
  "reset.new_password":                 "كلمة المرور الجديدة",
  "reset.confirm_password":             "تأكيد كلمة المرور",
  "reset.submit":                       "إعادة تعيين كلمة المرور",

  "alert.login_success":                "تم تسجيل الدخول بنجاح",
  "alert.email_verified":               "تم التحقق من البريد! يمكنك تسجيل الدخول الآن.",
  "alert.password_reset":               "تم إعادة تعيين كلمة المرور. يرجى تسجيل الدخول.",
  "alert.invalid_credentials":          "البريد الإلكتروني أو كلمة المرور غير صحيحة.",
  "alert.email_taken":                  "هذا البريد الإلكتروني مسجّل بالفعل.",
  "alert.invalid_otp":                  "الرمز غير صحيح أو منتهي الصلاحية.",
  "alert.email_required":               "البريد الإلكتروني مطلوب.",
  "alert.email_invalid":                "أدخل بريداً إلكترونياً صحيحاً.",
  "alert.password_required":            "كلمة المرور مطلوبة.",
  "alert.password_min_length":          "كلمة المرور يجب أن تكون 8 أحرف على الأقل.",
  "alert.confirm_required":             "يرجى تأكيد كلمة المرور.",
  "alert.passwords_mismatch":           "كلمتا المرور غير متطابقتين.",
  "alert.name_required":                "الاسم الكامل مطلوب."
}
```

---

## 12. File & Folder Structure

Only `DAKKN.MVC` files — nothing else touched in this phase.

```
DAKKN.MVC/
├── Controllers/
│   └── AuthController.cs               ← All 8 actions (Login, Register, Otp, Resend,
│                                           ForgotPassword, ResetPassword, Logout)
│
├── Mock/
│   └── AuthMockStore.cs                ← Static in-memory users + OTP store
│                                           DELETE in Phase 2
│
├── ViewModels/
│   ├── Auth/
│   │   ├── LoginViewModel.cs
│   │   ├── RegisterViewModel.cs
│   │   ├── OtpViewModel.cs
│   │   ├── ForgotPasswordViewModel.cs
│   │   └── ResetPasswordViewModel.cs
│   └── Shared/
│       └── PasswordInputModel.cs       ← Model for _PasswordInput partial
│
├── Views/
│   ├── Auth/
│   │   ├── Login.cshtml
│   │   ├── Register.cshtml
│   │   ├── Otp.cshtml                  ← Shared; Purpose-aware
│   │   ├── ForgotPassword.cshtml
│   │   └── ResetPassword.cshtml
│   └── Shared/
│       ├── _LayoutAuth.cshtml          ← Auth-only layout (no nav/footer)
│       └── Partials/
│           ├── _AuthAlert.cshtml       ← Success / error banners
│           ├── _OtpInput.cshtml        ← 6-box OTP widget
│           └── _PasswordInput.cshtml   ← Password field + show/hide toggle
│
└── wwwroot/
    ├── images/
    │   └── logo.svg                    ← DAKKN logo (from Stitch or assets)
    ├── js/
    │   ├── tailwind.config.js          ← Add auth design tokens (from Stitch)
    │   ├── landing.js                  ← Add §10.1, §10.2, §10.3 blocks
    │   └── i18n/
    │       ├── en.json                 ← Add §11 keys
    │       └── ar.json                 ← Add §11 keys
    └── css/
        └── (no new CSS files — utility-first only)
```

---

## 13. Implementation Checklist

### Mock / Setup
- [ ] Create `Mock/AuthMockStore.cs` with seeded user
- [ ] Add `[Route("auth")]` to `AuthController`

### ViewModels
- [ ] `LoginViewModel`
- [ ] `RegisterViewModel`
- [ ] `OtpViewModel` (with `Code` computed property)
- [ ] `ForgotPasswordViewModel`
- [ ] `ResetPasswordViewModel`
- [ ] `PasswordInputModel` (shared partial model)

### Controller Actions
- [ ] `GET  /auth/login`
- [ ] `POST /auth/login` → mock validate → cookie stub → redirect Home
- [ ] `GET  /auth/register`
- [ ] `POST /auth/register` → mock create → TempData → redirect Otp
- [ ] `GET  /auth/otp` → reads TempData
- [ ] `POST /auth/otp` → mock validate → branch on Purpose
- [ ] `POST /auth/resend-otp` → re-issue OTP → redirect Otp
- [ ] `GET  /auth/forgot-password`
- [ ] `POST /auth/forgot-password` → mock OTP → redirect Otp
- [ ] `GET  /auth/reset-password` → guard TempData
- [ ] `POST /auth/reset-password` → mock update → redirect Login
- [ ] `POST /auth/logout`

### Stitch Screen Extraction
- [ ] Fetch Screen 1 (`c57ea1d0287c409499c25765c2303b05`) via Stitch MCP → extract HTML
- [ ] Fetch Screen 2 (`2542b2655a13496dba11df7e53be8ee3`) via Stitch MCP → extract HTML
- [ ] Fetch Screen 3 (`d39e784e635d4958be142ceb71d8c4bf`) via Stitch MCP → extract HTML
- [ ] Fetch Screen 4 (`700d811e07ab4f5489bd3c099c5b1b48`) via Stitch MCP → extract HTML
- [ ] Fetch Screen 5 (`c45299961a474f049421d54b6cb1f607`) via Stitch MCP → extract HTML
- [ ] Map all design tokens → `tailwind.config.js` (replace placeholder hex values)

### Views
- [ ] `_LayoutAuth.cshtml` (auth-only layout, no nav)
- [ ] `Login.cshtml`
- [ ] `Register.cshtml`
- [ ] `Otp.cshtml` — dev OTP banner, countdown timer
- [ ] `ForgotPassword.cshtml`
- [ ] `ResetPassword.cshtml`

### Partials
- [ ] `_AuthAlert.cshtml` (success + error banners)
- [ ] `_OtpInput.cshtml` (6 boxes, dir="ltr")
- [ ] `_PasswordInput.cshtml` (show/hide toggle, eye icons)

### JavaScript (`landing.js`)
- [ ] OTP auto-advance + paste handler
- [ ] Password show/hide toggle
- [ ] OTP countdown timer (10 min)

### i18n
- [ ] All auth keys added to `en.json`
- [ ] All auth keys added to `ar.json`
- [ ] `data-i18n` attribute on every user-facing string in all 5 views and 3 partials
- [ ] RTL tested in Arabic (`dir="rtl"` on `<html>` when language = ar)

### Dev UX
- [ ] `DEV — OTP` banner visible in `Development` environment on Otp view
- [ ] Dev banner hidden in `Production` environment

---

*Phase 2 hook: when Application layer is ready, delete `Mock/AuthMockStore.cs`,
inject `ISender` into `AuthController`, and replace mock blocks with
`await _sender.Send(new XyzCommand(...))` calls. Views and partials need zero changes.*
