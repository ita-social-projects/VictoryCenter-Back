using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.FaqQuestions.Update;

namespace VictoryCenter.BLL.Validators.Localization.FaqQuestions;

public class UpdateFaqQuestionLocalizationValidator : AbstractValidator<UpdateFaqQuestionLocalizationCommand>
{
    public UpdateFaqQuestionLocalizationValidator(BaseFaqQuestionLocalizationValidator baseFaqQuestionLocalizationsValidator)
    {
        RuleFor(c => c.UpdateFaqQuestionLocalizationDto).SetValidator(baseFaqQuestionLocalizationsValidator);
    }
}
