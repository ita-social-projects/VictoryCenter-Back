using FluentValidation;
using VictoryCenter.BLL.Commands.Public.Auth.Login;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Public.Auth;

namespace VictoryCenter.BLL.Validators.Auth;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.RequestDto.Email)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(LoginRequestDto.Email)))
            .EmailAddress().WithMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(nameof(LoginRequestDto.Email)));

        RuleFor(x => x.RequestDto.Password)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(LoginRequestDto.Password)));
    }
}
