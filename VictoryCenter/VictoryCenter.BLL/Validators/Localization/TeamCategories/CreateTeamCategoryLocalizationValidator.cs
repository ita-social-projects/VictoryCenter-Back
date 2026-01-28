using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamCategories.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.TeamCategories;
public class CreateTeamCategoryLocalizationValidator : AbstractValidator<CreateTeamCategoryLocalizationCommand>
{
    public CreateTeamCategoryLocalizationValidator(BaseTeamCategoryLocalizationValidator baseTeamCategoryLocalizationValidator)
    {
        RuleFor(c => c.CreateTeamCategoryLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreateTeamCategoryLocalizationDto>());
        RuleFor(c => c.CreateTeamCategoryLocalizationDto)
            .SetValidator(baseTeamCategoryLocalizationValidator);
    }
}
