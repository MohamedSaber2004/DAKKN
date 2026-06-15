using MediatR;

namespace DAKKN.Application.Features.Auth.Comands.VerifyForgetPasswordOtp
{
    public record VerifyForgetPasswordOtpCommand(string Email, string Otp) : IRequest<bool>;
}
