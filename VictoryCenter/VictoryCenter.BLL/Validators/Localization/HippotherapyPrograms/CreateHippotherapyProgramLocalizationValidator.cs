using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgram.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.BLL.Validators.Localization.Base;
using VictoryCenter.BLL.Validators.Localization.HippotherapyProgramSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyPrograms;

public class CreateHippotherapyProgramLocalizationValidator : AbstractValidator<CreateHippotherapyProgramLocalizationCommand>
{
    public CreateHippotherapyProgramLocalizationValidator(
        BaseHippotherapyProgramLocalizationValidator baseHippotherapyProgramLocalizationValidator,
        CreateHippotherapyProgramSectionLocalizationValidator sectionValidator)
    {
        RuleFor(x => x.CreateHippotherapyProgramLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreateHippotherapyProgramLocalizationDto>());

        RuleFor(c => c.CreateHippotherapyProgramLocalizationDto).SetValidator(baseHippotherapyProgramLocalizationValidator);

        RuleForEach(x => x.CreateHippotherapyProgramLocalizationDto.Sections)
            .SetValidator(sectionValidator)
            .When(x => x.CreateHippotherapyProgramLocalizationDto.Sections != null);
    }
}
