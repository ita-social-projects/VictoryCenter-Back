using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgram.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyPrograms;

public class CreateHippotherapyProgramLocalizationValidator : AbstractValidator<CreateHippotherapyProgramLocalizationCommand>
{
    public CreateHippotherapyProgramLocalizationValidator(BaseHippotherapyProgramLocalizationValidator baseHippotherapyProgramLocalizationValidator)
    {
        RuleFor(x => x.CreateHippotherapyProgramLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreateHippotherapyProgramLocalizationDto>());

        RuleFor(c => c.CreateHippotherapyProgramLocalizationDto).SetValidator(baseHippotherapyProgramLocalizationValidator);
    }
}
