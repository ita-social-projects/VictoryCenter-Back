using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
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

            var entity = await _repositoryWrapper.MainPageRepository
                .GetFirstOrDefaultAsync(new QueryOptions<MainPageEntity>
                {
                    AsNoTracking = false,
                    Include = q => q
                        .Include(e => e.Image)
                        .Include(e => e.MainAboutUs)
                        .Include(e => e.MainPartners)
                        .Include(e => e.ImpactStatistics)
                            .ThenInclude(s => s.Image)
                        .Include(e => e.ImpactStatistics)
                            .ThenInclude(s => s.Metrics)
                });

            if (entity is null)
            {
                return Result.Fail<MainPageDto>(ErrorMessagesConstants.NotFound());
            }

            var impactStatisticsDto = request.UpdateMainPageDto.ImpactStatistics ?? [];

            var requestedImageIds = new HashSet<long>();
            if (request.UpdateMainPageDto.ImageId.HasValue)
            {
                requestedImageIds.Add(request.UpdateMainPageDto.ImageId.Value);
            }

            foreach (var imageId in impactStatisticsDto.Where(s => s.ImageId.HasValue).Select(s => s.ImageId!.Value))
            {
                requestedImageIds.Add(imageId);
            }

            if (requestedImageIds.Count > 0)
            {
                var existingImageIds = (await _repositoryWrapper.ImageRepository.GetAllAsync(new QueryOptions<Image>
                {
                    Filter = i => requestedImageIds.Contains(i.Id)
                }))
                .Select(i => i.Id)
                .ToHashSet();

                var nonExistingImageIds = requestedImageIds.Except(existingImageIds).ToList();
                if (nonExistingImageIds.Count > 0)
                {
                    return Result.Fail<MainPageDto>(
                        ErrorMessagesConstants.NotFound(nonExistingImageIds, typeof(Image)));
                }
            }

            entity.Title = request.UpdateMainPageDto.Title;
            entity.Description = request.UpdateMainPageDto.Description;
            entity.ImageId = request.UpdateMainPageDto.ImageId;

            if (request.UpdateMainPageDto.MainAboutUs is not null)
            {
                if (entity.MainAboutUs is null)
                {
                    entity.MainAboutUs = _mapper.Map<DAL.Entities.MainAboutUs>(request.UpdateMainPageDto.MainAboutUs);
                }
                else
                {
                    _mapper.Map(request.UpdateMainPageDto.MainAboutUs, entity.MainAboutUs);
                }
            }

            if (request.UpdateMainPageDto.MainPartners is not null)
            {
                if (entity.MainPartners is null)
                {
                    entity.MainPartners = _mapper.Map<DAL.Entities.MainPartners>(request.UpdateMainPageDto.MainPartners);
                }
                else
                {
                    _mapper.Map(request.UpdateMainPageDto.MainPartners, entity.MainPartners);
                }
            }

            var existingStatsById = entity.ImpactStatistics.ToDictionary(s => s.Id);

            var requestStatIds = impactStatisticsDto
                .Where(s => s.Id.HasValue)
                .Select(s => s.Id!.Value)
                .ToHashSet();

            foreach (var existingStat in entity.ImpactStatistics.ToList())
            {
                if (!requestStatIds.Contains(existingStat.Id))
                {
                    entity.ImpactStatistics.Remove(existingStat);
                }
            }

            foreach (var statDto in impactStatisticsDto)
            {
                if (statDto.Id.HasValue && existingStatsById.TryGetValue(statDto.Id.Value, out var existingStat))
                {
                    existingStat.Description = statDto.Description;
                    existingStat.ImageId = statDto.ImageId;

                    var existingMetricsById = existingStat.Metrics.ToDictionary(m => m.Id);

                    var requestMetricIds = statDto.Metrics
                        .Where(m => m.Id.HasValue)
                        .Select(m => m.Id!.Value)
                        .ToHashSet();

                    foreach (var existingMetric in existingStat.Metrics.ToList())
                    {
                        if (!requestMetricIds.Contains(existingMetric.Id))
                        {
                            existingStat.Metrics.Remove(existingMetric);
                        }
                    }

                    foreach (var metricDto in statDto.Metrics)
                    {
                        if (metricDto.Id.HasValue && existingMetricsById.TryGetValue(metricDto.Id.Value, out var existingMetric))
                        {
                            existingMetric.Value = metricDto.Value;
                            existingMetric.Signature = metricDto.Signature;
                        }
                        else
                        {
                            var newMetric = _mapper.Map<Metric>(metricDto);
                            newMetric.Statistics = existingStat;
                            existingStat.Metrics.Add(newMetric);
                        }
                    }
                }
                else
                {
                    var newStat = _mapper.Map<ImpactStatistics>(statDto);
                    newStat.MainPageId = entity.Id;

                    foreach (var metric in newStat.Metrics)
                    {
                        metric.Statistics = newStat;
                    }

                    entity.ImpactStatistics.Add(newStat);
                }
            }

            await _repositoryWrapper.SaveChangesAsync();

            var resultEntity = await _repositoryWrapper.MainPageRepository
                .GetFirstOrDefaultAsync(new QueryOptions<MainPageEntity>
                {
                    Filter = e => e.Id == entity.Id,
                    Include = q => q
                        .Include(e => e.Image)
                        .Include(e => e.MainAboutUs)
                        .Include(e => e.MainPartners)
                        .Include(e => e.ImpactStatistics)
                            .ThenInclude(s => s.Image)
                        .Include(e => e.ImpactStatistics)
                            .ThenInclude(s => s.Metrics)
                });

            if (resultEntity is null)
            {
                return Result.Fail<MainPageDto>(
                    ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(MainPageEntity)));
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
            return Result.Fail<MainPageDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(MainPageEntity)));
        }
    }
}