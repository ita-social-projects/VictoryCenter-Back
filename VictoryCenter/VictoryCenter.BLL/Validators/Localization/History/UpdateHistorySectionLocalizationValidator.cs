using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.History.Update;

namespace VictoryCenter.BLL.Validators.Localization.History;

public class UpdateHistorySectionLocalizationValidator : AbstractValidator<UpdateHistoryLocalizationCommand>
{
    public UpdateHistorySectionLocalizationValidator(BaseHistorySectionContentLocalizationValidator contentValidator)
    {
        RuleForEach(x => x.UpdateHistorySectionLocalizationDtos)
            .ChildRules(section =>
            {
                section.RuleForEach(s => s.Contents)
                    .SetValidator(contentValidator)
                    .When(s => s.Contents != null);
            })
            .When(x => x.UpdateHistorySectionLocalizationDtos != null);
    }
}
