using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgram.Update;
using VictoryCenter.BLL.Validators.Localization.HippotherapyProgramSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyPrograms;
public class UpdateHippotherapyProgramLocalizationValidator : AbstractValidator<UpdateHippotherapyProgramLocalizationCommand>
{
    public UpdateHippotherapyProgramLocalizationValidator(
        BaseHippotherapyProgramLocalizationValidator baseHippotherapyProgramLocalizationValidator,
        UpdateHippotherapyProgramSectionLocalizationValidator updateHippotherapyProgramSectionLocalizationValidator)
    {
        RuleFor(c => c.UpdateHippotherapyProgramLocalizationDto).SetValidator(baseHippotherapyProgramLocalizationValidator);

        RuleForEach(x => x.UpdateHippotherapyProgramLocalizationDto.Sections)
            .SetValidator(updateHippotherapyProgramSectionLocalizationValidator)
            .When(x => x.UpdateHippotherapyProgramLocalizationDto.Sections != null);
    }
}
