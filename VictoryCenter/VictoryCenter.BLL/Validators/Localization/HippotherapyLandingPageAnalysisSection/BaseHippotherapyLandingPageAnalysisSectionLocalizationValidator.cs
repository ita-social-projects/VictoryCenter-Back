using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnalysisSection;

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
            .MinimumLength(HippotherapyLandingPageAnalysisSectionLocalizationConstants.TitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto.Title),
                HippotherapyLandingPageAnalysisSectionLocalizationConstants.TitleMinLength))
            .MaximumLength(HippotherapyLandingPageAnalysisSectionLocalizationConstants.TitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto.Title),
                HippotherapyLandingPageAnalysisSectionLocalizationConstants.TitleMaxLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto.Description)))
            .MinimumLength(HippotherapyLandingPageAnalysisSectionLocalizationConstants.DescriptionMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto.Description),
                HippotherapyLandingPageAnalysisSectionLocalizationConstants.DescriptionMinLength))
            .MaximumLength(HippotherapyLandingPageAnalysisSectionLocalizationConstants.DescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto.Description),
                HippotherapyLandingPageAnalysisSectionLocalizationConstants.DescriptionMaxLength));
    }
}
