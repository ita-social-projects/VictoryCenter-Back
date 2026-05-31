using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.PdfReports.Update;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.PdfReports;

public class UpdatePdfReportValidator : AbstractValidator<UpdatePdfReportCommand>
{
    public UpdatePdfReportValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeGreaterThan(nameof(UpdatePdfReportCommand.Id), 0));

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(PdfReportConstants.NameRequiredErrorMessage)
            .MinimumLength(PdfReportConstants.NameMinLength)
            .WithMessage(PdfReportConstants.NameMinLengthErrorMessage)
            .MaximumLength(PdfReportConstants.NameMaxLength)
            .WithMessage(PdfReportConstants.NameMaxLengthErrorMessage);
    }
}
