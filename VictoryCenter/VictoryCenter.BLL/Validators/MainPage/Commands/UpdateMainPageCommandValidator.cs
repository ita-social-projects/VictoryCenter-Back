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
                .WithMessage(ErrorMessagesConstants.PropertyIsRequired("ImpactStatistics.Id and ImpactStatistics.Metrics[].Id"));
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

        if (statDto?.Metrics is null || existingMainPage.ImpactStatistics is null)
        {
            return true;
        }

        // Requiring every existing Id prevents replacing a metric to change its kind.
        if (statDto.Metrics.Any(m => m?.Id is null))
        {
            return false;
        }

        var existingMetricIds = existingMainPage.ImpactStatistics.Metrics
            .Select(m => m.Id)
            .ToHashSet();

        return existingMetricIds.SetEquals(statDto.Metrics.Select(m => m.Id!.Value));
    }
}
