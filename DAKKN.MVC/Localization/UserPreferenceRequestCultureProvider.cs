using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using DAKKN.Application.Features.Users.Queries.GetUserSettings;
using System.Threading.Tasks;
using System;

namespace DAKKN.MVC.Localization
{
    public class UserPreferenceRequestCultureProvider : RequestCultureProvider
    {
        public override async Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            if (httpContext.User?.Identity == null || !httpContext.User.Identity.IsAuthenticated)
            {
                return null;
            }

            try
            {
                var mediator = httpContext.RequestServices.GetService<IMediator>();
                if (mediator == null) return null;

                // Send the query to get user settings from DB
                var settings = await mediator.Send(new GetUserSettingsQuery());
                
                if (string.IsNullOrEmpty(settings?.Language))
                {
                    return null;
                }

                return new ProviderCultureResult(settings.Language);
            }
            catch
            {
                // Fallback to next provider if something fails
                return null;
            }
        }
    }
}
