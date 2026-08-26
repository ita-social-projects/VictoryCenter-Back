using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.VideoReviews.Delete;

public class DeleteVideoReviewHandler : IRequestHandler<DeleteVideoReviewCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteVideoReviewHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
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

        _repositoryWrapper.VideoReviewsRepository.Delete(entity);

        try
        {
            return await _repositoryWrapper.SaveChangesAsync() > 0
                ? Result.Ok(entity.Id)
                : Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntity(typeof(VideoReview)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(VideoReview)));
        }
    }
}
