using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.FeedbackHistories.Reorder;

public class ReorderFeedbackHistoriesHandler : IRequestHandler<ReorderFeedbackHistoriesCommand, Result<Unit>>
{
    private readonly IValidator<ReorderFeedbackHistoriesCommand> _validator;
    private readonly IReorderService _reorderService;

    public ReorderFeedbackHistoriesHandler(
        IValidator<ReorderFeedbackHistoriesCommand> validator,
        IReorderService reorderService)
    {
        _validator = validator;
        _reorderService = reorderService;
    }

    public async Task<Result<Unit>> Handle(ReorderFeedbackHistoriesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var orderedIds = request.ReorderFeedbackHistoriesDto.OrderedIds;

            await _reorderService.SwapElementsAsync<FeedbackHistory>(
                orderedIds,
                fh => fh.Id);

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
            return Result.Fail<Unit>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(FeedbackHistory)));
        }
    }
}
