using DAKKN.Application.Features.Support.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Support.Queries.GetSettings
{
    public record GetSettingsQuery : IRequest<SupportSettingsDto>;
}
