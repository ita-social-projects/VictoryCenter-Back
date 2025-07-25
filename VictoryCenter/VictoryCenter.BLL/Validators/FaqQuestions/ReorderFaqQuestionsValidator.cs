using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.FaqQuestions.Reorder;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;

namespace VictoryCenter.BLL.Validators.FaqQuestions;

public class ReorderFaqQuestionsValidator : AbstractValidator<ReorderFaqQuestionsCommand>
{
    private const int MaxFaqQuestionIds = 500;

    public ReorderFaqQuestionsValidator()
    {
        RuleFor(x => x.ReorderFaqQuestionsDto.PageId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(ReorderFaqQuestionsDto.PageId)));

        RuleFor(x => x.ReorderFaqQuestionsDto.OrderedIds)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(ReorderFaqQuestionsDto.OrderedIds)))
            .Must(ids => ids.Count <= MaxFaqQuestionIds)
            .WithMessage(ErrorMessagesConstants
                .CollectionCannotContainMoreThan(nameof(ReorderFaqQuestionsDto.OrderedIds), MaxFaqQuestionIds))
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage(ErrorMessagesConstants
                .CollectionMustContainUniqueValues(nameof(ReorderFaqQuestionsDto.OrderedIds)));

        RuleForEach(x => x.ReorderFaqQuestionsDto.OrderedIds)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustBePositive($"Each {nameof(ReorderFaqQuestionsDto.OrderedIds)} element"));
    }
}
