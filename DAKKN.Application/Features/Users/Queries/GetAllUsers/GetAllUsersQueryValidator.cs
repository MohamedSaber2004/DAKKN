using DAKKN.Application.Localization;
using DAKKN.Domain.Enums;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;

namespace DAKKN.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryValidator : AbstractValidator<GetAllUsersQuery>
    {
        public GetAllUsersQueryValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage(localizer[LocalizationKeys.ValidationMessages.GreaterThanOrEqual.Value, 1]);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage(localizer[LocalizationKeys.ValidationMessages.Range.Value, 1, 100]);

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100)
                .WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 100]);

            var validRoles = Enum.GetNames(typeof(UserType));
            RuleFor(x => x.Role)
                .Must(role => string.IsNullOrEmpty(role) || validRoles.Contains(role))
                .WithMessage(localizer[LocalizationKeys.Users.InvalidRole.Value]);

            RuleFor(x => x.Status)
                .Must(status => string.IsNullOrEmpty(status) || status == "Active" || status == "Deleted")
                .WithMessage(localizer[LocalizationKeys.Users.InvalidStatus.Value]);
        }
    }
}
