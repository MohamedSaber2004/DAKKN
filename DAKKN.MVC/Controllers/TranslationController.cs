using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace DAKKN.MVC.Controllers
{
    public class TranslationController : Controller
    {
        [HttpGet]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            if (string.IsNullOrEmpty(culture)) culture = "ar";
            if (string.IsNullOrEmpty(returnUrl)) returnUrl = "/";

            // Set the ASP.NET Core Culture cookie
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { 
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    HttpOnly = false,
                    Secure = Request.IsHttps, // Only secure if using HTTPS
                    SameSite = SameSiteMode.Lax,
                    Path = "/"
                }
            );

            return LocalRedirect(returnUrl);
        }
    }
}
