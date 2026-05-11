using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.ImpactStatistics.ReorderMetrics;

public class ReorderMetricsHandler : IRequestHandler<ReorderMetricsCommand, Result<Unit>>
{
    private readonly IValidator<ReorderMetricsCommand> _validator;
    private readonly IReorderService _reorderService;

    public ReorderMetricsHandler(
        IValidator<ReorderMetricsCommand> validator,
        IReorderService reorderService)
    {
        _validator = validator;
        _reorderService = reorderService;
    }

    public async Task<Result<Unit>> Handle(ReorderMetricsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var orderedIds = request.ReorderDto.OrderedIds;
            var statisticId = request.ReorderDto.StatisticId;

            await _reorderService.SwapElementsAsync<Metric>(
                idsOrder: orderedIds,
                idSelector: e => e.Id,
                groupSelector: e => e.StatisticId == statisticId);

            return Result.Ok();
        }
        catch (ValidationException ex)
        {
            return Result.Fail<Unit>(ex.Message);
        }
        catch (ReorderException ex)
        {
            return Result.Fail(ReorderConstants.ErrorWithReordering(ex.Message));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<Unit>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(Metric)));
        }
    }
}