using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;

namespace VictoryCenter.BLL.Validators.FaqQuestions;

public class BaseFaqQuestionValidator : AbstractValidator<CreateFaqQuestionDto>
{
    public static readonly short QuestionTextMinLength = 10;
    public static readonly short QuestionTextMaxLength = 150;
    public static readonly short AnswerTextMinLength = 50;
    public static readonly short AnswerTextMaxLength = 1000;

    public BaseFaqQuestionValidator()
    {
        RuleFor(x => x.QuestionText)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateFaqQuestionDto.QuestionText)))
            .MinimumLength(QuestionTextMinLength)
            .WithMessage(ErrorMessagesConstants
                            .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CreateFaqQuestionDto.QuestionText), QuestionTextMinLength))
            .MaximumLength(QuestionTextMaxLength)
            .WithMessage(ErrorMessagesConstants
                            .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreateFaqQuestionDto.QuestionText), QuestionTextMaxLength));

        RuleFor(x => x.AnswerText)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateFaqQuestionDto.AnswerText)))
            .MinimumLength(AnswerTextMinLength)
            .WithMessage(ErrorMessagesConstants
                            .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CreateFaqQuestionDto.AnswerText), AnswerTextMinLength))
            .MaximumLength(AnswerTextMaxLength)
            .WithMessage(ErrorMessagesConstants
                            .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreateFaqQuestionDto.AnswerText), AnswerTextMaxLength));

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
