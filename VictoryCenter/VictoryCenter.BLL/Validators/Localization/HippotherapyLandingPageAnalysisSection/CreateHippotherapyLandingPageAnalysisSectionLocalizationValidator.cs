using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAnalysisSection.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnalysisSection;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageAnalysisSection;

public class CreateHippotherapyLandingPageAnalysisSectionLocalizationValidator
    : AbstractValidator<CreateHippotherapyLandingPageAnalysisSectionLocalizationCommand>
{
    public CreateHippotherapyLandingPageAnalysisSectionLocalizationValidator(
        BaseHippotherapyLandingPageAnalysisSectionLocalizationValidator baseValidator)
    {
        RuleFor(c => c.CreateHippotherapyLandingPageAnalysisSectionLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreateHippotherapyLandingPageAnalysisSectionLocalizationDto>());
        RuleFor(c => c.CreateHippotherapyLandingPageAnalysisSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
