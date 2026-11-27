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

        if (existingMainPage.ImpactStatistics is null)
        {
            // A new statistics block cannot reference existing metrics; its composition is checked by UpdateImpactStatisticDtoValidator.
            return statDto.Metrics.All(m => m.Id is null);
        }

        return MetricsMatchExistingStatistics(statDto.Metrics, existingMainPage.ImpactStatistics.Metrics);
    }

    private static bool MetricsMatchExistingStatistics(
        ICollection<UpdateMetricDto> metrics,
        ICollection<Metric> existingMetrics)
    {
        // Requiring every existing Id with its stored kind prevents replacing a metric to change its kind.
        if (metrics.Any(m => m.Id is null))
        {
            return false;
        }

        var existingKindsById = existingMetrics.ToDictionary(m => m.Id, m => m.Kind);

        return existingKindsById.Keys.ToHashSet().SetEquals(metrics.Select(m => m.Id!.Value))
            && metrics.All(m => existingKindsById[m.Id!.Value] == m.Kind);
    }
}
