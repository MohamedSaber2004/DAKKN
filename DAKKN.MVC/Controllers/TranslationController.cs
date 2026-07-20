using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MediatR;
using DAKKN.Application.Features.Users.Commands.UpdateUserSettings;
using Microsoft.Extensions.Logging;

namespace DAKKN.MVC.Controllers
{
    [AllowAnonymous]
    public class TranslationController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TranslationController> _logger;

        public TranslationController(IMediator mediator, ILogger<TranslationController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> SetLanguage(string culture, string returnUrl)
        {
            if (string.IsNullOrEmpty(culture)) culture = "ar";

            // Set the ASP.NET Core Culture cookie
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { 
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    HttpOnly = false,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Path = "/"
                }
            );

            // If user is logged in, sync the preference to the database
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                try
                {
                    await _mediator.Send(new UpdateUserSettingsCommand(null, culture, null, null, null, null));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to sync language preference '{Culture}' to DB for user {UserId}", culture, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
                }
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("/");
        }
    }
}
