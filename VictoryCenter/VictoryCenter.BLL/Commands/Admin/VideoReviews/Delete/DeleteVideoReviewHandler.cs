using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.VideoReviews.Delete;

public class DeleteVideoReviewHandler : IRequestHandler<DeleteVideoReviewCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly TimeProvider _timeProvider;
    private readonly IReorderService _reorderService;

    public DeleteVideoReviewHandler(
        IRepositoryWrapper repositoryWrapper,
        TimeProvider timeProvider,
        IReorderService reorderService)
    {
        _repositoryWrapper = repositoryWrapper;
        _timeProvider = timeProvider;
        _reorderService = reorderService;
    }

    public async Task<Result<long>> Handle(DeleteVideoReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await using var transaction = await _repositoryWrapper.BeginTransactionAsync();

            var archivedRows = await _repositoryWrapper.VideoReviewsRepository.ArchiveAsync(
                request.Id,
                _timeProvider.GetUtcNow());

            if (archivedRows == 0)
            {
                var entity = await _repositoryWrapper.VideoReviewsRepository.GetFirstOrDefaultAsync(
                    new QueryOptions<VideoReview>
                    {
                        Filter = videoReview => videoReview.Id == request.Id,
                        AsNoTracking = true
                    });

                if (entity is null)
                {
                    return Result.Fail<long>(ErrorMessagesConstants.NotFound(request.Id, typeof(VideoReview)));
                }

                await transaction.CommitAsync(cancellationToken);
                return Result.Ok(entity.Id);
            }

            await _reorderService.RenumberPriorityAsync<VideoReview>(videoReview => !videoReview.IsArchived);

            await transaction.CommitAsync(cancellationToken);
            return Result.Ok(request.Id);
        }
        catch (DbUpdateException)
        {
            return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(VideoReview)));
        }
    }
}
