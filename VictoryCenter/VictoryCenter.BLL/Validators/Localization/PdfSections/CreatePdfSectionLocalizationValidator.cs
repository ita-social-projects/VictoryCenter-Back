using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.PdfSections.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.PdfSections;

public class CreatePdfSectionLocalizationValidator : AbstractValidator<CreatePdfSectionLocalizationCommand>
{
    public CreatePdfSectionLocalizationValidator(BasePdfSectionLocalizationValidator basePdfSectionLocalizationValidator)
    {
        RuleFor(x => x.Dto)
            .SetValidator(new LocalizationIdentityValidator<CreatePdfSectionLocalizationDto>());
        RuleFor(x => x.Dto)
            .SetValidator(basePdfSectionLocalizationValidator);
    }
}
