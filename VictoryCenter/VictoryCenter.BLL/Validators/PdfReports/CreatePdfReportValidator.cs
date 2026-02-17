using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.PdfReports.Create;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.PdfReports;

public class CreatePdfReportValidator : AbstractValidator<CreatePdfReportCommand>
{
    private const long MaxPdfSizeInBytes = 10 * 1024 * 1024;
    private const string PdfMimeType = "application/pdf";

    public CreatePdfReportValidator()
    {
        RuleFor(x => x.CreatePdfReportDto)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreatePdfReportCommand.CreatePdfReportDto)));

        RuleFor(x => x.CreatePdfReportDto.File)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("File"))
            .Must(file => file?.Length > 0)
            .WithMessage("File cannot be empty")
            .Must(file => file?.Length <= MaxPdfSizeInBytes)
            .WithMessage($"File size cannot exceed {MaxPdfSizeInBytes / 1024 / 1024} MB")
            .Must(file => file?.ContentType.Equals(PdfMimeType, StringComparison.OrdinalIgnoreCase) == true)
            .WithMessage("File must be a PDF");
    }
}
