using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.CMS.DTOs;
using DAKKN.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.CMS.Commands.UpdateLandingPageSettings
{
    public class UpdateLandingPageSettingsCommandHandler : IRequestHandler<UpdateLandingPageSettingsCommand, LandingPageSettingsDto>
    {
        private readonly IApplicationDbContext _context;

        public UpdateLandingPageSettingsCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LandingPageSettingsDto> Handle(UpdateLandingPageSettingsCommand request, CancellationToken cancellationToken)
        {
            var existing = await _context.LandingPageSettings
                .ToListAsync(cancellationToken);

            var pairs = new[]
            {
                (Value: request.SectionOrder, Key: "cms_section_order", Desc: "Section order for landing page"),
                (Value: request.Hero,         Key: "cms_hero",          Desc: "Hero banner settings"),
                (Value: request.About,        Key: "cms_about",         Desc: "About section settings"),
                (Value: request.Categories,   Key: "cms_categories",    Desc: "Featured categories"),
                (Value: request.Products,     Key: "cms_products",      Desc: "Featured products"),
                (Value: request.Testimonials, Key: "cms_testimonials",  Desc: "Customer testimonials"),
                (Value: request.Contact,      Key: "cms_contact",       Desc: "Contact info settings"),
            };

            foreach (var (value, key, desc) in pairs)
            {
                if (value is null) continue;

                var setting = existing.FirstOrDefault(s => s.Key == key);
                if (setting is null)
                {
                    setting = new LandingPageSetting
                    {
                        Key = key,
                        Value = value,
                        Description = desc
                    };
                    _context.LandingPageSettings.Add(setting);
                    existing.Add(setting);
                }
                else
                {
                    setting.Value = value;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            string GetExisting(string key) => existing.FirstOrDefault(s => s.Key == key)?.Value ?? string.Empty;

            return new LandingPageSettingsDto
            {
                SectionOrder = request.SectionOrder ?? GetExisting("cms_section_order"),
                Hero =         request.Hero         ?? GetExisting("cms_hero"),
                About =        request.About        ?? GetExisting("cms_about"),
                Categories =   request.Categories   ?? GetExisting("cms_categories"),
                Products =     request.Products     ?? GetExisting("cms_products"),
                Testimonials = request.Testimonials ?? GetExisting("cms_testimonials"),
                Contact =      request.Contact      ?? GetExisting("cms_contact"),
            };
        }
    }
}
