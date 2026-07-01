using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Models;
using DAKKN.Application.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace DAKKN.Appearence.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;
        private readonly ILogger<ApiExceptionFilterAttribute> _logger;
        private readonly IStringLocalizer<Messages> _localizer;

        public ApiExceptionFilterAttribute(
            ILogger<ApiExceptionFilterAttribute> logger,
            IStringLocalizer<Messages> localizer)
        {
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                { typeof(ValidationException), HandleValidationException },
                { typeof(NotFoundException), HandleNotFoundException },
                { typeof(BadRequestException), HandleBadRequestException },
                { typeof(UnAuthorizedException), HandleUnauthorizedException },
                { typeof(DbUpdateConcurrencyException), HandleDbUpdateConcurrencyException },

            };
            _logger = logger;
            _localizer = localizer;
        }

        public override void OnException(ExceptionContext context)
        {
            HandleException(context);

            base.OnException(context);
        }

        private void HandleException(ExceptionContext context)
        {
            Type type = context.Exception.GetType();
            if (_exceptionHandlers.ContainsKey(type))
            {
                _exceptionHandlers[type].Invoke(context);
                return;
            }

            if (!context.ModelState.IsValid)
            {
                HandleInvalidModelStateException(context);
                return;
            }

            HandleUnknownException(context);
        }

        private void HandleValidationException(ExceptionContext context)
        {
            var exception = context.Exception as ValidationException;

            var message = _localizer[LocalizationKeys.ExceptionMessages.Validation.Key];
            var details = ApiResponse<object>.Error(exception?.Errors, message, StatusCodes.Status400BadRequest);

            context.Result = new BadRequestObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleInvalidModelStateException(ExceptionContext context)
        {
            var errors = context.ModelState.ToDictionary(
                                kvp => kvp.Key,
                                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                            );

            var message = _localizer[LocalizationKeys.ExceptionMessages.InvalidModelState.Key];
            var details = ApiResponse<object>.Error(errors, message, StatusCodes.Status400BadRequest);

            context.Result = new BadRequestObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleNotFoundException(ExceptionContext context)
        {
            var exception = context.Exception as NotFoundException;

            var message = string.IsNullOrEmpty(exception?.Message)
                ? _localizer[LocalizationKeys.ExceptionMessages.NotFound.Key]
                : exception.Message;

            var details = ApiResponse<object>.Error(message, StatusCodes.Status404NotFound);

            context.Result = new NotFoundObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleBadRequestException(ExceptionContext context)
        {
            var exception = context.Exception as BadRequestException;

            var message = string.IsNullOrEmpty(exception?.Message)
                ? _localizer[LocalizationKeys.ExceptionMessages.BadRequest.Key]
                : exception.Message;

            var details = ApiResponse<object>.Error(message, StatusCodes.Status400BadRequest);

            context.Result = new BadRequestObjectResult(details);

            context.ExceptionHandled = true;

            _logger.LogError(exception, "{Message}", message);
        }

        private void HandleUnauthorizedException(ExceptionContext context)
        {
            var exception = context.Exception as UnAuthorizedException;

            var message = string.IsNullOrEmpty(exception?.Message)
                ? _localizer[LocalizationKeys.ExceptionMessages.Unauthorized.Key]
                : exception.Message;

            var details = ApiResponse<object>.Error(new Dictionary<string, string[]>(), message, StatusCodes.Status401Unauthorized);

            context.Result = new UnauthorizedObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleUnknownException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "An unhandled exception has occurred while executing the request.");

            var message = _localizer[LocalizationKeys.ExceptionMessages.UnknownException.Key];
            var details = ApiResponse<object>.Error(message, StatusCodes.Status500InternalServerError);

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            context.ExceptionHandled = true;
        }

        private void HandleDbUpdateConcurrencyException(ExceptionContext context)
        {
            _logger.LogWarning(context.Exception, "A concurrency conflict occurred while updating the database.");

            var message = "The record has been modified by another user. Please refresh and try again.";
            var details = ApiResponse<object>.Error(message, StatusCodes.Status409Conflict);

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status409Conflict
            };

            context.ExceptionHandled = true;
        }
    }
}
