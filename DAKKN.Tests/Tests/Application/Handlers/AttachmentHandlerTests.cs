using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.Attachments.Commands.UploadImage;
using DAKKN.Application.Features.Attachments.Commands.UpdateImage;
using DAKKN.Application.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace DAKKN.Tests.Tests.Application.Handlers
{
    public class AttachmentHandlerTests
    {
        private readonly Mock<IImageValidator> _imageValidatorMock;
        private readonly Mock<IStringLocalizer<Messages>> _localizerMock;

        public AttachmentHandlerTests()
        {
            _imageValidatorMock = new Mock<IImageValidator>();
            _localizerMock = new Mock<IStringLocalizer<Messages>>();
            _localizerMock.Setup(l => l[It.IsAny<string>()]).Returns((string key) => new LocalizedString(key, key));
        }

        [Fact]
        public async Task UploadImage_ShouldReturnPath()
        {
            _imageValidatorMock.Setup(v => v.UploadImage(It.IsAny<IFormFile>(), It.IsAny<int>()))
                .ReturnsAsync((true, "/uploads/test.jpg"));

            var handler = new UploadImageCommandHandler(_imageValidatorMock.Object);
            var result = await handler.Handle(new UploadImageCommand { UploadPlace = 1, File = Mock.Of<IFormFile>() }, default);

            result.Should().Be("/uploads/test.jpg");
        }

        [Fact]
        public async Task UploadImage_Failure_ShouldThrow()
        {
            _imageValidatorMock.Setup(v => v.UploadImage(It.IsAny<IFormFile>(), It.IsAny<int>()))
                .ReturnsAsync((false, "Upload failed"));

            var handler = new UploadImageCommandHandler(_imageValidatorMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new UploadImageCommand { UploadPlace = 1, File = Mock.Of<IFormFile>() }, default))
                .Should().ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task UploadMultipleImage_ShouldReturnPaths()
        {
            _imageValidatorMock.Setup(v => v.UploadMultipleImage(It.IsAny<List<IFormFile>>(), It.IsAny<int>()))
                .ReturnsAsync((true, "/uploads/a.jpg;/uploads/b.jpg"));

            var handler = new UploadImageCommandHandler(_imageValidatorMock.Object);
            var result = await handler.Handle(new UploadMultipleImageCommand
            {
                UploadPlace = 1,
                Files = [Mock.Of<IFormFile>(), Mock.Of<IFormFile>()]
            }, default);

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task UpdateImage_ShouldDeleteOldAndUploadNew()
        {
            _imageValidatorMock.Setup(v => v.DeleteImage(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(true);
            _imageValidatorMock.Setup(v => v.UploadImage(It.IsAny<IFormFile>(), It.IsAny<int>()))
                .ReturnsAsync((true, "/uploads/new.jpg"));

            var handler = new UpdateImageCommandHandler(_localizerMock.Object, _imageValidatorMock.Object);
            var result = await handler.Handle(new UpdateImageCommand { ImageName = "old.jpg", UploadPlace = 1, File = Mock.Of<IFormFile>() }, default);

            result.Should().Be("/uploads/new.jpg");
        }

        [Fact]
        public async Task UpdateImage_DeleteFailure_ShouldThrow()
        {
            _imageValidatorMock.Setup(v => v.DeleteImage(It.IsAny<string>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Delete failed"));

            var handler = new UpdateImageCommandHandler(_localizerMock.Object, _imageValidatorMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new UpdateImageCommand { ImageName = "old.jpg", UploadPlace = 1, File = Mock.Of<IFormFile>() }, default))
                .Should().ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task UpdateImage_NullImageName_ShouldUploadOnly()
        {
            _imageValidatorMock.Setup(v => v.UploadImage(It.IsAny<IFormFile>(), It.IsAny<int>()))
                .ReturnsAsync((true, "/uploads/new.jpg"));

            var handler = new UpdateImageCommandHandler(_localizerMock.Object, _imageValidatorMock.Object);
            var result = await handler.Handle(new UpdateImageCommand { ImageName = null, UploadPlace = 1, File = Mock.Of<IFormFile>() }, default);

            result.Should().Be("/uploads/new.jpg");
        }
    }
}
