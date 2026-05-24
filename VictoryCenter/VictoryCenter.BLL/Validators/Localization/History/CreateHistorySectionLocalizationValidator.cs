using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.History.Create;
namespace VictoryCenter.BLL.Validators.Localization.History;

public class CreateHistorySectionLocalizationValidator : AbstractValidator<CreateHistoryLocalizationCommand>
{
    public CreateHistorySectionLocalizationValidator(BaseHistorySectionContentLocalizationValidator contentValidator)
    {
        RuleForEach(x => x.CreateHistorySectionLocalizationDtos)
            .ChildRules(section =>
            {
                section.RuleForEach(s => s.Contents)
                    .SetValidator(contentValidator)
                    .When(s => s.Contents != null);
            })
            .When(x => x.CreateHistorySectionLocalizationDtos != null);
    }
}
