using FluentValidation;
using VictoryCenter.BLL.Commands.Auth.Login;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.Auth;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.RequestDto.Email)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired("Email"))
            .EmailAddress().WithMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat("Email"));

        RuleFor(x => x.RequestDto.Password)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired("Password"));
    }
}
