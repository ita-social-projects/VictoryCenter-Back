using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.PdfSection.Update;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.PdfSection;

public class UpdatePdfSectionValidator : AbstractValidator<UpdatePdfSectionCommand>
{
    public UpdatePdfSectionValidator()
    {
        RuleFor(x => x.Dto)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdatePdfSectionCommand.Dto)));

        When(x => x.Dto != null, () =>
        {
            RuleFor(x => x.Dto.Title)
                .NotEmpty()
                .WithMessage(PdfSectionConstants.TitleRequiredErrorMessage)
                .MinimumLength(PdfSectionConstants.TitleMinLength)
                .WithMessage(PdfSectionConstants.TitleMinLengthErrorMessage)
                .MaximumLength(PdfSectionConstants.TitleMaxLength)
                .WithMessage(PdfSectionConstants.TitleMaxLengthErrorMessage);

            RuleFor(x => x.Dto.Description)
                .NotEmpty()
                .WithMessage(PdfSectionConstants.DescriptionRequiredErrorMessage)
                .MinimumLength(PdfSectionConstants.DescriptionMinLength)
                .WithMessage(PdfSectionConstants.DescriptionMinLengthErrorMessage)
                .MaximumLength(PdfSectionConstants.DescriptionMaxLength)
                .WithMessage(PdfSectionConstants.DescriptionMaxLengthErrorMessage);
        });
    }
}
