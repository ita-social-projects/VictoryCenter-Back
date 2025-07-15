using FluentValidation;
using VictoryCenter.BLL.Commands.Auth.Login;

namespace VictoryCenter.BLL.Validators.Auth;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.RequestDto.Email)
            .NotEmpty().WithMessage("Email cannot be empty")
            .EmailAddress().WithMessage("Email address must be in a valid format");

        RuleFor(x => x.RequestDto.Password)
            .NotEmpty().WithMessage("Password cannot be empty");
    }
}
