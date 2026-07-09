using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.Notifications.ReportFunds;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Microsoft.EntityFrameworkCore;

namespace VictoryCenter.BLL.Commands.Admin.ImpactStatistics.UpdateSingleMetric;

public class UpdateSingleMetricHandler : IRequestHandler<UpdateSingleMetricCommand, Result<UpdateMetricResult>>
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

    public async Task<Result<UpdateMetricResult>> Handle(UpdateSingleMetricCommand request, CancellationToken cancellationToken)
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
                return Result.Fail<UpdateMetricResult>(ErrorMessagesConstants.NotFound(request.MetricId, typeof(Metric)));
            }

            if (request.Dto.ExpectedVersion != null &&
                (metric.RowVersion == null || !metric.RowVersion.SequenceEqual(request.Dto.ExpectedVersion)))
            {
                return Result.Fail<UpdateMetricResult>("Metric was modified by another user. Please refresh and try again.");
            }

            var result = new UpdateMetricResult();
            bool propertiesChanged = false;

            if (request.Dto.Value.HasValue && metric.Value != request.Dto.Value.Value)
            {
                metric.Value = request.Dto.Value.Value;
                result.UpdatedFields.Add(nameof(request.Dto.Value));
                propertiesChanged = true;
            }

            if (request.Dto.Name is not null && metric.Name != request.Dto.Name)
            {
                metric.Name = request.Dto.Name;
                result.UpdatedFields.Add(nameof(request.Dto.Name));
                propertiesChanged = true;
            }

            if (propertiesChanged)
            {
                SetLocalizationsToOutdated(
                    metric.Localizations.Where(l => l.LanguageId != LocalizationLanguageConstants.PrimaryLanguageId));

                var primaryLoc = metric.Localizations.FirstOrDefault(l => l.LanguageId == LocalizationLanguageConstants.PrimaryLanguageId);
                if (primaryLoc != null)
                {
                    primaryLoc.TranslationStatus = TranslationStatus.Relevant;
                }

                result.WasModified = true;
            }

            if (request.Dto.Type.HasValue && metric.Type != request.Dto.Type.Value)
            {
                metric.Type = request.Dto.Type.Value;
                result.UpdatedFields.Add(nameof(request.Dto.Type));
                result.WasModified = true;
            }

            if (request.Dto.Prefix.HasValue && metric.Prefix != request.Dto.Prefix.Value)
            {
                metric.Prefix = request.Dto.Prefix.Value;
                result.UpdatedFields.Add(nameof(request.Dto.Prefix));
                result.WasModified = true;
            }

            if (request.Dto.IsAutoSynced.HasValue && metric.IsAutoSynced != request.Dto.IsAutoSynced.Value)
            {
                metric.IsAutoSynced = request.Dto.IsAutoSynced.Value;
                result.UpdatedFields.Add(nameof(request.Dto.IsAutoSynced));
                result.WasModified = true;
            }

            if (request.Dto.Localization is not null)
            {
                var existingLoc = metric.Localizations.FirstOrDefault(l => l.LanguageId == request.Dto.Localization.LanguageId);

                if (existingLoc is not null)
                {
                    bool locChanged = false;

                    if (request.Dto.Localization.Name != null && existingLoc.Name != request.Dto.Localization.Name)
                    {
                        existingLoc.Name = request.Dto.Localization.Name;
                        locChanged = true;
                    }

                    if (request.Dto.Localization.Value != null && existingLoc.Value != request.Dto.Localization.Value)
                    {
                        existingLoc.Value = request.Dto.Localization.Value;
                        locChanged = true;
                    }

                    if (locChanged)
                    {
                        existingLoc.TranslationStatus = TranslationStatus.Relevant;
                        result.UpdatedFields.Add(nameof(request.Dto.Localization));
                        result.WasModified = true;
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
                    result.UpdatedFields.Add(nameof(request.Dto.Localization));
                    result.WasModified = true;
                }
            }

            if (!result.WasModified)
            {
                return Result.Ok(result);
            }

            await _repositoryWrapper.SaveChangesAsync();
            transaction.Complete();

            if (metric.Type == MetricType.Raised && metric.IsAutoSynced)
            {
                await _mediator.Publish(new ReportFundsChangedNotification(), CancellationToken.None);
            }

            return Result.Ok(result);
        }
        catch (ValidationException vex)
        {
            return Result.Fail<UpdateMetricResult>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (Exception ex)
        {
            return Result.Fail<UpdateMetricResult>($"Failed to update metric: {ex.Message}");
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
