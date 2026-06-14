using DAKKN.Application.Common.Models;
using DAKKN.Application.Localization;
using DAKKN.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace DAKKN.Appearence.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly UserType[] _roles;

        public RoleAuthorizeAttribute(params UserType[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user == null || !user.Identity!.IsAuthenticated)
            {
                HandleUnauthenticated(context);
                return;
            }

            if (_roles.Any())
            {
                var userRoles = user.FindAll(ClaimTypes.Role).Select(c => c.Value);
                var hasRole = _roles.Any(role => userRoles.Contains(role.ToString(), StringComparer.OrdinalIgnoreCase));
                
                if (!hasRole)
                {
                    HandleForbidden(context);
                }
            }
        }

        private void HandleUnauthenticated(AuthorizationFilterContext context)
        {
            var isApiRequest = context.HttpContext.Request.Path.Value?.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) ?? false;

            if (isApiRequest)
            {
                var message = JsonLocalizationProvider.GetLocalizedString(LocalizationKeys.ExceptionMessages.Unauthorized.Value);
                var response = ApiResponse<object>.Error(new Dictionary<string, string[]>(), message, StatusCodes.Status401Unauthorized);
                context.Result = new JsonResult(response) { StatusCode = StatusCodes.Status401Unauthorized };
            }
            else
            {
                context.Result = new ChallengeResult(IdentityConstants.ApplicationScheme);
            }
        }

        private void HandleForbidden(AuthorizationFilterContext context)
        {
            var isApiRequest = context.HttpContext.Request.Path.Value?.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) ?? false;

            if (isApiRequest)
            {
                var message = "You do not have permission to access this resource.";
                var response = ApiResponse<object>.Error(new Dictionary<string, string[]>(), message, StatusCodes.Status403Forbidden);
                context.Result = new JsonResult(response) { StatusCode = StatusCodes.Status403Forbidden };
            }
            else
            {
                // Redirect to an Access Denied page or back to Home with an error
                context.Result = new ForbidResult();
            }
        }
    }
}
