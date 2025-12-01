using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Create;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.Donate.SupportOptions;

public class CreateSupportOptionsCommandValidator : AbstractValidator<CreateSupportOptionsCommand>
{
    public CreateSupportOptionsCommandValidator()
    {
        RuleFor(command => command.CreateSupportOptionsDto)
            .SetValidator(new BaseSupportOptionsDtoValidator());

        RuleFor(command => command.CreateSupportOptionsDto.Currency)
            .IsInEnum()
            .WithMessage(SupportOptionsConstants.OnlyUsdOrEurOrUahMessage);
    }
}
