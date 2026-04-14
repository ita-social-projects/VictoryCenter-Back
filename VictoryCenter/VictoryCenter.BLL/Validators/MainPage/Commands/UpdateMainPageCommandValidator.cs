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
                .MustAsync(RequireIdsWhenExistingImpactStatisticsPresentAsync)
                .WithMessage(ErrorMessagesConstants.PropertyIsRequired("ImpactStatistics[].Id and ImpactStatistics[].Metrics[].Id"));
        });
    }

    private async Task<bool> RequireIdsWhenExistingImpactStatisticsPresentAsync(
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

        if (existingMainPage is null || existingMainPage.ImpactStatistics.Count == 0)
        {
            return true;
        }

        var stats = command.UpdateMainPageDto.ImpactStatistics ?? [];

        if (stats.Any(s => !s.Id.HasValue))
        {
            return false;
        }

        if (stats.Any(s => s.Metrics.Any(m => !m.Id.HasValue)))
        {
            return false;
        }

        return true;
    }
}