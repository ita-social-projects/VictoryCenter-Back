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
    private readonly IReorderService _reorderService;

    public DeleteVideoReviewHandler(IRepositoryWrapper repositoryWrapper, IReorderService reorderService)
    {
        _repositoryWrapper = repositoryWrapper;
        _reorderService = reorderService;
    }

    public async Task<Result<long>> Handle(DeleteVideoReviewCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repositoryWrapper.VideoReviewsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<VideoReview>
            {
                Filter = videoReview => videoReview.Id == request.Id,
                AsNoTracking = false
            });

        if (entity is null)
        {
            return Result.Fail<long>(ErrorMessagesConstants.NotFound(request.Id, typeof(VideoReview)));
        }

        try
        {
            using var transactionScope = _repositoryWrapper.BeginTransaction();

            _repositoryWrapper.VideoReviewsRepository.Delete(entity);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntity(typeof(VideoReview)));
            }

            await _reorderService.RenumberPriorityAsync<VideoReview>();

            transactionScope.Complete();
            return Result.Ok(entity.Id);
        }
        catch (DbUpdateException)
        {
            return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(VideoReview)));
        }
    }
}
