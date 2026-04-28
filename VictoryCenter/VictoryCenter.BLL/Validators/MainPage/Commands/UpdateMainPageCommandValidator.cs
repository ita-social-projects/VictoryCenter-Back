using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Admin.MainPage.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Validators.MainPage.Dto;
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
                .WithMessage(ErrorMessagesConstants.PropertyIsRequired("ImpactStatistics[].Id and ImpactStatistics[].Metrics[].Id"));
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
                        .ThenInclude(s => s.Metrics),
            });

        if (existingMainPage is null)
        {
            return true;
        }

        var stats = command.UpdateMainPageDto.ImpactStatistics ?? [];

        var existingStatIds = existingMainPage.ImpactStatistics.Select(s => s.Id).ToHashSet();

        foreach (var stat in stats)
        {
            if (!stat.Id.HasValue)
            {
                continue;
            }

            if (!existingStatIds.Contains(stat.Id.Value))
            {
                return false;
            }
        }

        var existingMetricIds = existingMainPage.ImpactStatistics
            .SelectMany(s => s.Metrics)
            .Select(m => m.Id)
            .ToHashSet();

        foreach (var metricId in stats
                     .SelectMany(s => s.Metrics ?? [])
                     .Where(m => m.Id.HasValue)
                     .Select(m => m.Id!.Value))
        {
            if (!existingMetricIds.Contains(metricId))
            {
                return false;
            }
        }

        return true;
    }
}