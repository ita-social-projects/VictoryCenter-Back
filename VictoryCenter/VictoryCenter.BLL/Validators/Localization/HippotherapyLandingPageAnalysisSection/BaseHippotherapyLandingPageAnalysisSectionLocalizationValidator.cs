using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnalysisSection;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageAnalysisSection;

public class BaseHippotherapyLandingPageAnalysisSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto>
{
    public BaseHippotherapyLandingPageAnalysisSectionLocalizationValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto.Title)))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length >= HippotherapyLandingPageAnalysisSectionLocalizationConstants.TitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto.Title),
                HippotherapyLandingPageAnalysisSectionLocalizationConstants.TitleMinLength))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length <= HippotherapyLandingPageAnalysisSectionLocalizationConstants.TitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto.Title),
                HippotherapyLandingPageAnalysisSectionLocalizationConstants.TitleMaxLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto.Description)))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length >= HippotherapyLandingPageAnalysisSectionLocalizationConstants.DescriptionMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto.Description),
                HippotherapyLandingPageAnalysisSectionLocalizationConstants.DescriptionMinLength))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length <= HippotherapyLandingPageAnalysisSectionLocalizationConstants.DescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto.Description),
                HippotherapyLandingPageAnalysisSectionLocalizationConstants.DescriptionMaxLength));
    }
}
