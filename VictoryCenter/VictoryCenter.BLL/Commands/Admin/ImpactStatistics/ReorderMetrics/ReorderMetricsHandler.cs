using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ImpactStatistics.ReorderMetrics;

public class ReorderMetricsHandler : IRequestHandler<ReorderMetricsCommand, Result<Unit>>
{
    private readonly IValidator<ReorderMetricsCommand> _validator;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public ReorderMetricsHandler(
        IValidator<ReorderMetricsCommand> validator,
        IRepositoryWrapper repositoryWrapper)
    {
        _validator = validator;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<Unit>> Handle(ReorderMetricsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var orderedIds = request.ReorderDto.OrderedIds;
            var statisticId = request.ReorderDto.StatisticId;

            using var transaction = _repositoryWrapper.BeginTransaction();

            var metrics = (await _repositoryWrapper.MetricRepository.GetAllAsync(new QueryOptions<Metric>
            {
                Filter = m => m.StatisticId == statisticId,
                AsNoTracking = false
            }))
            .OrderBy(m => m.Priority)
            .ThenBy(m => m.Id)
            .ToList();

            var visibleMetrics = metrics.Where(m => !m.IsHidden).ToList();
            var hiddenMetrics = metrics.Where(m => m.IsHidden).ToList();

            var visibleIds = visibleMetrics.Select(m => m.Id).ToHashSet();
            var requestedIds = orderedIds.ToHashSet();

            if (orderedIds.Count != visibleMetrics.Count || !requestedIds.SetEquals(visibleIds))
            {
                var matchedCount = orderedIds.Count(id => visibleIds.Contains(id));
                throw new ReorderException(ReorderConstants.NotAllEntitiesFoundForReorder(
                    foundCount: matchedCount,
                    expectedCount: visibleMetrics.Count));
            }

            var visibleMetricsById = visibleMetrics.ToDictionary(m => m.Id);

            long priority = 0;
            foreach (var metricId in orderedIds)
            {
                var metric = visibleMetricsById[metricId];
                if (metric.Priority != priority)
                {
                    metric.Priority = priority;
                    _repositoryWrapper.MetricRepository.Update(metric);
                }

                priority++;
            }

            foreach (var metric in hiddenMetrics)
            {
                if (metric.Priority != priority)
                {
                    metric.Priority = priority;
                    _repositoryWrapper.MetricRepository.Update(metric);
                }

                priority++;
            }

            await _repositoryWrapper.SaveChangesAsync();
            transaction.Complete();

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
