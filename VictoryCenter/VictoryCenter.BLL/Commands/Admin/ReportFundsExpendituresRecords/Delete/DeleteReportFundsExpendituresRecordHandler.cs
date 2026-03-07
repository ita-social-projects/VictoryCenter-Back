using FluentResults;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.Delete;

public class DeleteReportFundsExpendituresRecordHandler
    : IRequestHandler<DeleteReportFundsExpendituresRecordCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteReportFundsExpendituresRecordHandler(IRepositoryWrapper repositoryWrapper)
    {
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

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            return Result.Ok(entityToDelete.Id);
        }

        return Result.Fail<long>(
            ErrorMessagesConstants.FailedToDeleteEntity(typeof(ReportFundsExpendituresRecord)));
    }
}
