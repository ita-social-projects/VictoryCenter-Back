using FluentValidation;
using VictoryCenter.BLL.Commands.Programs.Create;

namespace VictoryCenter.BLL.Validators.Programs;

public class CreateProgramValidator : AbstractValidator<CreateProgramCommand>
{
    public CreateProgramValidator(BaseProgramValidator baseProgramValidator)
    {
        RuleFor(x => x.createProgramDto).SetValidator(baseProgramValidator);
    }
}
