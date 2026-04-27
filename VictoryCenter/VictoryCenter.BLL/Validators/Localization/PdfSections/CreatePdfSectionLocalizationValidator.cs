using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.PdfSection.Create;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.Localization.PdfSections;

public class CreatePdfSectionLocalizationValidator : AbstractValidator<CreatePdfSectionLocalizationCommand>
{
    public CreatePdfSectionLocalizationValidator(BasePdfSectionLocalizationValidator basePdfSectionLocalizationValidator)
    {
        RuleFor(x => x.Dto.LanguageId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive("LanguageId"));

        RuleFor(x => x.Dto)
            .SetValidator(basePdfSectionLocalizationValidator);
    }
}
