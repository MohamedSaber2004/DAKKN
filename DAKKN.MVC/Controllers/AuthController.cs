using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Options;
using DAKKN.Application.Features.Auth.Comands.ForgetPassword;
using DAKKN.Application.Features.Auth.Comands.LoginWithGoogle;
using DAKKN.Application.Features.Auth.Comands.Logout;
using DAKKN.Application.Features.Auth.Comands.RefreshToken;
using DAKKN.Application.Features.Auth.Comands.SignIn;
using DAKKN.Application.Features.Auth.Comands.SignUp;
using DAKKN.Application.Features.Auth.Comands.VerifyForgetPasswordOtp;
using DAKKN.Application.Features.Auth.Commands.ResetPassword;
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
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace DAKKN.MVC.Controllers
{
    [Route("auth")]
    [EnableRateLimiting("auth")]
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly GoogleAuthSettings _googleAuthSettings;
        private readonly IWebHostEnvironment _env;

        public AuthController(
            IWebHostEnvironment env,
            IMediator mediator,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<GoogleAuthSettings> googleAuthSettings)
        {
            _env = env;
            _mediator = mediator;
            _userManager = userManager;
            _signInManager = signInManager;
            _googleAuthSettings = googleAuthSettings.Value;
        }

        // ──────────────────────────────────────────────
        // LOGIN
        // ──────────────────────────────────────────────

        [HttpGet("login")]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["GoogleClientId"] = _googleAuthSettings?.WebClientId;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginViewModel vm)
        {
            ViewData["GoogleClientId"] = _googleAuthSettings?.WebClientId;
            if (!ModelState.IsValid) return View(vm);

            try
            {
                var result = await _mediator.Send(new SignInCommand(vm.Email, vm.Password, vm.RememberMe));
                
                // Sign in via Identity Cookies to support MVC page requests
                var user = await _userManager.FindByEmailAsync(vm.Email);
                if (user != null)
                {
                    await _signInManager.SignInAsync(user, vm.RememberMe);
                }

                return View("AuthCallback", result);
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
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
            }

            return View(vm);
        }

        [HttpPost("login-google")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] string idToken, string? returnUrl = null)
        {
            try
            {
                var result = await _mediator.Send(new LoginWithGoogleCommand(idToken));

                // Sign in via Identity Cookies
                var user = await _userManager.FindByIdAsync(result.UserId.ToString());
                if (user != null)
                {
                    await _signInManager.SignInAsync(user, isPersistent: true);
                }

                return Ok(result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors });
            }
            catch (UnAuthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ──────────────────────────────────────────────
        // REGISTER
        // ──────────────────────────────────────────────

        [HttpGet("register")]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["GoogleClientId"] = _googleAuthSettings?.WebClientId;
            return View(new RegisterViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel vm)
        {
            ViewData["GoogleClientId"] = _googleAuthSettings?.WebClientId;
            if (!ModelState.IsValid) return View(vm);

            try
            {
                var result = await _mediator.Send(new SignupCommand(vm.FullName, vm.Email, vm.Password, vm.ConfirmPassword));

                // Sign in via Identity Cookies to support MVC page requests (Auto-login after signup)
                var user = await _userManager.FindByEmailAsync(vm.Email);
                if (user != null)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                }

                return View("AuthCallback", result);
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
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
            }

            return View(vm);
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
        public async Task<IActionResult> Otp(OtpViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            if (vm.Purpose == "ForgotPassword")
            {
                try
                {
                    var isValid = await _mediator.Send(new VerifyForgetPasswordOtpCommand(vm.Email, vm.Code));
                    if (!isValid)
                    {
                        ModelState.AddModelError(string.Empty, "invalid_otp");
                        return View(vm);
                    }

                    TempData["ResetEmail"] = vm.Email;
                    TempData["ResetToken"] = vm.Code;
                    return RedirectToAction(nameof(ResetPassword));
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
                    return View(vm);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(vm);
                }
            }

            if (vm.Purpose == "VerifyEmail")
            {
                if (!AuthMockStore.ValidateOtp(vm.Email, vm.Code))
                {
                    ModelState.AddModelError(string.Empty, "invalid_otp");
                    return View(vm);
                }

                var user = AuthMockStore.FindByEmail(vm.Email);
                if (user is not null) user.IsVerified = true;

                TempData["SuccessMessage"] = "email_verified";
                return RedirectToAction(nameof(Login));
            }

            return View(vm);
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp(string email, string purpose)
        {
            TempData["OtpEmail"]   = email;
            TempData["OtpPurpose"] = purpose;

            if (purpose == "ForgotPassword")
            {
                try
                {
                    await _mediator.Send(new ForgetPasswordCommand(email));
                }
                catch
                {
                    // For security, ignore errors in resend
                }
            }
            else
            {
                if (_env.IsDevelopment())
                    TempData["DevOtp"] = AuthMockStore.IssueOtp(email);
                else
                    AuthMockStore.IssueOtp(email);
            }

            return RedirectToAction(nameof(Otp));
        }

        // ──────────────────────────────────────────────
        // FORGOT PASSWORD  Step 1/3
        // ──────────────────────────────────────────────

        [HttpGet("forgot-password")]
        public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                await _mediator.Send(new ForgetPasswordCommand(vm.Email));
            }
            catch (ValidationException ex)
            {
                // To prevent email enumeration, we don't show validation errors like "User not found"
                // But we can show other validation errors if they occur (e.g. invalid email format)
                if (ex.Errors.ContainsKey("Email") && ex.Errors["Email"].Any(x => x.Contains("not found", StringComparison.OrdinalIgnoreCase)))
                {
                    // Ignore user not found
                }
                else
                {
                    foreach (var error in ex.Errors)
                    {
                        foreach (var message in error.Value)
                        {
                            ModelState.AddModelError(error.Key, message);
                        }
                    }
                    if (ModelState.ErrorCount > 0) return View(vm);
                }
            }
            catch (Exception)
            {
                // Ignore other errors for security
            }

            TempData["OtpEmail"] = vm.Email;
            TempData["OtpPurpose"] = "ForgotPassword";
            return RedirectToAction(nameof(Otp));
        }

        // ──────────────────────────────────────────────
        // RESET PASSWORD  Step 3/3
        // ──────────────────────────────────────────────

        [HttpGet("reset-password")]
        public IActionResult ResetPassword()
        {
            var email = TempData.Peek("ResetEmail") as string;
            var token = TempData.Peek("ResetToken") as string;

            // Guard: cannot reach this page without passing OTP
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return RedirectToAction(nameof(ForgotPassword));

            return View(new ResetPasswordViewModel { Email = email, Token = token });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                await _mediator.Send(new ResetPasswordCommand(vm.Email, vm.Token, vm.NewPassword, vm.ConfirmPassword));
                TempData["SuccessMessage"] = "password_reset";
                return RedirectToAction(nameof(Login));
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
        // LOGOUT
        // ──────────────────────────────────────────────

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromForm] string? refreshToken)
        {
            if (!string.IsNullOrEmpty(refreshToken))
            {
                try
                {
                    await _mediator.Send(new LogoutCommand(refreshToken));
                }
                catch { /* Ignore revocation errors */ }
            }

            await _signInManager.SignOutAsync();

            return View("LogoutCallback");
        }

        [HttpGet("access-denied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
