using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Notifications.ReportFunds;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Microsoft.EntityFrameworkCore;

namespace VictoryCenter.BLL.Commands.Admin.ImpactStatistics.UpdateSingleMetric;

public class UpdateSingleMetricHandler : IRequestHandler<UpdateSingleMetricCommand, Result<Unit>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateSingleMetricCommand> _validator;
    private readonly IMediator _mediator;

    public UpdateSingleMetricHandler(
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateSingleMetricCommand> validator,
        IMediator mediator)
    {
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _mediator = mediator;
    }

    public async Task<Result<Unit>> Handle(UpdateSingleMetricCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            using var transaction = _repositoryWrapper.BeginTransaction();

            var options = new QueryOptions<Metric>
            {
                AsNoTracking = false,
                Filter = m => m.Id == request.MetricId,
                Include = q => q.Include(x => x.Localizations)
            };

            var metric = await _repositoryWrapper.MetricRepository.GetFirstOrDefaultAsync(options);

            if (metric is null)
            {
                return Result.Fail<Unit>(ErrorMessagesConstants.NotFound(request.MetricId, typeof(Metric)));
            }

            bool isChanged = false;
            bool propertiesChanged = false;

            if (request.Dto.Value.HasValue && metric.Value != request.Dto.Value.Value)
            {
                metric.Value = request.Dto.Value.Value;
                propertiesChanged = true;
            }

            if (request.Dto.Name is not null && metric.Name != request.Dto.Name)
            {
                metric.Name = request.Dto.Name;
                propertiesChanged = true;
            }

            if (propertiesChanged)
            {
                SetLocalizationsToOutdated(metric.Localizations);
                isChanged = true;
            }

            if (request.Dto.Type.HasValue && metric.Type != request.Dto.Type.Value)
            {
                metric.Type = request.Dto.Type.Value;
                isChanged = true;
            }

            if (request.Dto.Prefix.HasValue && metric.Prefix != request.Dto.Prefix.Value)
            {
                metric.Prefix = request.Dto.Prefix.Value;
                isChanged = true;
            }

            if (request.Dto.IsAutoSynced.HasValue && metric.IsAutoSynced != request.Dto.IsAutoSynced.Value)
            {
                metric.IsAutoSynced = request.Dto.IsAutoSynced.Value;
                isChanged = true;
            }

            if (request.Dto.Localization is not null)
            {
                var existingLoc = metric.Localizations.FirstOrDefault(l => l.LanguageId == request.Dto.Localization.LanguageId);

                if (existingLoc is not null)
                {
                    bool locChanged = existingLoc.Value != request.Dto.Localization.Value ||
                                      existingLoc.Name != request.Dto.Localization.Name;

                    existingLoc.Value = request.Dto.Localization.Value;
                    existingLoc.Name = request.Dto.Localization.Name;

                    if (locChanged)
                    {
                        existingLoc.TranslationStatus = TranslationStatus.Relevant;
                        isChanged = true;
                    }
                }
                else
                {
                    await _repositoryWrapper.MetricLocalizationsRepository.CreateAsync(new MetricLocalization
                    {
                        EntityId = metric.Id,
                        LanguageId = request.Dto.Localization.LanguageId,
                        Value = request.Dto.Localization.Value,
                        Name = request.Dto.Localization.Name,
                        TranslationStatus = TranslationStatus.Relevant
                    });
                    isChanged = true;
                }
            }

            if (isChanged)
            {
                await _repositoryWrapper.SaveChangesAsync();

                if (metric.Type == MetricType.Raised && metric.IsAutoSynced)
                {
                    await _mediator.Publish(new ReportFundsChangedNotification(), CancellationToken.None);
                }
            }

            transaction.Complete();

            return Result.Ok(Unit.Value);
        }
        catch (ValidationException vex)
        {
            return Result.Fail<Unit>(vex.Errors.Select(e => e.ErrorMessage));
        }
    }

    private static void SetLocalizationsToOutdated(IEnumerable<MetricLocalization> localizations)
    {
        foreach (var loc in localizations)
        {
            loc.TranslationStatus = TranslationStatus.Outdated;
        }
    }
}
