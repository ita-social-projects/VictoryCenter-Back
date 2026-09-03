using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.FeedbackReviews.Delete;

public class DeleteFeedbackReviewHandler : IRequestHandler<DeleteFeedbackReviewCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IReorderService _reorderService;

    public DeleteFeedbackReviewHandler(
        IRepositoryWrapper repositoryWrapper,
        IReorderService reorderService)
    {
        _repositoryWrapper = repositoryWrapper;
        _reorderService = reorderService;
    }

    public async Task<Result<long>> Handle(DeleteFeedbackReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var reviewToDelete = await _repositoryWrapper.FeedbackReviewsRepository.GetFirstOrDefaultAsync(
                new QueryOptions<FeedbackReview>
                {
                    Filter = entity => entity.Id == request.Id,
                });

            if (reviewToDelete is null)
            {
                return Result.Fail<long>(ErrorMessagesConstants.NotFound(request.Id, typeof(FeedbackReview)));
            }

            using var transactionScope = _repositoryWrapper.BeginTransaction();

            _repositoryWrapper.FeedbackReviewsRepository.Delete(reviewToDelete);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<long>(
                    ErrorMessagesConstants.FailedToDeleteEntity(typeof(FeedbackReview)));
            }

            await _reorderService.RenumberPriorityAsync<FeedbackReview>();

            transactionScope.Complete();

            return Result.Ok(reviewToDelete.Id);
        }
        catch (DbUpdateException)
        {
            return Result.Fail<long>(
                ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(FeedbackReview)));
        }
    }
}
