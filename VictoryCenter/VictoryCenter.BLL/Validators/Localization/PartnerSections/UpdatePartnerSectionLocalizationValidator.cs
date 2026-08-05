using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.PartnerSections.Update;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.Localization.PartnerSections;

public class UpdatePartnerSectionLocalizationValidator : AbstractValidator<UpdatePartnerSectionLocalizationCommand>
{
    public UpdatePartnerSectionLocalizationValidator(BasePartnerSectionLocalizationValidator baseValidator)
    {
        RuleFor(x => x.UpdatePartnerSectionLocalizationDto)
            .NotNull().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdatePartnerSectionLocalizationCommand.UpdatePartnerSectionLocalizationDto)))
            .SetValidator(baseValidator);
    }
}
