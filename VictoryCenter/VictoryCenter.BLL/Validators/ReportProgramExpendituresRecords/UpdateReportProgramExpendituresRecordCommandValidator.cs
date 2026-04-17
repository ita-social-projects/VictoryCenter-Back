using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;

public class UpdateReportProgramExpendituresRecordCommandValidator
    : AbstractValidator<UpdateReportProgramExpendituresRecordCommand>
{
    public UpdateReportProgramExpendituresRecordCommandValidator(IRepositoryWrapper repositoryWrapper)
    {
        RuleFor(x => x.ReportProgramExpendituresRecordId)
            .MustBeValidId(
                nameof(UpdateReportProgramExpendituresRecordCommand.ReportProgramExpendituresRecordId));

        RuleFor(x => x.Dto)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.HippotherapyProgramCategoryId)
                    .Cascade(CascadeMode.Stop)
                    .MustBeValidId(
                        nameof(UpdateReportProgramExpendituresRecordDto.HippotherapyProgramCategoryId))
                    .MustAsync(async (categoryId, cancellationToken) =>
                    {
                        var category = await repositoryWrapper
                            .HippotherapyProgramCategoriesRepository
                            .GetFirstOrDefaultAsync(new QueryOptions<HippotherapyProgramCategory>
                            {
                                Filter = e => e.Id == categoryId,
                                AsNoTracking = true
                            });
                        return category is not null;
                    })
                    .WithMessage(ErrorMessagesConstants.NotFound());
                RuleFor(x => x.Dto.AmountUah)
                    .MustBeValidAmountOfMoney(nameof(UpdateReportProgramExpendituresRecordDto.AmountUah));
                RuleFor(x => x.Dto.AmountUsd)
                    .MustBeValidAmountOfMoney(nameof(UpdateReportProgramExpendituresRecordDto.AmountUsd));
            });
    }
}
