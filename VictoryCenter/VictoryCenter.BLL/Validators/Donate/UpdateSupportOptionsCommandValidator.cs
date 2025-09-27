using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;

namespace VictoryCenter.BLL.Validators.Donate;
public class UpdateSupportOptionsCommandValidator : AbstractValidator<UpdateSupportOptionsCommand>
{
    public UpdateSupportOptionsCommandValidator()
    {
        RuleFor(command => command.UpdateSupportOptionsDto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SupportOptionsDto.Name)));

        RuleFor(command => command.UpdateSupportOptionsDto.Value)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SupportOptionsDto.Value)));
    }
}
