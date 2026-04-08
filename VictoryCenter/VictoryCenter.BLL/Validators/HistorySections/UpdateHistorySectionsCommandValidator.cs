using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.History.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HistorySection;

namespace VictoryCenter.BLL.Validators.HistorySections;

public class UpdateHistorySectionsCommandValidator : AbstractValidator<UpdateHistorySectionsCommand>
{
    public UpdateHistorySectionsCommandValidator(UpdateHistorySectionValidator sectionValidator)
    {
        RuleFor(x => x.UpdateSections)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHistorySectionsCommand.UpdateSections)));

        RuleFor(x => x.UpdateSections)
            .Must(HasUniqueSectionOrders)
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(UpdateHistorySectionDto.Order)));

        RuleForEach(x => x.UpdateSections)
            .SetValidator(sectionValidator);
    }

    private static bool HasUniqueSectionOrders(List<UpdateHistorySectionDto>? sections)
    {
        if (sections is null)
        {
            return true;
        }

        return sections.Select(x => x.Order).Distinct().Count() == sections.Count;
    }
}
