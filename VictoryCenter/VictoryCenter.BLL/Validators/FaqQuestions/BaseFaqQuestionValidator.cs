using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.FaqQuestions;

namespace VictoryCenter.BLL.Validators.FaqQuestions;

public class BaseFaqQuestionValidator : AbstractValidator<CreateFaqQuestionDto>
{
    private const short QuestionTextMinLength = 10;
    private const short QuestionTextMaxLength = 150;
    private const short AnswerTextMinLength = 50;
    private const short AnswerTextMaxLength = 1000;

    public BaseFaqQuestionValidator()
    {
        RuleFor(x => x.QuestionText)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Question Text"))
            .MinimumLength(QuestionTextMinLength)
            .WithMessage(ErrorMessagesConstants
                            .PropertyMustHaveAMinimumLengthOfNCharacters("Question Text", QuestionTextMinLength))
            .MaximumLength(QuestionTextMaxLength)
            .WithMessage(ErrorMessagesConstants
                            .PropertyMustHaveAMaximumLengthOfNCharacters("Question Text", QuestionTextMaxLength));

        RuleFor(x => x.AnswerText)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Answer Text"))
            .MinimumLength(AnswerTextMinLength)
            .WithMessage(ErrorMessagesConstants
                            .PropertyMustHaveAMinimumLengthOfNCharacters("Answer Text", AnswerTextMinLength))
            .MaximumLength(AnswerTextMaxLength)
            .WithMessage(ErrorMessagesConstants
                            .PropertyMustHaveAMaximumLengthOfNCharacters("Answer Text", AnswerTextMaxLength));

        RuleFor(x => x.PageIds)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty("PageIds"));

        RuleForEach(x => x.PageIds)
            .GreaterThan(0)
            .WithMessage(id => ErrorMessagesConstants.PropertyMustBePositive("PageIds"));

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage(ErrorMessagesConstants.UnknownStatusValue);
    }
}
