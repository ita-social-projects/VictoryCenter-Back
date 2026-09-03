using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.FeedbackHistories.Delete;

public class DeleteFeedbackHistoryHandler : IRequestHandler<DeleteFeedbackHistoryCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IReorderService _reorderService;

    public DeleteFeedbackHistoryHandler(IRepositoryWrapper repositoryWrapper, IReorderService reorderService)
    {
        _repositoryWrapper = repositoryWrapper;
        _reorderService = reorderService;
    }

    public async Task<Result<long>> Handle(DeleteFeedbackHistoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entityToDelete = await _repositoryWrapper.FeedbackHistoriesRepository.GetFirstOrDefaultAsync(
                new QueryOptions<FeedbackHistory>
                {
                    Filter = entity => entity.Id == request.Id,
                    AsNoTracking = false
                });

            if (entityToDelete is null)
            {
                return Result.Fail<long>(ErrorMessagesConstants.NotFound(request.Id, typeof(FeedbackHistory)));
            }

            using var scope = _repositoryWrapper.BeginTransaction();

            _repositoryWrapper.FeedbackHistoriesRepository.Delete(entityToDelete);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntity(typeof(FeedbackHistory)));
            }

            await _reorderService.RenumberPriorityAsync<FeedbackHistory>();
            scope.Complete();

            return Result.Ok(request.Id);
        }
        catch (DbUpdateException)
        {
            return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntity(typeof(FeedbackHistory)));
        }
    }
}
