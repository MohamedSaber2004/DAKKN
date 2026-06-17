using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DAKKN.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Unit>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateUserCommandHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IStringLocalizer<Messages> localizer,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _localizer = localizer;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            // Use _userManager.Users to ensure we find the user even if they are soft-deleted
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

            if (user == null)
            {
                throw new NotFoundException(_localizer[LocalizationKeys.AuthMessages.UserNotFound.Value]);
            }

            var userByEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userByEmail != null && userByEmail.Id != user.Id)
            {
                throw new BadRequestException(_localizer[LocalizationKeys.AuthMessages.EmailFoundBefore.Value]);
            }

            user.UpdateProfile(request.FullName, request.BirthDate ?? user.BirthDate, request.Gender ?? user.Gender, user.ProfilePictureUrl);
            user.Email = request.Email;
            user.UserName = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            
            // Handle restoration and activation (Inactive is Deleted in this business)
            if (request.IsActive) 
            {
                if (user.IsDeleted) user.Restore();
                user.Activate();
            }
            else 
            {
                if (!user.IsDeleted) user.SoftDelete(_currentUserService.UserId.ToString());
                user.Deactivate();
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException(errors);
            }

            // Explicitly call SaveChanges through UnitOfWork to ensure custom properties like IsDeleted are persisted
            await _unitOfWork.SaveChangesAsync();

            // Update Password if provided
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, request.Password);
                if (!passwordResult.Succeeded)
                {
                    var errors = string.Join(", ", passwordResult.Errors.Select(e => e.Description));
                    throw new BadRequestException(errors);
                }
            }

            // Update Role
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(request.Role))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                
                if (!await _roleManager.RoleExistsAsync(request.Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>(request.Role));
                }
                await _userManager.AddToRoleAsync(user, request.Role);
            }

            return Unit.Value;
        }
    }
}
