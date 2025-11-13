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
            .MaximumLength(SupportOptionsConstants.NameMaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(SupportOptionsDto.Name),
                    SupportOptionsConstants.NameMaxLength));

        RuleFor(dto => dto.Value)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SupportOptionsDto.Value)))
            .MaximumLength(SupportOptionsConstants.ValueMaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(SupportOptionsDto.Value),
                    SupportOptionsConstants.ValueMaxLength));
    }
}
