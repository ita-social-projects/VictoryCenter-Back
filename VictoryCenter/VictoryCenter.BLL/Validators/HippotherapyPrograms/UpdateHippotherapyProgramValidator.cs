using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Update;

namespace VictoryCenter.BLL.Validators.HippotherapyPrograms;

public class UpdateHippotherapyProgramValidator : AbstractValidator<UpdateHippotherapyProgramCommand>
{
    public UpdateHippotherapyProgramValidator(BaseHippotherapyProgramValidator baseProgramValidator)
    {
        RuleFor(x => x.UpdateProgramDto).SetValidator(baseProgramValidator);
    }
}
