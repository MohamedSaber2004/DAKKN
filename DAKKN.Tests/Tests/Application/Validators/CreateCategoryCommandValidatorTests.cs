using DAKKN.Application.Features.Categories.Commands.CreateCategory;
using DAKKN.Application.Localization;
using Microsoft.Extensions.Localization;

namespace DAKKN.Tests.Tests.Application.Validators
{
    public class CreateCategoryCommandValidatorTests
    {
        private readonly Mock<IStringLocalizer<Messages>> _localizerMock;
        private readonly CreateCategoryCommandValidator _validator;

        public CreateCategoryCommandValidatorTests()
        {
            _localizerMock = new Mock<IStringLocalizer<Messages>>();
            _localizerMock.Setup(l => l[It.IsAny<string>()]).Returns((string key) => new LocalizedString(key, key));
            _localizerMock.Setup(l => l[It.IsAny<string>(), It.IsAny<object[]>()])
                .Returns<string, object[]>((key, args) => new LocalizedString(key, key));

            _validator = new CreateCategoryCommandValidator(_localizerMock.Object);
        }

        [Fact]
        public void ShouldPass_WhenCommandIsValid()
        {
            var command = new CreateCategoryCommand("Valid Name", "اسم صحيح");
            var result = _validator.Validate(command);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ShouldFail_WhenCategoryNameIsEmpty()
        {
            var command = new CreateCategoryCommand("", "اسم");
            var result = _validator.Validate(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CategoryName" && e.ErrorMessage == "validation.required");
        }

        [Fact]
        public void ShouldFail_WhenArNameIsEmpty()
        {
            var command = new CreateCategoryCommand("Name", "");
            var result = _validator.Validate(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ArName" && e.ErrorMessage == "validation.required");
        }

        [Fact]
        public void ShouldFail_WhenCategoryNameExceedsMaxLength()
        {
            var command = new CreateCategoryCommand(new string('x', 151), "اسم");
            var result = _validator.Validate(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CategoryName");
        }

        [Fact]
        public void ShouldFail_WhenBothNamesAreEmpty()
        {
            var command = new CreateCategoryCommand("", "");
            var result = _validator.Validate(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
        }
    }
}
