using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.History.Update;

namespace VictoryCenter.BLL.Validators.Localization.History;

public class UpdateHistorySectionLocalizationValidator : AbstractValidator<UpdateHistoryLocalizationCommand>
{
    public UpdateHistorySectionLocalizationValidator(BaseHistorySectionContentLocalizationValidator contentValidator)
    {
        RuleFor(x => x.UpdateHistorySectionLocalizationDtos)
            .NotNull()
            .NotEmpty()
            .WithMessage("History section localizations must be provided");

        RuleForEach(x => x.UpdateHistorySectionLocalizationDtos)
            .ChildRules(section =>
            {
                section.RuleFor(s => s.Contents)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Section must have at least one content localization");

                section.RuleForEach(s => s.Contents)
                    .SetValidator(contentValidator)
                    .When(s => s.Contents != null && s.Contents.Any());
            })
            .When(x => x.UpdateHistorySectionLocalizationDtos != null && x.UpdateHistorySectionLocalizationDtos.Any());
    }
}
