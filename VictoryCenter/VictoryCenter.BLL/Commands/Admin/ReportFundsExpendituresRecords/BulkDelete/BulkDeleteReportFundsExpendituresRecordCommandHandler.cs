using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.BulkDelete;

public class BulkDeleteReportFundsExpendituresRecordCommandHandler
    : IRequestHandler<BulkDeleteReportFundsExpendituresRecordCommand, Result<long[]>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<BulkDeleteReportFundsExpendituresRecordCommand> _validator;

    public BulkDeleteReportFundsExpendituresRecordCommandHandler(
        IValidator<BulkDeleteReportFundsExpendituresRecordCommand> validator, IRepositoryWrapper repositoryWrapper)
    {
        _validator = validator;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long[]>> Handle(
        BulkDeleteReportFundsExpendituresRecordCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var entitiesToDelete = (await _repositoryWrapper
                .ReportFundsExpendituresRecordsRepository
                .GetAllAsync(new QueryOptions<ReportFundsExpendituresRecord>
                {
                    Filter = record => request.Ids.Contains(record.Id)
                })).ToList();

            var existingRecordIds = entitiesToDelete.Select(e => e.Id);

            var nonExistingRecordIds = request.Ids.Except(existingRecordIds).ToList();

            if (nonExistingRecordIds.Any())
            {
                return Result.Fail(ErrorMessagesConstants.NotFound(
                    nonExistingRecordIds,
                    typeof(ReportFundsExpendituresRecord)));
            }

            _repositoryWrapper
                .ReportFundsExpendituresRecordsRepository
                .DeleteRange(entitiesToDelete);

            if (await _repositoryWrapper.SaveChangesAsync() == 0)
            {
                return Result.Fail(
                    ErrorMessagesConstants.FailedToDeleteEntities(typeof(ReportFundsExpendituresRecord)));
            }

            return Result.Ok(request.Ids.ToArray());
        }
        catch (ValidationException validationException)
        {
            return Result.Fail(
                validationException.Errors.Select(error => error.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail(
                ErrorMessagesConstants.FailedToDeleteEntitiesInDatabase(
                    typeof(ReportFundsExpendituresRecord)));
        }
    }
}
