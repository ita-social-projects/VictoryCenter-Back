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
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SupportOptionsDto.Name)))
            .MaximumLength(SupportOptionsConstants.Name.MaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(SupportOptionsDto.Name),
                    SupportOptionsConstants.Name.MaxLength));

        RuleFor(dto => dto.Value)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SupportOptionsDto.Value)))
            .MaximumLength(SupportOptionsConstants.Value.MaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(SupportOptionsDto.Value),
                    SupportOptionsConstants.Value.MaxLength));
    }
}
