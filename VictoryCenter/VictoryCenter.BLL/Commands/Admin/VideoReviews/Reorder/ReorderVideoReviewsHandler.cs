using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.VideoReviews.Reorder;

public class ReorderVideoReviewsHandler : IRequestHandler<ReorderVideoReviewsCommand, Result<Unit>>
{
    private readonly IValidator<ReorderVideoReviewsCommand> _validator;
    private readonly IReorderService _reorderService;

    public ReorderVideoReviewsHandler(
        IValidator<ReorderVideoReviewsCommand> validator,
        IReorderService reorderService)
    {
        _validator = validator;
        _reorderService = reorderService;
    }

    public async Task<Result<Unit>> Handle(ReorderVideoReviewsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var orderedIds = request.ReorderVideoReviewsDto.OrderedIds;

            await _reorderService.SwapElementsAsync<VideoReview>(
                orderedIds,
                vr => vr.Id);

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
            return Result.Fail<Unit>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(VideoReview)));
        }
    }
}
