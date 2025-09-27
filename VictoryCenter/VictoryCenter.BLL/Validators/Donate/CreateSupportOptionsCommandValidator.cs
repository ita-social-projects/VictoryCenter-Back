using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;

namespace VictoryCenter.BLL.Validators.Donate;
public class CreateSupportOptionsCommandValidator : AbstractValidator<CreateSupportOptionsCommand>
{
    public CreateSupportOptionsCommandValidator()
    {
        RuleFor(command => command.CreateSupportOptionsDto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SupportOptionsDto.Name)));

        RuleFor(command => command.CreateSupportOptionsDto.Value)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SupportOptionsDto.Value)));
    }
}
