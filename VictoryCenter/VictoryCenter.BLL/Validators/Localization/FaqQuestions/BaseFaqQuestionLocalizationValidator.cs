using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.FaqQuestions;

namespace VictoryCenter.BLL.Validators.Localization.FaqQuestions;

public class BaseFaqQuestionLocalizationValidator : AbstractValidator<UpdateFaqQuestionLocalizationDto>
{
    public BaseFaqQuestionLocalizationValidator()
    {
        RuleFor(x => x.QuestionText)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateFaqQuestionLocalizationDto.QuestionText)))
            .MinimumLength(FaqQuestionLocalizationConstants.QuestionTextMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateFaqQuestionLocalizationDto.QuestionText),
                FaqQuestionLocalizationConstants.QuestionTextMinLength))
            .MaximumLength(FaqQuestionLocalizationConstants.QuestionTextMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateFaqQuestionLocalizationDto.QuestionText),
                FaqQuestionLocalizationConstants.QuestionTextMaxLength));
        RuleFor(x => x.AnswerText)
            .MinimumLength(FaqQuestionLocalizationConstants.AnswerTextMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateFaqQuestionLocalizationDto.AnswerText),
                FaqQuestionLocalizationConstants.AnswerTextMinLength))
            .MaximumLength(FaqQuestionLocalizationConstants.AnswerTextMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateFaqQuestionLocalizationDto.AnswerText),
                FaqQuestionLocalizationConstants.AnswerTextMaxLength));
    }
}
