using DAKKN.MVC.Mock;
using DAKKN.MVC.ViewModels.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace DAKKN.MVC.Controllers
{
    [Route("auth")]
    [EnableRateLimiting("auth")]
    public class AuthController(IWebHostEnvironment env) : Controller
    {
        // ──────────────────────────────────────────────
        // LOGIN
        // ──────────────────────────────────────────────

        [HttpGet("login")]
        [OutputCache(Duration =600)]
        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel vm)
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

            // Issue Authentication Cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "Customer") // Default to Customer for now
            };

            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookies", principal, new AuthenticationProperties
            {
                IsPersistent = vm.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            });

            TempData["SuccessMessage"] = "login_success";
            return RedirectToAction("Index", "Home");
        }

        // ──────────────────────────────────────────────
        // REGISTER
        // ──────────────────────────────────────────────

        [HttpGet("register")]
        [OutputCache(Duration = 600)]
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
        [OutputCache(Duration = 600)]
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
        [OutputCache(Duration = 600)]
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
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction(nameof(Login));
        }
    }
}
