using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;

namespace VictoryCenter.BLL.Validators.FaqQuestions;

public class BaseFaqQuestionValidator : AbstractValidator<CreateFaqQuestionDto>
{
    public BaseFaqQuestionValidator()
    {
        RuleFor(x => x.QuestionText)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateFaqQuestionDto.QuestionText)))
            .MinimumLength(FaqConstants.QuestionTextMinLength)
            .WithMessage(ErrorMessagesConstants
                            .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CreateFaqQuestionDto.QuestionText), FaqConstants.QuestionTextMinLength))
            .MaximumLength(FaqConstants.QuestionTextMaxLength)
            .WithMessage(ErrorMessagesConstants
                            .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreateFaqQuestionDto.QuestionText), FaqConstants.QuestionTextMaxLength));

        RuleFor(x => x.AnswerText)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateFaqQuestionDto.AnswerText)))
            .MinimumLength(FaqConstants.AnswerTextMinLength)
            .WithMessage(ErrorMessagesConstants
                            .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CreateFaqQuestionDto.AnswerText), FaqConstants.AnswerTextMinLength))
            .MaximumLength(FaqConstants.AnswerTextMaxLength)
            .WithMessage(ErrorMessagesConstants
                            .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreateFaqQuestionDto.AnswerText), FaqConstants.AnswerTextMaxLength));

        RuleFor(x => x.PageIds)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(CreateFaqQuestionDto.PageIds)))
            .Must(pageIds => pageIds.Distinct().Count() == pageIds.Count)
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(CreateFaqQuestionDto.PageIds)));

        RuleForEach(x => x.PageIds)
            .GreaterThan(0)
            .WithMessage(id => ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateFaqQuestionDto.PageIds)));

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage(ErrorMessagesConstants.UnknownStatusValue);
    }
}
