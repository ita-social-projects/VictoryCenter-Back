using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.VideoReviews.Restore;

public class RestoreVideoReviewHandler : IRequestHandler<RestoreVideoReviewCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public RestoreVideoReviewHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(RestoreVideoReviewCommand request, CancellationToken cancellationToken)
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

        if (!entity.IsArchived)
        {
            return Result.Ok(entity.Id);
        }

        entity.IsArchived = false;
        entity.ArchivedAt = null;

        try
        {
            return await _repositoryWrapper.SaveChangesAsync() > 0
                ? Result.Ok(entity.Id)
                : Result.Fail<long>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(VideoReview)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<long>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(VideoReview)));
        }
    }
}
