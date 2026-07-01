using DAKKN.Application.Common.Services;
using DAKKN.Application.Localization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Localization;

namespace DAKKN.Tests.Tests.Application.Services
{
    public class ImageValidatorTests
    {
        private readonly Mock<IWebHostEnvironment> _envMock;
        private readonly Mock<IStringLocalizer<Messages>> _localizerMock;
        private readonly FileExtensionContentTypeProvider _contentProvider;
        private readonly ImageValidator _validator;

        public ImageValidatorTests()
        {
            _envMock = new Mock<IWebHostEnvironment>();
            _localizerMock = new Mock<IStringLocalizer<Messages>>();
            _localizerMock.Setup(l => l[It.IsAny<string>()])
                .Returns((string key) => new LocalizedString(key, key));
            _contentProvider = new FileExtensionContentTypeProvider();
            _validator = new ImageValidator(_envMock.Object, _contentProvider, _localizerMock.Object);
        }

        [Fact]
        public void IsValidImage_ShouldReturnTrue_ForValidJpg()
        {
            var file = CreateMockFormFile("test.jpg", "image/jpeg", 1024);
            var result = _validator.IsValidImage(file);
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidImage_ShouldReturnTrue_ForValidPng()
        {
            var file = CreateMockFormFile("test.png", "image/png", 2048);
            var result = _validator.IsValidImage(file);
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidImage_ShouldReturnTrue_ForValidWebp()
        {
            var file = CreateMockFormFile("test.webp", "image/webp", 1024);
            var result = _validator.IsValidImage(file);
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidImage_ShouldReturnFalse_WhenFileIsNull()
        {
            var result = _validator.IsValidImage((IFormFile)null!);
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidImage_ShouldReturnFalse_WhenFileIsEmpty()
        {
            var file = CreateMockFormFile("test.jpg", "image/jpeg", 0);
            var result = _validator.IsValidImage(file);
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidImage_ShouldReturnFalse_WhenFileExceedsMaxSize()
        {
            var file = CreateMockFormFile("test.jpg", "image/jpeg", 11 * 1024 * 1024);
            var result = _validator.IsValidImage(file);
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidImage_ShouldReturnFalse_ForInvalidExtension()
        {
            var file = CreateMockFormFile("test.exe", "application/x-msdownload", 1024);
            var result = _validator.IsValidImage(file);
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidImage_ShouldReturnFalse_ForInvalidContentType()
        {
            var file = CreateMockFormFile("test.jpg", "text/plain", 1024);
            var result = _validator.IsValidImage(file);
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidImage_ShouldReturnFalse_WhenFileNameIsEmpty()
        {
            var file = CreateMockFormFile("", "image/jpeg", 1024);
            var result = _validator.IsValidImage(file);
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidImage_ShouldReturnTrue_WhenContentTypeIsEmpty_ForGif()
        {
            var file = CreateMockFormFile("test.gif", "", 1024);
            var result = _validator.IsValidImage(file);
            result.Should().BeTrue();
        }

        [Fact]
        public void GetUniqueFileName_ShouldGenerateTimestampedFileName()
        {
            var result = _validator.GetUniqueFileName("photo.jpg");
            result.Should().EndWith(".jpg");
            result.Length.Should().BeGreaterThan(15);
        }

        [Fact]
        public void GetUniqueFileName_ShouldDefaultToJpg_WhenNoExtension()
        {
            var result = _validator.GetUniqueFileName("photo");
            result.Should().EndWith(".jpg");
        }

        [Fact]
        public void GetUniqueFileName_ShouldDefaultToJpg_WhenFileNameIsEmpty()
        {
            var result = _validator.GetUniqueFileName("");
            result.Should().EndWith(".jpg");
        }

        [Fact]
        public void GetUniqueFileName_ShouldDefaultToJpg_WhenFileNameIsNull()
        {
            var result = _validator.GetUniqueFileName(null!);
            result.Should().EndWith(".jpg");
        }

        private static IFormFile CreateMockFormFile(string fileName, string contentType, long length)
        {
            var stream = new MemoryStream(new byte[length]);
            var formFile = new FormFile(stream, 0, length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
            return formFile;
        }
    }
}
