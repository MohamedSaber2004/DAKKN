using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Features.Auth.Comands.RefreshToken;
using DAKKN.Application.Features.Auth.Comands.SignIn;
using DAKKN.Application.Features.Auth.Comands.SignUp;
using DAKKN.Application.Features.Auth.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.MVC.Mock;
using DAKKN.MVC.ViewModels.Auth;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace DAKKN.MVC.Controllers
{
    [Route("auth")]
    [EnableRateLimiting("auth")]
    public class AuthController(IWebHostEnvironment env, IMediator mediator, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly IMediator _mediator = mediator;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        // ──────────────────────────────────────────────
        // LOGIN
        // ──────────────────────────────────────────────

        [HttpGet("login")]
        public IActionResult Login(string? returnUrl = null) => View(new LoginViewModel { ReturnUrl = returnUrl });

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            try
            {
                var result = await _mediator.Send(new SignInCommand(vm.Email, vm.Password, vm.RememberMe));
                return await SignInAndRedirect(result, vm.RememberMe, vm.ReturnUrl);
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    foreach (var message in error.Value)
                    {
                        ModelState.AddModelError(error.Key, message);
                    }
                }
            }
            catch (BadRequestException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(vm);
        }

        // ──────────────────────────────────────────────
        // REGISTER
        // ──────────────────────────────────────────────

        [HttpGet("register")]
        public IActionResult Register(string? returnUrl = null) => View(new RegisterViewModel { ReturnUrl = returnUrl });

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            try
            {
                var result = await _mediator.Send(new SignupCommand(vm.FullName, vm.Email, vm.Password, vm.ConfirmPassword));
                return await SignInAndRedirect(result, vm.RememberMe, vm.ReturnUrl);
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    foreach (var message in error.Value)
                    {
                        ModelState.AddModelError(error.Key, message);
                    }
                }
            }
            catch (BadRequestException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(vm);
        }

        private async Task<IActionResult> SignInAndRedirect(AuthResponseDto result, bool rememberMe, string? returnUrl = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.id.ToString()),
                new Claim(ClaimTypes.Name, result.FullName),
                new Claim(ClaimTypes.Email, result.Email)
            };

            foreach (var role in result.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme, ClaimTypes.Name, ClaimTypes.Role);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            });

            TempData["SuccessMessage"] = "login_success";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            if (result.Roles.Contains("Admin"))
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            return RedirectToAction("Index", "Customer");
        }

        // ──────────────────────────────────────────────
        // REFRESH TOKEN
        // ──────────────────────────────────────────────

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(string token)
        {
            try
            {
                var result = await _mediator.Send(new RefreshTokenCommand(token));
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            TempData.Clear();
            TempData["SuccessMessage"] = "logout_success";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet("access-denied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
