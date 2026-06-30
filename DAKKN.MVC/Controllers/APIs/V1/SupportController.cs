using Asp.Versioning;
using DAKKN.Appearence.Filters;
using DAKKN.Appearence.Routes;
using DAKKN.Application.Features.Support.Commands.AddInternalNote;
using DAKKN.Application.Features.Support.Commands.ArchiveTicket;
using DAKKN.Application.Features.Support.Commands.AssignTicket;
using DAKKN.Application.Features.Support.Commands.CloseTicket;
using DAKKN.Application.Features.Support.Commands.CreateCategory;
using DAKKN.Application.Features.Support.Commands.CreateFAQ;
using DAKKN.Application.Features.Support.Commands.CreateFAQCategory;
using DAKKN.Application.Features.Support.Commands.CreateTicket;
using DAKKN.Application.Features.Support.Commands.DeleteAttachment;
using DAKKN.Application.Features.Support.Commands.DeleteCategory;
using DAKKN.Application.Features.Support.Commands.DeleteFAQ;
using DAKKN.Application.Features.Support.Commands.DeleteFAQCategory;
using DAKKN.Application.Features.Support.Commands.EscalateTicket;
using DAKKN.Application.Features.Support.Commands.ReopenTicket;
using DAKKN.Application.Features.Support.Commands.ReplyTicket;
using DAKKN.Application.Features.Support.Commands.UpdateCategory;
using DAKKN.Application.Features.Support.Commands.UpdateFAQ;
using DAKKN.Application.Features.Support.Commands.UpdateFAQCategory;
using DAKKN.Application.Features.Support.Commands.UpdateSettings;
using DAKKN.Application.Features.Support.Commands.UpdateTicketPriority;
using DAKKN.Application.Features.Support.Commands.UpdateTicketStatus;
using DAKKN.Application.Features.Support.Commands.UploadAttachment;
using DAKKN.Application.Features.Support.Queries.GetAdminTicketDetails;
using DAKKN.Application.Features.Support.Queries.GetAdminTickets;
using DAKKN.Application.Features.Support.Queries.GetCategories;
using DAKKN.Application.Features.Support.Queries.GetDashboardStats;
using DAKKN.Application.Features.Support.Queries.GetFAQCategories;
using DAKKN.Application.Features.Support.Queries.GetFAQs;
using DAKKN.Application.Features.Support.Queries.GetMyTickets;
using DAKKN.Application.Features.Support.Queries.GetSettings;
using DAKKN.Application.Features.Support.Queries.GetTicketDetails;
using DAKKN.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DAKKN.Appearence.Controllers.APIs.V1
{
    [ApiVersion("1.0")]
    public class SupportController : BaseApiController
    {
        public SupportController(IMediator mediator) : base(mediator) { }

        #region Customer Endpoints

        [HttpPost]
        [Route(ApiRoutes.Support.CreateTicket)]
        [RoleAuthorize(UserType.User, UserType.Admin)]
        public async Task<IActionResult> CreateTicket([FromForm] CreateTicketCommand command)
        {
            var result = await _mediator.Send(command);
            return Created(ApiRoutes.Support.CreateTicket, result);
        }

        [HttpGet]
        [Route(ApiRoutes.Support.MyTickets)]
        [RoleAuthorize(UserType.User, UserType.Admin)]
        public async Task<IActionResult> GetMyTickets([FromQuery] GetMyTicketsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet]
        [Route(ApiRoutes.Support.TicketDetails)]
        [RoleAuthorize(UserType.User, UserType.Admin)]
        public async Task<IActionResult> GetTicketDetails(Guid ticketId)
        {
            var result = await _mediator.Send(new GetTicketDetailsQuery(ticketId, false));
            return Ok(result);
        }

        [HttpPost]
        [Route(ApiRoutes.Support.Reply)]
        [RoleAuthorize(UserType.User, UserType.Admin)]
        public async Task<IActionResult> Reply([FromForm] ReplyTicketCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost]
        [Route(ApiRoutes.Support.Close)]
        [RoleAuthorize(UserType.User, UserType.Admin)]
        public async Task<IActionResult> Close(Guid ticketId)
        {
            await _mediator.Send(new CloseTicketCommand(ticketId));
            return Ok(true);
        }

        [HttpPost]
        [Route(ApiRoutes.Support.Reopen)]
        [RoleAuthorize(UserType.User, UserType.Admin)]
        public async Task<IActionResult> Reopen(Guid ticketId)
        {
            await _mediator.Send(new ReopenTicketCommand(ticketId));
            return Ok(true);
        }

        [HttpPost]
        [Route(ApiRoutes.Support.UploadAttachment)]
        [RoleAuthorize(UserType.User, UserType.Admin)]
        public async Task<IActionResult> UploadAttachment([FromForm] UploadAttachmentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete]
        [Route(ApiRoutes.Support.DeleteAttachment)]
        [RoleAuthorize(UserType.User, UserType.Admin)]
        public async Task<IActionResult> DeleteAttachment(Guid attachmentId)
        {
            await _mediator.Send(new DeleteAttachmentCommand(attachmentId));
            return Deleted(true);
        }

        #endregion

        #region Admin Endpoints

        [HttpGet]
        [Route(ApiRoutes.Support.Dashboard)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _mediator.Send(new GetDashboardStatsQuery());
            return Ok(result);
        }

        [HttpGet]
        [Route(ApiRoutes.Support.AdminTickets)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> GetAdminTickets([FromQuery] GetAdminTicketsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet]
        [Route(ApiRoutes.Support.AdminTicketDetails)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> GetAdminTicketDetails(Guid ticketId)
        {
            var result = await _mediator.Send(new GetAdminTicketDetailsQuery(ticketId));
            return Ok(result);
        }

        [HttpPost]
        [Route(ApiRoutes.Support.Assign)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> Assign(Guid ticketId, [FromBody] AssignTicketCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        [Route(ApiRoutes.Support.UpdateStatus)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> UpdateStatus(Guid ticketId, [FromBody] UpdateTicketStatusCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        [Route(ApiRoutes.Support.UpdatePriority)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> UpdatePriority(Guid ticketId, [FromBody] UpdateTicketPriorityCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost]
        [Route(ApiRoutes.Support.AddNote)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> AddNote([FromBody] AddInternalNoteCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost]
        [Route(ApiRoutes.Support.Escalate)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> Escalate(Guid ticketId, [FromBody] string reason)
        {
            await _mediator.Send(new EscalateTicketCommand(ticketId, reason));
            return Ok(true);
        }

        [HttpDelete]
        [Route(ApiRoutes.Support.Archive)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> Archive(Guid ticketId)
        {
            await _mediator.Send(new ArchiveTicketCommand(ticketId));
            return Deleted(true);
        }

        [HttpGet]
        [Route(ApiRoutes.Support.Categories)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _mediator.Send(new GetCategoriesQuery(true));
            return Ok(result);
        }

        [HttpPost]
        [Route(ApiRoutes.Support.Categories)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
        {
            var result = await _mediator.Send(command);
            return Created(ApiRoutes.Support.Categories, result);
        }

        [HttpPut]
        [Route(ApiRoutes.Support.CategoryById)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete]
        [Route(ApiRoutes.Support.CategoryById)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            await _mediator.Send(new DeleteCategoryCommand(id));
            return Deleted(true);
        }

        [HttpGet]
        [Route(ApiRoutes.Support.FAQs)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> GetFAQs()
        {
            var result = await _mediator.Send(new GetFAQsQuery(null, false));
            return Ok(result);
        }

        [HttpPost]
        [Route(ApiRoutes.Support.FAQs)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> CreateFAQ([FromBody] CreateFAQCommand command)
        {
            var result = await _mediator.Send(command);
            return Created(ApiRoutes.Support.FAQs, result);
        }

        [HttpPut]
        [Route(ApiRoutes.Support.FAQById)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> UpdateFAQ(Guid id, [FromBody] UpdateFAQCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete]
        [Route(ApiRoutes.Support.FAQById)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> DeleteFAQ(Guid id)
        {
            await _mediator.Send(new DeleteFAQCommand(id));
            return Deleted(true);
        }

        [HttpGet]
        [Route(ApiRoutes.Support.FAQCategories)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> GetFAQCategories()
        {
            var result = await _mediator.Send(new GetFAQCategoriesQuery());
            return Ok(result);
        }

        [HttpPost]
        [Route(ApiRoutes.Support.FAQCategories)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> CreateFAQCategory([FromBody] CreateFAQCategoryCommand command)
        {
            var result = await _mediator.Send(command);
            return Created(ApiRoutes.Support.FAQCategories, result);
        }

        [HttpPut]
        [Route(ApiRoutes.Support.FAQCategoryById)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> UpdateFAQCategory(Guid id, [FromBody] UpdateFAQCategoryCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete]
        [Route(ApiRoutes.Support.FAQCategoryById)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> DeleteFAQCategory(Guid id)
        {
            await _mediator.Send(new DeleteFAQCategoryCommand(id));
            return Deleted(true);
        }

        [HttpGet]
        [Route(ApiRoutes.Support.Settings)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> GetSettings()
        {
            var result = await _mediator.Send(new GetSettingsQuery());
            return Ok(result);
        }

        [HttpPut]
        [Route(ApiRoutes.Support.Settings)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> UpdateSettings([FromBody] UpdateSettingsCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        #endregion

        #region Public Endpoints

        [HttpGet]
        [Route(ApiRoutes.Support.PublicFAQs)]
        public async Task<IActionResult> GetPublicFAQs([FromQuery] Guid? categoryId)
        {
            var result = await _mediator.Send(new GetFAQsQuery(categoryId, true));
            return Ok(result);
        }

        [HttpGet]
        [Route(ApiRoutes.Support.PublicCategories)]
        public async Task<IActionResult> GetPublicCategories()
        {
            var result = await _mediator.Send(new GetCategoriesQuery(false));
            return Ok(result);
        }

        #endregion
    }
}
