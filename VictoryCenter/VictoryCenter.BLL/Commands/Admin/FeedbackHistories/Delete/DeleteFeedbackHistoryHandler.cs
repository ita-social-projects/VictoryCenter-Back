using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.FeedbackHistories.Delete;

public class DeleteFeedbackHistoryHandler : IRequestHandler<DeleteFeedbackHistoryCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteFeedbackHistoryHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(DeleteFeedbackHistoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entityToDelete = await _repositoryWrapper.FeedbackHistoriesRepository.GetFirstOrDefaultAsync(new QueryOptions<FeedbackHistory>
            {
                Filter = entity => entity.Id == request.Id,
                AsNoTracking = false
            });

            if (entityToDelete is null)
            {
                return Result.Fail<long>(ErrorMessagesConstants.NotFound(request.Id, typeof(FeedbackHistory)));
            }

            _repositoryWrapper.FeedbackHistoriesRepository.Delete(entityToDelete);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntity(typeof(FeedbackHistory)));
            }

            return Result.Ok(entityToDelete.Id);
        }
        catch (DbUpdateException)
        {
            return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntity(typeof(FeedbackHistory)));
        }
    }
}
