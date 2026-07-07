using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.PdfReports.Reorder;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;

namespace VictoryCenter.BLL.Validators.PdfReports;

public class ReorderPdfReportsCommandValidator : AbstractValidator<ReorderPdfReportsCommand>
{
    public ReorderPdfReportsCommandValidator()
    {
        RuleFor(x => x.ReorderPdfReportsDto.LanguageId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(ReorderPdfReportsDto.LanguageId)));

        RuleFor(x => x.ReorderPdfReportsDto.OrderedIds)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(
                nameof(ReorderPdfReportsDto.OrderedIds)))
            .Must(ids => ids.Count <= ReorderConstants.MaxElementsSwapCount)
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(ReorderPdfReportsDto.OrderedIds),
                ReorderConstants.MaxElementsSwapCount))
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(
                nameof(ReorderPdfReportsDto.OrderedIds)));

        RuleForEach(x => x.ReorderPdfReportsDto.OrderedIds)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(
                $"Each {nameof(ReorderPdfReportsDto.OrderedIds)} element"));
    }
}
