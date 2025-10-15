using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Create;

namespace VictoryCenter.BLL.Validators.HippotherapyPrograms;

public class CreateHippotherapyProgramValidator : AbstractValidator<CreateHippotherapyProgramCommand>
{
    public CreateHippotherapyProgramValidator(BaseHippotherapyProgramValidator baseProgramValidator)
    {
        RuleFor(x => x.CreateProgramDto).SetValidator(baseProgramValidator);
    }
}
