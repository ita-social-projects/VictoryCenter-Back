using FluentValidation;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Common;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyProgramSection;

public class CreateHippotherapyProgramSectionContentLocalizationValidator : AbstractValidator<CreateHippotherapyProgramSectionContentLocalizationDto>
{
    public CreateHippotherapyProgramSectionContentLocalizationValidator(BaseProgramSectionContentLocalizationValidator baseProgramSectionContentLocalizationValidator)
    {
        RuleFor(x => (BaseHippotherapyProgramSectionContentLocalizationDto)x)
            .SetValidator(baseProgramSectionContentLocalizationValidator);
    }
}
