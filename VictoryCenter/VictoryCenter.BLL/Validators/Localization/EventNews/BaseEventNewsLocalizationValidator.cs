using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNews;

namespace VictoryCenter.BLL.Validators.Localization.EventNews;

public class BaseEventNewsLocalizationValidator : AbstractValidator<UpdateEventNewsLocalizationDto>
{
    public BaseEventNewsLocalizationValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateEventNewsLocalizationDto.Title)))
            .MinimumLength(EventNewsConstants.TitleMinLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UpdateEventNewsLocalizationDto.Title), EventNewsConstants.TitleMinLength))
            .MaximumLength(EventNewsConstants.TitleMaxLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UpdateEventNewsLocalizationDto.Title), EventNewsConstants.TitleMaxLength));

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateEventNewsLocalizationDto.Description)))
            .MinimumLength(EventNewsConstants.DescriptionMinLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UpdateEventNewsLocalizationDto.Description), EventNewsConstants.DescriptionMinLength))
            .MaximumLength(EventNewsConstants.DescriptionMaxLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UpdateEventNewsLocalizationDto.Description), EventNewsConstants.DescriptionMaxLength));

        RuleFor(x => x.AdditionalDescription)
            .MinimumLength(EventNewsConstants.AdditionalDescriptionMinLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UpdateEventNewsLocalizationDto.AdditionalDescription), EventNewsConstants.AdditionalDescriptionMinLength))
            .MaximumLength(EventNewsConstants.AdditionalDescriptionMaxLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UpdateEventNewsLocalizationDto.AdditionalDescription), EventNewsConstants.AdditionalDescriptionMaxLength))
            .When(x => !string.IsNullOrWhiteSpace(x.AdditionalDescription));
    }
}
