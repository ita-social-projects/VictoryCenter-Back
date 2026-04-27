using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.PdfSection.Update;

namespace VictoryCenter.BLL.Validators.Localization.PdfSections;

public class UpdatePdfSectionLocalizationValidator : AbstractValidator<UpdatePdfSectionLocalizationCommand>
{
    public UpdatePdfSectionLocalizationValidator(BasePdfSectionLocalizationValidator basePdfSectionLocalizationValidator)
    {
        RuleFor(x => x.UpdatePdfSectionLocalizationDto)
            .SetValidator(basePdfSectionLocalizationValidator);
    }
}
