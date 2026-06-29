using DAKKN.Application.Features.StickerSuggestions.Commands.UpdateSuggestionStatus;
using DAKKN.Application.Features.StickerSuggestions.Queries.GetAllSuggestions;
using DAKKN.Application.Features.StickerSuggestions.Queries.GetSuggestionById;
using DAKKN.Application.Localization;
using DAKKN.Appearence.Filters;
using DAKKN.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DAKKN.Appearence.Controllers.Admin
{
    [RoleAuthorize(UserType.Admin)]
    public class AdminStickerSuggestionController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<Messages> _localizer;

        public AdminStickerSuggestionController(IMediator mediator, IStringLocalizer<Messages> localizer)
        {
            _mediator = mediator;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] int pageNumber = 1, [FromQuery] SuggestionStatus? status = null)
        {
            var result = await _mediator.Send(new GetAllSuggestionsQuery(pageNumber, 10, status));
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _mediator.Send(new GetSuggestionByIdQuery(id));
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(UpdateSuggestionStatusCommand command)
        {
            if (!ModelState.IsValid)
            {
                var suggestion = await _mediator.Send(new GetSuggestionByIdQuery(command.SuggestionId));
                ViewData["ModelErrors"] = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["Error"] = _localizer[LocalizationKeys.ExceptionMessages.InvalidModelState.Value].Value;
                return View("Details", suggestion);
            }

            try
            {
                var message = await _mediator.Send(command);
                TempData["Success"] = message;
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Details", new { id = command.SuggestionId });
        }
    }
}
