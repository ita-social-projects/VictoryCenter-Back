using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Notifications.ReportFunds;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.Delete;

public class DeleteReportFundsExpendituresRecordHandler
    : IRequestHandler<DeleteReportFundsExpendituresRecordCommand, Result<long>>
{
    private readonly IMediator _mediator;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteReportFundsExpendituresRecordHandler(
        IMediator mediator,
        IRepositoryWrapper repositoryWrapper)
    {
        _mediator = mediator;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(
        DeleteReportFundsExpendituresRecordCommand request, CancellationToken cancellationToken)
    {
        var entityToDelete = await _repositoryWrapper.ReportFundsExpendituresRecordsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<ReportFundsExpendituresRecord>
            {
                Filter = entity => entity.Id == request.Id
            });

        if (entityToDelete is null)
        {
            return Result.Fail<long>(
                ErrorMessagesConstants.NotFound(request.Id, typeof(ReportFundsExpendituresRecord)));
        }

        _repositoryWrapper.ReportFundsExpendituresRecordsRepository.Delete(entityToDelete);

        try
        {
            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                await _mediator.Publish(new ReportFundsChangedNotification(), cancellationToken);

                return Result.Ok(entityToDelete.Id);
            }
        }
        catch (DbUpdateException)
        {
            return Result.Fail<long>(
                ErrorMessagesConstants.FailedToDeleteEntity(typeof(ReportFundsExpendituresRecord)));
        }

        return Result.Fail<long>(
            ErrorMessagesConstants.FailedToDeleteEntity(typeof(ReportFundsExpendituresRecord)));
    }
}
