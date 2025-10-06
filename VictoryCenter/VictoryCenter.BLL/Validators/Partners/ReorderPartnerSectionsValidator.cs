using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Partners.Reorder;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;

namespace VictoryCenter.BLL.Validators.Partners;

public class ReorderPartnerSectionsValidator : AbstractValidator<ReorderPartnersSectionsCommand>
{
    public ReorderPartnerSectionsValidator()
    {
        RuleFor(x => x.ReorderDto.OrderedIds)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(ReorderFaqQuestionsDto.OrderedIds)))
            .Must(ids => ids.Count <= PartnerConstants.PartnersMaxCount)
            .WithMessage(ErrorMessagesConstants
                .CollectionCannotContainMoreThan(nameof(ReorderFaqQuestionsDto.OrderedIds), PartnerConstants.PartnersMaxCount))
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage(ErrorMessagesConstants
                .CollectionMustContainUniqueValues(nameof(ReorderFaqQuestionsDto.OrderedIds)));

        RuleForEach(x => x.ReorderDto.OrderedIds)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustBePositive($"Each {nameof(ReorderFaqQuestionsDto.OrderedIds)} element"));
    }
}
