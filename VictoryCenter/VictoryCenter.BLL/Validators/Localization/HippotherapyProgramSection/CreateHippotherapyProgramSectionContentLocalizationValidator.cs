using FluentValidation;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyProgramSection;

public class CreateHippotherapyProgramSectionContentLocalizationValidator : AbstractValidator<CreateHippotherapyProgramSectionContentLocalizationDto>
{
    public CreateHippotherapyProgramSectionContentLocalizationValidator(BaseProgramSectionContentLocalizationValidator baseProgramSectionContentLocalizationValidator)
    {
        RuleFor(x => (UpdateHippotherapyProgramSectionContentLocalizationDto)x)
            .SetValidator(baseProgramSectionContentLocalizationValidator);
    }
}
