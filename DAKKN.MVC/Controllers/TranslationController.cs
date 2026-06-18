using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MediatR;
using DAKKN.Application.Features.Users.Commands.UpdateUserSettings;

namespace DAKKN.MVC.Controllers
{
    public class TranslationController : Controller
    {
        private readonly IMediator _mediator;

        public TranslationController(IMediator mediator)
        {
            _mediator = mediator;
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
                catch
                {
                    // Ignore background sync errors to not block the UI
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
