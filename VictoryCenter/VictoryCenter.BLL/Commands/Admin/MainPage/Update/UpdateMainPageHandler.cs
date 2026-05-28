using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using ImpactStatisticsEntity = VictoryCenter.DAL.Entities.ImpactStatistics;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.BLL.Commands.Admin.MainPage.Update;

public class UpdateMainPageHandler : IRequestHandler<UpdateMainPageCommand, Result<MainPageDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateMainPageCommand> _validator;

    public UpdateMainPageHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IValidator<UpdateMainPageCommand> validator)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<MainPageDto>> Handle(UpdateMainPageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            using var transaction = _repositoryWrapper.BeginTransaction();

            var entity = await GetMainPageAggregateAsync();
            if (entity is null)
            {
                return Result.Fail<MainPageDto>(ErrorMessagesConstants.NotFound());
            }

            var imageValidationResult = await ValidateImagesExistAsync(request.UpdateMainPageDto);
            if (imageValidationResult.IsFailed)
            {
                return Result.Fail<MainPageDto>(imageValidationResult.Errors);
            }

            UpdateBaseFields(entity, request.UpdateMainPageDto);
            UpdateSections(entity, request.UpdateMainPageDto);
            SyncImpactStatistics(entity, request.UpdateMainPageDto.ImpactStatistics);

            await _repositoryWrapper.SaveChangesAsync();

            await SyncLocalizationsAsync(entity, request.UpdateMainPageDto.ImpactStatistics);

            var resultEntity = await GetMainPageAggregateAsync(entity.Id);
            if (resultEntity is null)
            {
                return Result.Fail<MainPageDto>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(MainPageEntity)));
            }

            transaction.Complete();

            return Result.Ok(_mapper.Map<MainPageEntity, MainPageDto>(resultEntity));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<MainPageDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<MainPageDto>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(MainPageEntity)));
        }
    }

    private async Task<MainPageEntity?> GetMainPageAggregateAsync(long? id = null)
    {
        var options = new QueryOptions<MainPageEntity>
        {
            AsNoTracking = false,
            Include = q => q
                .Include(e => e.Image)
                .Include(e => e.Localizations).ThenInclude(l => l.Language)
                .Include(e => e.MainAboutUs).ThenInclude(a => a!.Localizations).ThenInclude(l => l.Language)
                .Include(e => e.MainPartners).ThenInclude(p => p!.Localizations).ThenInclude(l => l.Language)
                .Include(e => e.ImpactStatistics).ThenInclude(s => s!.Image)
                .Include(e => e.ImpactStatistics).ThenInclude(s => s!.Localizations)
                .Include(e => e.ImpactStatistics).ThenInclude(s => s!.Metrics).ThenInclude(m => m.Localizations),
        };

        if (id.HasValue)
        {
            options.Filter = e => e.Id == id.Value;
        }

        return await _repositoryWrapper.MainPageRepository.GetFirstOrDefaultAsync(options);
    }

    private async Task<Result> ValidateImagesExistAsync(UpdateMainPageDto requestDto)
    {
        var requestedImageIds = new HashSet<long>();

        if (requestDto.ImageId.HasValue)
        {
            requestedImageIds.Add(requestDto.ImageId.Value);
        }

        if (requestDto.ImpactStatistics?.ImageId.HasValue == true)
        {
            requestedImageIds.Add(requestDto.ImpactStatistics.ImageId!.Value);
        }

        if (requestedImageIds.Count == 0)
        {
            return Result.Ok();
        }

        var existingImageIds = (await _repositoryWrapper.ImageRepository.GetAllAsync(new QueryOptions<Image>
        {
            Filter = i => requestedImageIds.Contains(i.Id),
        }))
        .Select(i => i.Id)
        .ToHashSet();

        var nonExistingImageIds = requestedImageIds.Except(existingImageIds).ToList();
        if (nonExistingImageIds.Count > 0)
        {
            return Result.Fail(ErrorMessagesConstants.NotFound(nonExistingImageIds, typeof(Image)));
        }

        return Result.Ok();
    }

    private void UpdateBaseFields(MainPageEntity entity, UpdateMainPageDto dto)
    {
        entity.Title = dto.Title;
        entity.Description = dto.Description;
        entity.ImageId = dto.ImageId;
    }

    private void UpdateSections(MainPageEntity entity, UpdateMainPageDto dto)
    {
        if (dto.MainAboutUs is not null)
        {
            if (entity.MainAboutUs is null)
            {
                entity.MainAboutUs = _mapper.Map<DAL.Entities.MainAboutUs>(dto.MainAboutUs);
            }
            else
            {
                _mapper.Map(dto.MainAboutUs, entity.MainAboutUs);
            }
        }

        if (dto.MainPartners is not null)
        {
            if (entity.MainPartners is null)
            {
                entity.MainPartners = _mapper.Map<DAL.Entities.MainPartners>(dto.MainPartners);
            }
            else
            {
                _mapper.Map(dto.MainPartners, entity.MainPartners);
            }
        }
    }

    private void SyncImpactStatistics(MainPageEntity entity, UpdateImpactStatisticDto? statDto)
    {
        if (statDto is null)
        {
            return;
        }

        if (entity.ImpactStatistics is null)
        {
            entity.ImpactStatistics = _mapper.Map<ImpactStatisticsEntity>(statDto);
            entity.ImpactStatistics.MainPageId = entity.Id;
            return;
        }

        entity.ImpactStatistics.Title = statDto.Title;
        entity.ImpactStatistics.ImageId = statDto.ImageId;

        SyncMetrics(entity.ImpactStatistics, statDto.Metrics);
    }

    private void SyncMetrics(ImpactStatisticsEntity stat, ICollection<UpdateMetricDto> metricsDto)
    {
        var existingMetricsById = stat.Metrics.ToDictionary(m => m.Id);
        var requestMetricIds = metricsDto.Where(m => m.Id.HasValue).Select(m => m.Id!.Value).ToHashSet();

        foreach (var existingMetric in stat.Metrics.ToList())
        {
            if (!requestMetricIds.Contains(existingMetric.Id))
            {
                stat.Metrics.Remove(existingMetric);
            }
        }

        long nextPriority = stat.Metrics.Any()
            ? stat.Metrics.Max(m => m.Priority) + 1
            : 1;

        foreach (var metricDto in metricsDto)
        {
            if (metricDto.Id.HasValue && existingMetricsById.TryGetValue(metricDto.Id.Value, out var existingMetric))
            {
                existingMetric.Value = metricDto.Value;
                existingMetric.Name = metricDto.Name;
                existingMetric.Type = metricDto.Type;
                existingMetric.Prefix = metricDto.Prefix;
                existingMetric.IsAutoSynced = metricDto.IsAutoSynced;
            }
            else
            {
                var newMetric = _mapper.Map<Metric>(metricDto);
                newMetric.Statistics = stat;

                newMetric.Priority = nextPriority++;

                stat.Metrics.Add(newMetric);
            }
        }
    }

    private async Task SyncLocalizationsAsync(MainPageEntity entity, UpdateImpactStatisticDto? statDto)
    {
        var impactEntity = entity.ImpactStatistics;
        if (impactEntity is null || statDto is null)
        {
            return;
        }

        bool hasChanges = false;

        if (statDto.Localization is not null)
        {
            var existingLoc = impactEntity.Localizations
                .FirstOrDefault(l => l.LanguageId == statDto.Localization.LanguageId);

            if (existingLoc is not null)
            {
                existingLoc.Title = statDto.Localization.Title;
                existingLoc.TranslationStatus = TranslationStatus.Relevant;
            }
            else
            {
                await _repositoryWrapper.ImpactStatisticsLocalizationsRepository.CreateAsync(
                    new ImpactStatisticsLocalization
                    {
                        EntityId = impactEntity.Id,
                        LanguageId = statDto.Localization.LanguageId,
                        Title = statDto.Localization.Title,
                    });
            }

            hasChanges = true;
        }

        var metricsByType = impactEntity.Metrics.ToDictionary(m => m.Type);

        foreach (var metricDto in statDto.Metrics)
        {
            if (metricDto.Localization is null || !metricsByType.TryGetValue(metricDto.Type, out var metricEntity))
            {
                continue;
            }

            var existingMetricLoc = metricEntity.Localizations
                .FirstOrDefault(l => l.LanguageId == metricDto.Localization.LanguageId);

            if (existingMetricLoc is not null)
            {
                existingMetricLoc.Value = metricDto.Localization.Value;
                existingMetricLoc.Name = metricDto.Localization.Name;
                existingMetricLoc.TranslationStatus = TranslationStatus.Relevant;
            }
            else
            {
                await _repositoryWrapper.MetricLocalizationsRepository.CreateAsync(
                    new MetricLocalization
                    {
                        EntityId = metricEntity.Id,
                        LanguageId = metricDto.Localization.LanguageId,
                        Value = metricDto.Localization.Value,
                        Name = metricDto.Localization.Name,
                    });
            }

            hasChanges = true;
        }

        if (hasChanges)
        {
            await _repositoryWrapper.SaveChangesAsync();
        }
    }
}
