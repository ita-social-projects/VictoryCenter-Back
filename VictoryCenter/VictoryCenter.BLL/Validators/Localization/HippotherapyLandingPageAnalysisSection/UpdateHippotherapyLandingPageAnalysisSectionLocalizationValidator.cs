using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAnalysisSection.Update;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageAnalysisSection;

public class UpdateHippotherapyLandingPageAnalysisSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageAnalysisSectionLocalizationCommand>
{
    public UpdateHippotherapyLandingPageAnalysisSectionLocalizationValidator(
        BaseHippotherapyLandingPageAnalysisSectionLocalizationValidator baseValidator)
    {
        RuleFor(x => x.UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
