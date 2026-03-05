using FluentValidation;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Common;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Update;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyProgramSection;
public class UpdateHippotherapyProgramSectionContentLocalizationValidator : AbstractValidator<UpdateHippotherapyProgramSectionContentLocalizationDto>
{
    public UpdateHippotherapyProgramSectionContentLocalizationValidator(BaseProgramSectionContentLocalizationValidator baseProgramSectionContentLocalizationValidator)
    {
        RuleFor(x => x.EntityId)
            .GreaterThan(0).WithMessage("EntityId must be greater than 0.");
        RuleFor(x => (BaseHippotherapyProgramSectionContentLocalizationDto)x)
            .SetValidator(baseProgramSectionContentLocalizationValidator);
    }
}
