using FluentValidation;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnalysisSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageAnalysisSection;

public class BaseHippotherapyLandingPageAnalysisSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto>
{
    public BaseHippotherapyLandingPageAnalysisSectionLocalizationValidator()
    {
        RuleFor(x => x.Title)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto.Title),
                HippotherapyLandingPageAnalysisSectionLocalizationConstants.TitleMinLength,
                HippotherapyLandingPageAnalysisSectionLocalizationConstants.TitleMaxLength);

        RuleFor(x => x.Description)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto.Description),
                HippotherapyLandingPageAnalysisSectionLocalizationConstants.DescriptionMinLength,
                HippotherapyLandingPageAnalysisSectionLocalizationConstants.DescriptionMaxLength);
    }
}
