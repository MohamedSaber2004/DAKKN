using DAKKN.Application.Features.CMS.DTOs;
using MediatR;

namespace DAKKN.Application.Features.CMS.Queries.GetLandingPageSettings
{
    public record GetLandingPageSettingsQuery : IRequest<LandingPageSettingsDto>;
}
