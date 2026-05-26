using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.History.Create;
namespace VictoryCenter.BLL.Validators.Localization.History;

public class CreateHistorySectionLocalizationValidator : AbstractValidator<CreateHistoryLocalizationCommand>
{
    public CreateHistorySectionLocalizationValidator(BaseHistorySectionContentLocalizationValidator contentValidator)
    {
        RuleFor(x => x.CreateHistorySectionLocalizationDtos)
            .NotNull()
            .NotEmpty()
            .WithMessage("History section localizations must be provided");

        RuleForEach(x => x.CreateHistorySectionLocalizationDtos)
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
            .When(x => x.CreateHistorySectionLocalizationDtos != null && x.CreateHistorySectionLocalizationDtos.Any());
    }
}
