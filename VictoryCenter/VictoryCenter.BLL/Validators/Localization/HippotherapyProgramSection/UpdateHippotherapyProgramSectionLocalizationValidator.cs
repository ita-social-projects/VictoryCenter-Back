using FluentValidation;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Update;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyProgramSection;
public class UpdateHippotherapyProgramSectionLocalizationValidator : AbstractValidator<UpdateHippotherapyProgramSectionLocalizationDto>
{
    public UpdateHippotherapyProgramSectionLocalizationValidator(
        UpdateHippotherapyProgramSectionContentLocalizationValidator updateContentValidator)
    {
        RuleFor(x => x.EntityId)
            .GreaterThan(0).WithMessage("EntityId must be greater than 0.");
        RuleForEach(x => x.Contents)
            .SetValidator(updateContentValidator)
            .When(x => x.Contents != null);
    }
}
