using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Programs.Update;

namespace VictoryCenter.BLL.Validators.Programs;

public class UpdateProgramValidator : AbstractValidator<UpdateProgramCommand>
{
    public UpdateProgramValidator(BaseProgramValidator baseProgramValidator)
    {
        RuleFor(x => x.UpdateProgramDto).SetValidator(baseProgramValidator);
    }
}
