using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.HypotherapyPrograms.Create;

namespace VictoryCenter.BLL.Validators.HypotherapyPrograms;

public class CreateHypotherapyProgramValidator : AbstractValidator<CreateHypotherapyProgramCommand>
{
    public CreateHypotherapyProgramValidator(BaseHypotherapyProgramValidator baseProgramValidator)
    {
        RuleFor(x => x.CreateProgramDto).SetValidator(baseProgramValidator);
    }
}
