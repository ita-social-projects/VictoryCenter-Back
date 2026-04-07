using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.History.Update;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.HistorySections;

public class UpdateHistorySectionsCommandValidator : AbstractValidator<UpdateHistorySectionsCommand>
{
    public UpdateHistorySectionsCommandValidator(UpdateHistorySectionValidator sectionValidator)
    {
        RuleFor(x => x.UpdateSections)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHistorySectionsCommand.UpdateSections)));

        RuleForEach(x => x.UpdateSections)
            .SetValidator(sectionValidator);
    }
}
