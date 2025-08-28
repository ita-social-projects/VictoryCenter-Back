using FluentValidation;
using VictoryCenter.BLL.Commands.Programs.Update;

namespace VictoryCenter.BLL.Validators.Programs;

public class UpdateProgramValidator : AbstractValidator<UpdateProgramCommand>
{
    public UpdateProgramValidator(BaseProgramValidator baseProgramValidator)
    {
        RuleFor(x => x.updateProgramDto).SetValidator(baseProgramValidator);
    }
}
