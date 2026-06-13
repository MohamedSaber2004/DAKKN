using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Localization;
using MediatR;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Attachments.Commands.UpdateImage
{
    public class UpdateImageCommandHandler : IRequestHandler<UpdateImageCommand, string>
    {
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly IImageValidator _imageValidator;

        public UpdateImageCommandHandler
        (
            IStringLocalizer<Messages> localizer,
            IImageValidator imageValidator
        )
        {
            _localizer = localizer;
            _imageValidator = imageValidator;
        }

        public async Task<string> Handle(UpdateImageCommand request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(request.ImageName))
            {
                try
                {
                    await _imageValidator.DeleteImage(request.ImageName, request.UploadPlace);
                }
                catch
                {
                    throw new BadRequestException(_localizer[LocalizationKeys.UploadFileMessages.FileFailedToDeleted.Value]);
                }
            }

            var result = await _imageValidator.UploadImage(request.File, request.UploadPlace);

            if (!result.Uploaded)
            {
                throw new BadRequestException(result.Result);
            }
            return result.Result;
        }
    }
}
