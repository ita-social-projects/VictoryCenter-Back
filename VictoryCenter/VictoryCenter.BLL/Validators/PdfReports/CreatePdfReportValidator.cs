using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.PdfReports.Create;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.PdfReports;

public class CreatePdfReportValidator : AbstractValidator<CreatePdfReportCommand>
{
    public CreatePdfReportValidator()
    {
        RuleFor(x => x.CreatePdfReportDto)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreatePdfReportCommand.CreatePdfReportDto)));

        When(x => x.CreatePdfReportDto != null, () =>
        {
            RuleFor(x => x.CreatePdfReportDto.LanguageId)
                .GreaterThan(0)
                .WithMessage("LanguageId must be greater than 0.");

            RuleFor(x => x.CreatePdfReportDto.File)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("File"))
            .Must(file => file?.Length > 0)
            .WithMessage(PdfReportConstants.FileCannotBeEmpty)
            .Must(file => file?.Length <= PdfReportConstants.MaxPdfSizeInBytes)
            .WithMessage(string.Format(PdfReportConstants.FileTooLarge, PdfReportConstants.MaxPdfSizeInMb))
            .Must(file => file?.ContentType.Equals(PdfReportConstants.PdfMimeType, StringComparison.OrdinalIgnoreCase) == true)
            .WithMessage(PdfReportConstants.FileMustBePdf);
        });
    }
}
