using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.BulkDelete;

public class BulkDeleteReportProgramExpendituresRecordCommandHandler
    : IRequestHandler<BulkDeleteReportProgramExpendituresRecordCommand, Result<long[]>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<BulkDeleteReportProgramExpendituresRecordCommand> _validator;

    public BulkDeleteReportProgramExpendituresRecordCommandHandler(
        IValidator<BulkDeleteReportProgramExpendituresRecordCommand> validator, IRepositoryWrapper repositoryWrapper)
    {
        _validator = validator;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long[]>> Handle(
        BulkDeleteReportProgramExpendituresRecordCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            using var transaction = _repositoryWrapper.BeginTransaction();

            var existingRecordIds = await _repositoryWrapper
                .ReportProgramExpendituresRecordsRepository
                .GetAllProjectedAsync(
                    record => record.Id,
                    new QueryOptions<ReportProgramExpendituresRecord>
                    {
                        Filter = record => request.Ids.Contains(record.Id),
                        AsNoTracking = true
                    });

            var nonExistingRecordIds = request.Ids.Except(existingRecordIds).ToList();

            if (nonExistingRecordIds.Count > 0)
            {
                return Result.Fail(ErrorMessagesConstants.NotFound(
                    nonExistingRecordIds,
                    typeof(ReportProgramExpendituresRecord)));
            }

            var deletedEntityCount = await _repositoryWrapper
                .ReportProgramExpendituresRecordsRepository
                .BulkDeleteAsync(record => request.Ids.Contains(record.Id));

            if (deletedEntityCount != request.Ids.Count())
            {
                return Result.Fail(
                    ErrorMessagesConstants.FailedToDeleteEntities(typeof(ReportProgramExpendituresRecord)));
            }

            transaction.Complete();

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
                    typeof(ReportProgramExpendituresRecord)));
        }
    }
}
