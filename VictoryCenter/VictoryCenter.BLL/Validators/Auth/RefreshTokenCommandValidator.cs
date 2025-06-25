using FluentValidation;
using VictoryCenter.BLL.Commands.Auth.RefreshToken;

namespace VictoryCenter.BLL.Validators.Auth;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.Request.ExpiredAccessToken)
            .NotEmpty().WithMessage("Expired access token cannot be empty");

        RuleFor(x => x.Request.RefreshToken)
            .NotEmpty().WithMessage("Refresh token cannot be empty");
    }
}
