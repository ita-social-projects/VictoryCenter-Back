using FluentValidation;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyProgramSection;

public class CreateHippotherapyProgramSectionLocalizationValidator : AbstractValidator<CreateHippotherapyProgramSectionLocalizationDto>
{
    public CreateHippotherapyProgramSectionLocalizationValidator(CreateHippotherapyProgramSectionContentLocalizationValidator contentValidator)
    {
        RuleForEach(x => x.Contents)
            .SetValidator(contentValidator)
            .When(x => x.Contents != null);
    }
}
