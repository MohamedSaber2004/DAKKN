using MediatR;
using Microsoft.AspNetCore.Http;

namespace DAKKN.Application.Features.Attachments.Commands.UpdateImage
{
    public class UpdateImageCommand : IRequest<string>
    {
        public string? ImageName { get; set; }
        public int UploadPlace { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}
