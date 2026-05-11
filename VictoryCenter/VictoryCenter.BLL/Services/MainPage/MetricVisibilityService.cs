using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.MainPage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Services.MainPage;

public class MetricVisibilityService : IMetricVisibilityService
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public MetricVisibilityService(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task ToggleMetricVisibilityAsync(long id, bool isHidden)
    {
        using var transaction = _repositoryWrapper.BeginTransaction();

        var metricRepository = _repositoryWrapper.GetRepository<Metric>();

        var metric = await metricRepository.GetFirstOrDefaultAsync(new QueryOptions<Metric>
        {
            Filter = m => m.Id == id
        });

        if (metric is null)
        {
            throw new KeyNotFoundException(ErrorMessagesConstants.NotFound(id, typeof(Metric)));
        }

        if (metric.IsHidden == isHidden)
        {
            return;
        }

        if (isHidden)
        {
            var visibleCount = await metricRepository.CountAsync(new QueryOptions<Metric>
            {
                Filter = m => m.StatisticId == metric.StatisticId && !m.IsHidden
            });

            if (visibleCount <= 1)
            {
                throw new InvalidOperationException("Cannot hide the last visible metric in the block.");
            }
        }

        metric.IsHidden = isHidden;
        metricRepository.Update(metric);

        if (await _repositoryWrapper.SaveChangesAsync() <= 0)
        {
            throw new InvalidOperationException(ErrorMessagesConstants.FailedToUpdateEntity(typeof(Metric)));
        }

        transaction.Complete();
    }
}
