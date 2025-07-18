using FluentValidation;
using VictoryCenter.BLL.Commands.FaqQuestions.Create;

namespace VictoryCenter.BLL.Validators.FaqQuestions;

public class CreateFaqQuestionValidator : AbstractValidator<CreateFaqQuestionCommand>
{
    public CreateFaqQuestionValidator(BaseFaqQuestionValidator baseFaqQuestionValidator)
    {
        RuleFor(x => x.CreateFaqQuestionDto).SetValidator(baseFaqQuestionValidator);
    }
}
