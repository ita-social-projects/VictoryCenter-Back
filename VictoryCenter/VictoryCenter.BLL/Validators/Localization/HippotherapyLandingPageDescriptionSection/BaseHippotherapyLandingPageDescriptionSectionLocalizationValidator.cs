using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageDescriptionSection;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageDescriptionSection;

public class BaseHippotherapyLandingPageDescriptionSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto>
{
    public BaseHippotherapyLandingPageDescriptionSectionLocalizationValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto.Title)))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length >= HippotherapyLandingPageDescriptionSectionLocalizationConstants.TitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto.Title),
                HippotherapyLandingPageDescriptionSectionLocalizationConstants.TitleMinLength))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length <= HippotherapyLandingPageDescriptionSectionLocalizationConstants.TitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto.Title),
                HippotherapyLandingPageDescriptionSectionLocalizationConstants.TitleMaxLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto.Description)))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length >= HippotherapyLandingPageDescriptionSectionLocalizationConstants.DescriptionMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto.Description),
                HippotherapyLandingPageDescriptionSectionLocalizationConstants.DescriptionMinLength))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length <= HippotherapyLandingPageDescriptionSectionLocalizationConstants.DescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto.Description),
                HippotherapyLandingPageDescriptionSectionLocalizationConstants.DescriptionMaxLength));
    }
}
