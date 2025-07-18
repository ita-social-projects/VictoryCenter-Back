using FluentValidation;
using VictoryCenter.BLL.Commands.FaqQuestions.Update;

namespace VictoryCenter.BLL.Validators.FaqQuestions;

public class UpdateFaqQuestionValidator : AbstractValidator<UpdateFaqQuestionCommand>
{
    public UpdateFaqQuestionValidator(BaseFaqQuestionValidator baseFaqQuestionValidator)
    {
        RuleFor(x => x.UpdateFaqQuestionDto).SetValidator(baseFaqQuestionValidator);
    }
}
