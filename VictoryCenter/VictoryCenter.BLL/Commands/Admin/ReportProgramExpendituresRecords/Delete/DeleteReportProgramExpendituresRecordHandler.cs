using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Notifications.ReportFunds;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.Delete;

public class DeleteReportProgramExpendituresRecordHandler
    : IRequestHandler<DeleteReportProgramExpendituresRecordCommand, Result<long>>
{
    private readonly IMediator _mediator;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<DeleteReportProgramExpendituresRecordCommand> _validator;

    public DeleteReportProgramExpendituresRecordHandler(
        IRepositoryWrapper repositoryWrapper,
        IValidator<DeleteReportProgramExpendituresRecordCommand> validator,
        IMediator mediator)
    {
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _mediator = mediator;
    }

    public async Task<Result<long>> Handle(
        DeleteReportProgramExpendituresRecordCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var recordToBeDeleted = await _repositoryWrapper
                .ReportProgramExpendituresRecordsRepository.GetFirstOrDefaultAsync(
                    new QueryOptions<ReportProgramExpendituresRecord>
                    {
                        Filter = record => record.Id == request.ReportProgramExpendituresRecordId
                    });

            if (recordToBeDeleted is null)
            {
                return Result.Fail(
                    ErrorMessagesConstants.NotFound(
                        request.ReportProgramExpendituresRecordId,
                        typeof(ReportProgramExpendituresRecord)));
            }

            _repositoryWrapper.ReportProgramExpendituresRecordsRepository.Delete(recordToBeDeleted);

            if (await _repositoryWrapper.SaveChangesAsync() == 0)
            {
                return Result.Fail(
                    ErrorMessagesConstants.FailedToDeleteEntity(typeof(ReportProgramExpendituresRecord)));
            }

            await _mediator.Publish(new ReportFundsChangedNotification(), CancellationToken.None);

            return Result.Ok(request.ReportProgramExpendituresRecordId);
        }
        catch (ValidationException validationException)
        {
            return Result.Fail(
                validationException.Errors.Select(error => error.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail(
                ErrorMessagesConstants.FailedToDeleteEntityInDatabase(
                    typeof(ReportProgramExpendituresRecord)));
        }
    }
}
