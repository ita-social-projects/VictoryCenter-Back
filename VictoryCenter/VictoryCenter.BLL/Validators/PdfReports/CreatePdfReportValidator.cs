using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.PdfReports.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;

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
            RuleFor(x => x.CreatePdfReportDto.File)
                .NotNull()
                .WithMessage(ErrorMessagesConstants.PropertyIsRequired("File"))
                .Must(file => file?.Length > 0)
                .WithMessage(PdfReportConstants.FileCannotBeEmpty)
                .Must(file => file?.Length <= PdfReportConstants.MaxPdfSizeInBytes)
                .WithMessage(string.Format(PdfReportConstants.FileTooLarge, PdfReportConstants.MaxPdfSizeInMb))
                .Must(file => file?.ContentType.Equals(PdfReportConstants.PdfMimeType, StringComparison.OrdinalIgnoreCase) == true)
                .WithMessage(PdfReportConstants.FileMustBePdf);

            RuleFor(x => x.CreatePdfReportDto.LanguageId)
                .GreaterThan(0)
                .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreatePdfReportDto.LanguageId)));
        });
    }
}
