using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;

namespace VictoryCenter.BLL.Validators.Localization.PdfSections;

public class BasePdfSectionLocalizationValidator : AbstractValidator<UpdatePdfSectionLocalizationDto>
{
    public BasePdfSectionLocalizationValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdatePdfSectionLocalizationDto.Title)))
            .MinimumLength(PdfSectionLocalizationConstants.TitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdatePdfSectionLocalizationDto.Title), PdfSectionLocalizationConstants.TitleMinLength))
            .MaximumLength(PdfSectionLocalizationConstants.TitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdatePdfSectionLocalizationDto.Title), PdfSectionLocalizationConstants.TitleMaxLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdatePdfSectionLocalizationDto.Description)))
            .MinimumLength(PdfSectionLocalizationConstants.DescriptionMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdatePdfSectionLocalizationDto.Description), PdfSectionLocalizationConstants.DescriptionMinLength))
            .MaximumLength(PdfSectionLocalizationConstants.DescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdatePdfSectionLocalizationDto.Description), PdfSectionLocalizationConstants.DescriptionMaxLength));
    }
}
