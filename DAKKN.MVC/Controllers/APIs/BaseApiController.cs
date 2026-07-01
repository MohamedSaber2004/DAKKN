using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Common.Models;
using DAKKN.Application.Localization;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DAKKN.Appearence.Controllers.APIs
{

    [ApiController]
    [EnableCors("CQRS")]
    [IgnoreAntiforgeryToken]
    public class BaseApiController : Controller
    {
        protected readonly IMediator _mediator;
        private readonly IStringLocalizer<Messages>? _localizer;
        private readonly ICurrentUserService? _currentUserService;

        protected BaseApiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected BaseApiController(IMediator mediator, IStringLocalizer<Messages> localizer)
        {
            _mediator = mediator;
            _localizer = localizer;
        }

        protected BaseApiController(IMediator mediator, IStringLocalizer<Messages> localizer, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _localizer = localizer;
            _currentUserService = currentUserService;
        }

        protected IActionResult Ok(string message) => base.Ok(ApiResponse<string>.Ok(null, message ?? _localizer?[LocalizationKeys.ActionResultMessage.Ok.Value] ?? "Success"));
        protected IActionResult Ok<TData>(TData? data, string message = null!) => base.Ok(ApiResponse<TData>.Ok(data, message ?? _localizer?[LocalizationKeys.ActionResultMessage.Ok.Value] ?? "Success"));
        protected IActionResult Ok2<TData>(TData? data, string message = null!) => base.Ok(Ok(data, message ?? _localizer?[LocalizationKeys.ActionResultMessage.Ok.Value] ?? "Success"));
        protected IActionResult Deleted<TData>(string uri, TData data, string message = null!) => base.Accepted(uri, ApiResponse<TData>.Ok(data, message ?? _localizer?[LocalizationKeys.ActionResultMessage.Deleted.Value] ?? "Deleted Successfully"));
        protected IActionResult Accepted<TData>(string uri, TData data, string message = null!) => base.Accepted(uri, ApiResponse<TData>.Ok(data, message ?? _localizer?[LocalizationKeys.ActionResultMessage.Accepted.Value] ?? "Accepted"));
        protected IActionResult Created<TData>(string uri, TData data, string message = null!) => base.Created(uri, ApiResponse<TData>.Ok(data, message ?? _localizer?[LocalizationKeys.ActionResultMessage.Created.Value] ?? "Created Successfully"));
        protected IActionResult Deleted<TData>(TData data, string message = null!) => base.Accepted(ApiResponse<TData>.Ok(data, message ?? _localizer?[LocalizationKeys.ActionResultMessage.Deleted.Value] ?? "Deleted Successfully"));
        protected IActionResult Accepted<TData>(TData data, string message = null!) => base.Accepted(ApiResponse<TData>.Ok(data, message ?? _localizer?[LocalizationKeys.ActionResultMessage.Accepted.Value] ?? "Accepted"));

    }
}
