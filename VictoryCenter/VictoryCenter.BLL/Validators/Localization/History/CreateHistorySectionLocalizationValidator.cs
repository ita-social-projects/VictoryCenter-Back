using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.History.Create;
namespace VictoryCenter.BLL.Validators.Localization.History;

public class CreateHistorySectionLocalizationValidator : AbstractValidator<CreateHistoryLocalizationCommand>
{
    public CreateHistorySectionLocalizationValidator(CreateHistorySectionContentLocalizationValidator contentValidator)
    {
        RuleForEach(x => x.CreateHistorySectionLocalizationDto.Contents)
            .SetValidator(contentValidator)
            .When(x => x.CreateHistorySectionLocalizationDto.Contents != null);
    }
}
