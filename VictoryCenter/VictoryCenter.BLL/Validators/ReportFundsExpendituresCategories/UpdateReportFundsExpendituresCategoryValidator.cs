using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresCategories.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresCategories;

namespace VictoryCenter.BLL.Validators.ReportFundsExpendituresCategories;

public class UpdateReportFundsExpendituresCategoryValidator : AbstractValidator<UpdateReportFundsExpendituresCategoryCommand>
{
    public UpdateReportFundsExpendituresCategoryValidator()
    {
        RuleFor(command => command.UpdateReportFundsExpendituresCategoryDto)
            .NotNull();

        RuleFor(command => command.UpdateReportFundsExpendituresCategoryDto)
            .ChildRules(dto =>
            {
                dto.RuleFor(categoryDto => categoryDto.Name)
                    .NotEmpty()
                    .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ReportFundsExpendituresCategoryDto.Name)))
                    .MaximumLength(ReportFundsExpendituresCategoryConstants.NameMaxLength)
                    .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                        nameof(ReportFundsExpendituresCategoryDto.Name),
                        ReportFundsExpendituresCategoryConstants.NameMaxLength));
            });
    }
}
