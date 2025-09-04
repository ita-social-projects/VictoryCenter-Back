using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Programs.Create;

namespace VictoryCenter.BLL.Validators.Programs;

public class CreateProgramValidator : AbstractValidator<CreateProgramCommand>
{
    public CreateProgramValidator(BaseProgramValidator baseProgramValidator)
    {
        RuleFor(x => x.CreateProgramDto).SetValidator(baseProgramValidator);
    }
}
