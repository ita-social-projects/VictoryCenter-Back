using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.MainPage;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.ImpactStatistics.ToggleMetricVisibility;

public class ToggleMetricVisibilityHandler : IRequestHandler<ToggleMetricVisibilityCommand, Result<Unit>>
{
    private readonly IMetricVisibilityService _metricVisibilityService;

    public ToggleMetricVisibilityHandler(IMetricVisibilityService metricVisibilityService)
    {
        _metricVisibilityService = metricVisibilityService;
    }

    public async Task<Result<Unit>> Handle(ToggleMetricVisibilityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _metricVisibilityService.ToggleMetricVisibilityAsync(request.MetricId, request.Dto.IsHidden);
            return Result.Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return Result.Fail<Unit>(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Fail<Unit>(ex.Message);
        }
        catch (DbUpdateException)
        {
            return Result.Fail<Unit>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(Metric)));
        }
    }
}
