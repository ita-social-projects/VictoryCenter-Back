using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;

namespace VictoryCenter.BLL.Validators.Donate.SupportOptions;

public class SupportOptionsDtoValidator<T> : AbstractValidator<T>
    where T : ISupportOptions
{
    public SupportOptionsDtoValidator()
    {
        RuleFor(dto => dto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SupportOptionsDto.Name)));

        RuleFor(dto => dto.Value)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SupportOptionsDto.Value)));
    }
}
