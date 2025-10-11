using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Update;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;

namespace VictoryCenter.BLL.Validators.Donate.SupportOptions;
public class UpdateSupportOptionsCommandValidator : AbstractValidator<UpdateSupportOptionsCommand>
{
    public UpdateSupportOptionsCommandValidator()
    {
        RuleFor(command => command.UpdateSupportOptionsDto)
            .SetValidator(new SupportOptionsDtoValidator<UpdateSupportOptionsDto>());
    }
}
