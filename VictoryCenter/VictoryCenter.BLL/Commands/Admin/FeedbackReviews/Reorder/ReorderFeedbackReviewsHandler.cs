using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.FeedbackReviews.Reorder;

public class ReorderFeedbackReviewsHandler : IRequestHandler<ReorderFeedbackReviewsCommand, Result<Unit>>
{
    private readonly IValidator<ReorderFeedbackReviewsCommand> _validator;
    private readonly IReorderService _reorderService;

    public ReorderFeedbackReviewsHandler(
        IValidator<ReorderFeedbackReviewsCommand> validator,
        IReorderService reorderService)
    {
        _validator = validator;
        _reorderService = reorderService;
    }

    public async Task<Result<Unit>> Handle(ReorderFeedbackReviewsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var orderedIds = request.ReorderFeedbackReviewsDto.OrderedIds;

            await _reorderService.SwapElementsAsync<FeedbackReview>(
                orderedIds,
                fr => fr.Id);

            return Result.Ok(Unit.Value);
        }
        catch (ValidationException ex)
        {
            return Result.Fail(ex.Message);
        }
        catch (ReorderException ex)
        {
            return Result.Fail(ReorderConstants.ErrorWithReordering(ex.Message));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<Unit>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(FeedbackReview)));
        }
    }
}
