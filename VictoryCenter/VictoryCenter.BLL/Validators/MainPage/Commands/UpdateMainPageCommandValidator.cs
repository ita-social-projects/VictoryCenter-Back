using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Admin.MainPage.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.Validators.MainPage.Dto;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.BLL.Validators.MainPage.Commands;

public class UpdateMainPageCommandValidator : AbstractValidator<UpdateMainPageCommand>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public UpdateMainPageCommandValidator(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;

        RuleFor(x => x.UpdateMainPageDto)
            .NotNull()
            .SetValidator(new UpdateMainPageDtoValidator());

        When(x => x.UpdateMainPageDto is not null, () =>
        {
            RuleFor(x => x)
                .MustAsync(ValidateNestedIdsByMembershipAsync)
                .WithMessage(ErrorMessagesConstants.MetricsMustMatchExistingStatistics);
        });
    }

    private async Task<bool> ValidateNestedIdsByMembershipAsync(
        UpdateMainPageCommand command,
        CancellationToken cancellationToken)
    {
        var existingMainPage = await _repositoryWrapper.MainPageRepository.GetFirstOrDefaultAsync(
            new QueryOptions<MainPageEntity>
            {
                AsNoTracking = true,
                Include = q => q
                    .Include(e => e.ImpactStatistics)
                        .ThenInclude(s => s!.Metrics),
            });

        if (existingMainPage is null)
        {
            return true;
        }

        var statDto = command.UpdateMainPageDto.ImpactStatistics;

        if (statDto?.Id.HasValue == true && existingMainPage.ImpactStatistics?.Id != statDto.Id.Value)
        {
            return false;
        }

        if (statDto?.Metrics is null || statDto.Metrics.Any(m => m is null))
        {
            return true;
        }

        if (existingMainPage.ImpactStatistics is null &&
            statDto.Metrics.Count != MainPageConstants.ImpactStatistic.ExactMetricCount)
        {
            return false;
        }

        return MetricsMatchExistingStatistics(statDto.Metrics, existingMainPage.ImpactStatistics?.Metrics ?? []);
    }

    private static bool MetricsMatchExistingStatistics(
        ICollection<UpdateMetricDto> metrics,
        ICollection<Metric> existingMetrics)
    {
        var requestedIds = metrics
            .Where(m => m.Id.HasValue)
            .Select(m => m.Id!.Value)
            .ToHashSet();

        if (!requestedIds.SetEquals(existingMetrics.Select(m => m.Id)))
        {
            return false;
        }

        return metrics.All(metric => metric.Id.HasValue
            ? existingMetrics.Any(existing => existing.Id == metric.Id.Value && existing.Type == metric.Type)
            : existingMetrics.All(existing => existing.Type != metric.Type));
    }
}
