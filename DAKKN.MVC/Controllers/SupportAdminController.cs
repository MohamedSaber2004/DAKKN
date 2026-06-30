using DAKKN.Appearence.Filters;
using Microsoft.AspNetCore.Http;
using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Features.Support.Commands.AddInternalNote;
using DAKKN.Application.Features.Support.Commands.AssignTicket;
using DAKKN.Application.Features.Support.Commands.CloseTicket;
using DAKKN.Application.Features.Support.Commands.CreateCategory;
using DAKKN.Application.Features.Support.Commands.CreateFAQ;
using DAKKN.Application.Features.Support.Commands.CreateFAQCategory;
using DAKKN.Application.Features.Support.Commands.DeleteCategory;
using DAKKN.Application.Features.Support.Commands.DeleteFAQ;
using DAKKN.Application.Features.Support.Commands.DeleteFAQCategory;
using DAKKN.Application.Features.Support.Commands.ReopenTicket;
using DAKKN.Application.Features.Support.Commands.ReplyTicket;
using DAKKN.Application.Features.Support.Commands.UpdateCategory;
using DAKKN.Application.Features.Support.Commands.UpdateFAQ;
using DAKKN.Application.Features.Support.Commands.UpdateSettings;
using DAKKN.Application.Features.Support.Commands.UpdateTicketPriority;
using DAKKN.Application.Features.Support.Commands.UpdateTicketStatus;
using DAKKN.Application.Features.Support.Queries.GetAdminTicketDetails;
using DAKKN.Application.Features.Support.Queries.GetAdminTickets;
using DAKKN.Application.Features.Support.Queries.GetCategories;
using DAKKN.Application.Features.Support.Queries.GetDashboardStats;
using DAKKN.Application.Features.Support.Queries.GetFAQCategories;
using DAKKN.Application.Features.Support.Queries.GetFAQs;
using DAKKN.Application.Features.Support.Queries.GetSettings;
using DAKKN.Application.Localization;
using DAKKN.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DAKKN.MVC.Controllers
{
    [Route("admin")]
    [RoleAuthorize(UserType.Admin)]
    public class SupportAdminController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<Messages> _localizer;

        public SupportAdminController(IMediator mediator, IStringLocalizer<Messages> localizer)
        {
            _mediator = mediator;
            _localizer = localizer;
        }

        [HttpGet("support/dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            ViewData["Title"] = _localizer[LocalizationKeys.Support.Dashboard.Value];
            var stats = await _mediator.Send(new GetDashboardStatsQuery());
            return View(stats);
        }

        [HttpGet("support/tickets")]
        public async Task<IActionResult> Tickets(
            string? search, string? status, string? priority, Guid? categoryId,
            Guid? assignedToId, DateTime? dateFrom, DateTime? dateTo,
            string? sortBy, bool sortDescending = true, bool? hasAttachments = null,
            int pageNumber = 1, int pageSize = 10)
        {
            ViewData["Title"] = _localizer[LocalizationKeys.Support.Dashboard.Value];
            var categories = await _mediator.Send(new GetCategoriesQuery());
            ViewBag.Categories = categories.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            var query = new GetAdminTicketsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Search = search,
                Status = status,
                Priority = priority,
                CategoryId = categoryId,
                AssignedToId = assignedToId,
                DateFrom = dateFrom,
                DateTo = dateTo,
                SortBy = sortBy,
                SortDescending = sortDescending,
                HasAttachments = hasAttachments
            };
            var result = await _mediator.Send(query);
            return View(result);
        }

        [HttpGet("support/tickets/{id:guid}")]
        public async Task<IActionResult> TicketDetails(Guid id)
        {
            ViewData["Title"] = _localizer[LocalizationKeys.Support.Dashboard.Value];
            try
            {
                var ticket = await _mediator.Send(new GetAdminTicketDetailsQuery(id));
                return View(ticket);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("support/tickets/{id:guid}/reply")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(Guid id, string Message, List<IFormFile>? attachments)
        {
            if (string.IsNullOrWhiteSpace(Message))
            {
                TempData["ErrorMessage"] = _localizer[LocalizationKeys.ExceptionMessages.InvalidModelState.Value].Value;
                return RedirectToAction(nameof(TicketDetails), new { id });
            }

            var command = new ReplyTicketCommand
            {
                TicketId = id,
                Message = Message,
                IsStaffReply = true,
                Attachments = attachments
            };

            try
            {
                await _mediator.Send(command);
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.Support.TicketReplied.Value].Value;
            }
            catch (Exception ex) when (ex is ValidationException or BadRequestException or NotFoundException)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(TicketDetails), new { id });
        }

        [HttpPost("support/tickets/{id:guid}/close")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseTicket(Guid id)
        {
            try
            {
                await _mediator.Send(new CloseTicketCommand(id));
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.Support.TicketClosed.Value].Value;
            }
            catch (Exception ex) when (ex is ValidationException or BadRequestException or NotFoundException)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(TicketDetails), new { id });
        }

        [HttpPost("support/tickets/{id:guid}/reopen")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reopen(Guid id)
        {
            try
            {
                await _mediator.Send(new ReopenTicketCommand(id));
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.Support.TicketReopened.Value].Value;
            }
            catch (Exception ex) when (ex is ValidationException or BadRequestException or NotFoundException)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(TicketDetails), new { id });
        }

        [HttpPost("support/tickets/{id:guid}/assign")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(Guid id, Guid adminId, string adminName)
        {
            try
            {
                await _mediator.Send(new AssignTicketCommand(id, adminId, adminName));
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.Support.TicketAssigned.Value].Value;
            }
            catch (Exception ex) when (ex is ValidationException or BadRequestException or NotFoundException)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(TicketDetails), new { id });
        }

        [HttpPost("support/tickets/{id:guid}/priority")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePriority(Guid id, string newPriority)
        {
            try
            {
                await _mediator.Send(new UpdateTicketPriorityCommand(id, newPriority));
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.Support.TicketPriorityChanged.Value].Value;
            }
            catch (Exception ex) when (ex is ValidationException or BadRequestException or NotFoundException)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(TicketDetails), new { id });
        }

        [HttpPost("support/tickets/{id:guid}/status")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(Guid id, string newStatus)
        {
            try
            {
                await _mediator.Send(new UpdateTicketStatusCommand(id, newStatus));
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.Support.TicketStatusChanged.Value].Value;
            }
            catch (Exception ex) when (ex is ValidationException or BadRequestException or NotFoundException)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(TicketDetails), new { id });
        }

        [HttpPost("support/tickets/{id:guid}/note")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNote(Guid id, AddInternalNoteCommand command)
        {
            command.TicketId = id;

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = _localizer[LocalizationKeys.ExceptionMessages.InvalidModelState.Value].Value;
                return RedirectToAction(nameof(TicketDetails), new { id });
            }

            try
            {
                await _mediator.Send(command);
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.Support.InternalNotes.Value].Value;
            }
            catch (Exception ex) when (ex is ValidationException or BadRequestException or NotFoundException)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(TicketDetails), new { id });
        }

        [HttpGet("support/categories")]
        public async Task<IActionResult> Categories()
        {
            ViewData["Title"] = _localizer[LocalizationKeys.Support.Dashboard.Value];
            var categories = await _mediator.Send(new GetCategoriesQuery(IncludeInactive: true));
            return View(categories);
        }

        [HttpGet("support/categories/create")]
        public IActionResult CreateCategory()
        {
            return RedirectToAction(nameof(Categories));
        }

        [HttpPost("support/categories/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CreateCategoryCommand command)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = _localizer[LocalizationKeys.ExceptionMessages.InvalidModelState.Value].Value;
                return RedirectToAction(nameof(Categories));
            }

            try
            {
                await _mediator.Send(command);
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.ActionResultMessage.Created.Value].Value;
            }
            catch (Exception ex) when (ex is ValidationException or BadRequestException)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Categories));
        }

        [HttpGet("support/categories/edit/{id:guid}")]
        public IActionResult EditCategory(Guid id)
        {
            return RedirectToAction(nameof(Categories));
        }

        [HttpPost("support/categories/edit/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(Guid id, UpdateCategoryCommand command)
        {
            command.Id = id;

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = _localizer[LocalizationKeys.ExceptionMessages.InvalidModelState.Value].Value;
                return RedirectToAction(nameof(Categories));
            }

            try
            {
                await _mediator.Send(command);
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.ActionResultMessage.Ok.Value].Value;
            }
            catch (Exception ex) when (ex is ValidationException or BadRequestException)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Categories));
        }

        [HttpPost("support/categories/delete/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteCategoryCommand(id));
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.ActionResultMessage.Deleted.Value].Value;
            }
            catch (Exception ex) when (ex is ValidationException or BadRequestException or NotFoundException)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Categories));
        }

        [HttpGet("support/faqs")]
        public async Task<IActionResult> FAQs(Guid? categoryId)
        {
            ViewData["Title"] = _localizer[LocalizationKeys.Support.Dashboard.Value];
            var faqs = await _mediator.Send(new GetFAQsQuery(categoryId, OnlyPublished: false));
            var faqCategories = await _mediator.Send(new GetFAQCategoriesQuery());
            ViewBag.Categories = faqCategories.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            return View(faqs);
        }

        [HttpGet("support/faqs/create")]
        public async Task<IActionResult> CreateFAQ()
        {
            ViewData["Title"] = _localizer[LocalizationKeys.Support.Dashboard.Value];
            var faqCategories = await _mediator.Send(new GetFAQCategoriesQuery());
            ViewBag.Categories = faqCategories.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            return View();
        }

        [HttpPost("support/faqs/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFAQ(CreateFAQCommand command)
        {
            if (!ModelState.IsValid)
            {
                var catgories = await _mediator.Send(new GetFAQCategoriesQuery());
                ViewBag.Categories = catgories.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();
                return View(command);
            }

            try
            {
                await _mediator.Send(command);
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.ActionResultMessage.Created.Value].Value;
                return RedirectToAction(nameof(FAQs));
            }
            catch (ValidationException ex)
            {
                foreach (var kvp in ex.Errors)
                {
                    foreach (var error in kvp.Value)
                    {
                        ModelState.AddModelError(kvp.Key, error);
                    }
                }
            }
            catch (BadRequestException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            var createCat = await _mediator.Send(new GetFAQCategoriesQuery());
            ViewBag.Categories = createCat.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            return View(command);
        }

        [HttpGet("support/faqs/edit/{id:guid}")]
        public async Task<IActionResult> EditFAQ(Guid id)
        {
            ViewData["Title"] = _localizer[LocalizationKeys.Support.Dashboard.Value];
            var faqs = await _mediator.Send(new GetFAQsQuery(OnlyPublished: false));
            var faq = faqs.FirstOrDefault(f => f.Id == id);
            if (faq == null)
                return NotFound();

            var command = new UpdateFAQCommand
            {
                Id = faq.Id,
                Question = faq.Question,
                ArQuestion = faq.ArQuestion,
                Answer = faq.Answer,
                ArAnswer = faq.ArAnswer,
                CategoryId = faq.CategoryId,
                DisplayOrder = faq.DisplayOrder,
                IsPublished = faq.IsPublished
            };

            var editCategories = await _mediator.Send(new GetFAQCategoriesQuery());
            ViewBag.Categories = editCategories.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            return View(command);
        }

        [HttpPost("support/faqs/edit/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFAQ(Guid id, UpdateFAQCommand command)
        {
            command.Id = id;

            if (!ModelState.IsValid)
            {
                var editCat = await _mediator.Send(new GetFAQCategoriesQuery());
                ViewBag.Categories = editCat.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();
                return View(command);
            }

            try
            {
                await _mediator.Send(command);
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.ActionResultMessage.Ok.Value].Value;
                return RedirectToAction(nameof(FAQs));
            }
            catch (ValidationException ex)
            {
                foreach (var kvp in ex.Errors)
                {
                    foreach (var error in kvp.Value)
                    {
                        ModelState.AddModelError(kvp.Key, error);
                    }
                }
            }
            catch (BadRequestException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            var editCat2 = await _mediator.Send(new GetFAQCategoriesQuery());
            ViewBag.Categories = editCat2.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            return View(command);
        }

        [HttpPost("support/faqs/delete/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFAQ(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteFAQCommand(id));
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.ActionResultMessage.Deleted.Value].Value;
            }
            catch (Exception ex) when (ex is ValidationException or BadRequestException or NotFoundException)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(FAQs));
        }

        [HttpGet("support/settings")]
        public async Task<IActionResult> Settings()
        {
            ViewData["Title"] = _localizer[LocalizationKeys.Support.Settings.Value];
            var settings = await _mediator.Send(new GetSettingsQuery());
            var command = new UpdateSettingsCommand
            {
                SupportEmail = settings.SupportEmail,
                DefaultPriority = settings.DefaultPriority,
                DefaultResponseTime = settings.DefaultResponseTime,
                MaxAttachmentSize = settings.MaxAttachmentSize,
                AllowedExtensions = settings.AllowedExtensions,
                AutoCloseDays = settings.AutoCloseDays,
                NotifyOnNewTicket = settings.NotifyOnNewTicket,
                NotifyOnReply = settings.NotifyOnReply,
                NotifyOnAssignment = settings.NotifyOnAssignment,
                NotifyOnStatusChange = settings.NotifyOnStatusChange
            };
            return View(command);
        }

        [HttpPost("support/settings")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(UpdateSettingsCommand command)
        {
            if (!ModelState.IsValid)
                return View(command);

            try
            {
                await _mediator.Send(command);
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.Support.Settings.Value].Value;
                return RedirectToAction(nameof(Settings));
            }
            catch (ValidationException ex)
            {
                foreach (var kvp in ex.Errors)
                {
                    foreach (var error in kvp.Value)
                    {
                        ModelState.AddModelError(kvp.Key, error);
                    }
                }
            }
            catch (BadRequestException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(command);
        }

        [HttpGet("support/faq-categories")]
        public async Task<IActionResult> FAQCategories()
        {
            ViewData["Title"] = _localizer[LocalizationKeys.Support.Dashboard.Value];
            var categories = await _mediator.Send(new GetFAQCategoriesQuery());
            return View(categories);
        }

        [HttpPost("support/faq-categories/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFAQCategory(CreateFAQCategoryCommand command)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = _localizer[LocalizationKeys.ExceptionMessages.InvalidModelState.Value].Value;
                return RedirectToAction(nameof(FAQCategories));
            }

            try
            {
                await _mediator.Send(command);
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.ActionResultMessage.Created.Value].Value;
            }
            catch (Exception ex) when (ex is ValidationException or BadRequestException)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(FAQCategories));
        }

        [HttpPost("support/faq-categories/delete/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFAQCategory(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteFAQCategoryCommand(id));
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.ActionResultMessage.Deleted.Value].Value;
            }
            catch (Exception ex) when (ex is ValidationException or BadRequestException or NotFoundException)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(FAQCategories));
        }

    }
}
