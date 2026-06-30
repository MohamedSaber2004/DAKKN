using DAKKN.Application.Features.Support.Commands.CloseTicket;
using DAKKN.Application.Features.Support.Commands.CreateTicket;
using DAKKN.Application.Features.Support.Commands.ReplyTicket;
using DAKKN.Application.Features.Support.Queries.GetCategories;
using DAKKN.Application.Features.Support.Queries.GetMyTickets;
using DAKKN.Application.Features.Support.Queries.GetTicketDetails;
using DAKKN.Application.Localization;
using DAKKN.Appearence.Filters;
using DAKKN.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DAKKN.MVC.Controllers
{
    [Route("customer/support")]
    [RoleAuthorize(UserType.User, UserType.Admin)]
    public class SupportCustomerController(IStringLocalizer<Messages> localizer, IMediator mediator) : Controller
    {
        [HttpGet("")]
        public async Task<IActionResult> Index([FromQuery] int pageNumber = 1, [FromQuery] string? status = null, [FromQuery] string? search = null)
        {
            ViewData["Title"] = localizer[LocalizationKeys.Support.Dashboard.Value];

            var result = await mediator.Send(new GetMyTicketsQuery
            {
                PageNumber = pageNumber,
                PageSize = 10,
                Status = status,
                Search = search
            });
            return View(result);
        }

        [HttpGet("new")]
        public async Task<IActionResult> NewTicket()
        {
            ViewData["Title"] = localizer[LocalizationKeys.Support.NewTicket.Value];

            var categories = await mediator.Send(new GetCategoriesQuery());
            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost("new")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewTicket(CreateTicketCommand command, List<IFormFile>? attachments)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = localizer[LocalizationKeys.Support.NewTicket.Value];
                var categories = await mediator.Send(new GetCategoriesQuery());
                ViewBag.Categories = categories;
                return View(command);
            }

            command.Attachments = attachments;
            var result = await mediator.Send(command);

            TempData["SuccessMessage"] = localizer[LocalizationKeys.Support.TicketCreated.Value].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var ticket = await mediator.Send(new GetTicketDetailsQuery(id, IsAdmin: false));
            ViewData["Title"] = ticket.Subject;
            return View(ticket);
        }

        [HttpPost("{id:guid}/reply")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(Guid id, string message, List<IFormFile>? attachments)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                TempData["ErrorMessage"] = localizer[LocalizationKeys.ValidationMessages.Required.Value].Value;
                return RedirectToAction(nameof(Details), new { id });
            }

            var command = new ReplyTicketCommand
            {
                TicketId = id,
                Message = message,
                IsStaffReply = false,
                Attachments = attachments
            };

            await mediator.Send(command);

            TempData["SuccessMessage"] = localizer[LocalizationKeys.Support.TicketReplied.Value].Value;
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost("{id:guid}/close")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(Guid id)
        {
            await mediator.Send(new CloseTicketCommand(id));

            TempData["SuccessMessage"] = localizer[LocalizationKeys.Support.TicketClosed.Value].Value;
            return RedirectToAction(nameof(Index));
        }
    }
}
