using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Update;

namespace VictoryCenter.BLL.Validators.Donate.SupportOptions;

public class UpdateSupportOptionsCommandValidator : AbstractValidator<UpdateSupportOptionsCommand>
{
    public UpdateSupportOptionsCommandValidator()
    {
        RuleFor(command => command.UpdateSupportOptionsDto)
            .SetValidator(new BaseSupportOptionsDtoValidator());
    }
}
