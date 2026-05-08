using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.History.Update;

namespace VictoryCenter.BLL.Validators.Localization.History;

public class UpdateHistorySectionLocalizationValidator : AbstractValidator<UpdateHistoryLocalizationCommand>
{
    public UpdateHistorySectionLocalizationValidator(BaseHistorySectionContentLocalizationValidator contentValidator)
    {
        RuleForEach(x => x.UpdateHistorySectionLocalizationDto.Contents)
            .SetValidator(contentValidator)
            .When(x => x.UpdateHistorySectionLocalizationDto.Contents != null);
    }
}
