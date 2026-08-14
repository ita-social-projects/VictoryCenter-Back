using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgramCategories.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramCategories;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyProgramCategories;

public class CreateHippotherapyProgramCategoryLocalizationValidator
    : AbstractValidator<CreateHippotherapyProgramCategoryLocalizationCommand>
{
    public CreateHippotherapyProgramCategoryLocalizationValidator(
        BaseHippotherapyProgramCategoryLocalizationValidator baseValidator)
    {
        RuleFor(c => c.CreateHippotherapyProgramCategoryLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreateHippotherapyProgramCategoryLocalizationDto>());
        RuleFor(c => c.CreateHippotherapyProgramCategoryLocalizationDto)
            .SetValidator(baseValidator);
    }
}
