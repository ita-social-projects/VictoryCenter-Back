using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.HypotherapyPrograms.Update;

namespace VictoryCenter.BLL.Validators.HypotherapyPrograms;

public class UpdateHypotherapyProgramValidator : AbstractValidator<UpdateHypotherapyProgramCommand>
{
    public UpdateHypotherapyProgramValidator(BaseHypotherapyProgramValidator baseProgramValidator)
    {
        RuleFor(x => x.UpdateProgramDto).SetValidator(baseProgramValidator);
    }
}
