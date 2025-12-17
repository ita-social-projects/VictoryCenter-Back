using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.FaqQuestions.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.FaqQuestions;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.FaqQuestions;

public class CreateFaqQuestionLocalizationValidator : AbstractValidator<CreateFaqQuestionLocalizationCommand>
{
    public CreateFaqQuestionLocalizationValidator(BaseFaqQuestionLocalizationValidator baseFaqQuestionLocalizationValidator)
    {
        RuleFor(x => x.CreateFaqQuestionLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreateFaqQuestionLocalizationDto>());

        RuleFor(c => c.CreateFaqQuestionLocalizationDto).SetValidator(baseFaqQuestionLocalizationValidator);
    }
}
