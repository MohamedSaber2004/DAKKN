using DAKKN.Application.Features.StickerSuggestions.Commands.SubmitSuggestion;
using DAKKN.Application.Features.StickerSuggestions.Queries.GetMySuggestions;
using DAKKN.Application.Features.StickerSuggestions.Queries.GetSuggestionById;
using DAKKN.Application.Localization;
using DAKKN.Appearence.Filters;
using DAKKN.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DAKKN.Appearence.Controllers
{
    [RoleAuthorize(UserType.User, UserType.Admin)]
    public class StickerSuggestionController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<Messages> _localizer;

        public StickerSuggestionController(IMediator mediator, IStringLocalizer<Messages> localizer)
        {
            _mediator = mediator;
            _localizer = localizer;
        }

        [HttpGet]
        public IActionResult Submit()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(SubmitSuggestionCommand command)
        {
            if (!ModelState.IsValid)
                return View(command);

            var result = await _mediator.Send(command);
            TempData["Success"] = _localizer[LocalizationKeys.SuggestionMessages.Submitted.Value].Value;
            return RedirectToAction("MySuggestions");
        }

        [HttpGet]
        public async Task<IActionResult> MySuggestions([FromQuery] int pageNumber = 1)
        {
            var result = await _mediator.Send(new GetMySuggestionsQuery(pageNumber, 10));
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _mediator.Send(new GetSuggestionByIdQuery(id));
            return View(result);
        }
    }
}
