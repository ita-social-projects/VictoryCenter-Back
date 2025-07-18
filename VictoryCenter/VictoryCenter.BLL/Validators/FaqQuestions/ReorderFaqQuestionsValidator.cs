using FluentValidation;
using VictoryCenter.BLL.Commands.FaqQuestions.Reorder;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.FaqQuestions;

namespace VictoryCenter.BLL.Validators.FaqQuestions;

public class ReorderFaqQuestionsValidator : AbstractValidator<ReorderFaqQuestionsCommand>
{
    private const int MaxFaqQuestionIds = 500;

    public ReorderFaqQuestionsValidator(BaseFaqQuestionValidator baseFaqQuestionValidator)
    {
        RuleFor(x => x.ReorderFaqQuestionsDto.PageId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive("PageId"));

        RuleFor(x => x.ReorderFaqQuestionsDto.OrderedIds)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ReorderFaqQuestionsDto.OrderedIds)))
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(ReorderFaqQuestionsDto.OrderedIds)))
            .Must(ids => ids.Count <= MaxFaqQuestionIds)
            .WithMessage(ErrorMessagesConstants
                .CollectionCannotContainMoreThan(nameof(ReorderFaqQuestionsDto.OrderedIds), MaxFaqQuestionIds))
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage(ErrorMessagesConstants
                .CollectionContainsDuplicateValues(nameof(ReorderFaqQuestionsDto.OrderedIds)));

        RuleForEach(x => x.ReorderFaqQuestionsDto.OrderedIds)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustBePositive($"Each {nameof(ReorderFaqQuestionsDto.OrderedIds)} element"));
    }
}
