using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.BLL.Commands.Admin.MainPage.Create;

public class CreateMainPageHandler : IRequestHandler<CreateMainPageCommand, Result<MainPageDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateMainPageCommand> _validator;

    public CreateMainPageHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IValidator<CreateMainPageCommand> validator)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<MainPageDto>> Handle(CreateMainPageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            using var transaction = _repositoryWrapper.BeginTransaction();

            var entityIsExists = (await _repositoryWrapper.MainPageRepository
                .CountAsync()) > 0;

            if (entityIsExists)
            {
                return Result.Fail<MainPageDto>(
                    ErrorMessagesConstants.OnlyOneEntityOfTypeIsAllowed(nameof(DAL.Entities.MainPage)));
            }

            var requestedImageIds = new List<long>();

            if (request.CreateMainPageDto.ImageId.HasValue)
            {
                requestedImageIds.Add(request.CreateMainPageDto.ImageId.Value);
            }

            if (request.CreateMainPageDto.ImpactStatistics?.ImageId.HasValue == true)
            {
                requestedImageIds.Add(request.CreateMainPageDto.ImpactStatistics.ImageId!.Value);
            }

            if (requestedImageIds.Count > 0)
            {
                var existingImageIds = (await _repositoryWrapper.ImageRepository
                    .GetAllAsync(new QueryOptions<Image>
                    {
                        Filter = i => requestedImageIds.Contains(i.Id)
                    }))
                    .Select(i => i.Id)
                    .ToList();

                var nonExistingImageIds = requestedImageIds.Except(existingImageIds).ToList();

                if (nonExistingImageIds.Count > 0)
                {
                    return Result.Fail<MainPageDto>(
                        ErrorMessagesConstants.NotFound(nonExistingImageIds, typeof(Image)));
                }
            }

            var mainPageEntity = _mapper.Map<CreateMainPageDto, MainPageEntity>(request.CreateMainPageDto);

            if (mainPageEntity.ImpactStatistics?.Metrics != null)
            {
                int priorityIndex = 0;
                foreach (var metric in mainPageEntity.ImpactStatistics.Metrics)
                {
                    metric.Priority = priorityIndex++;
                }
            }

            await _repositoryWrapper.MainPageRepository.CreateAsync(mainPageEntity);
            await _repositoryWrapper.SaveChangesAsync();

            await SaveLocalizationsAsync(request.CreateMainPageDto, mainPageEntity);

            var resultEntity = await _repositoryWrapper.MainPageRepository
                .GetFirstOrDefaultAsync(new QueryOptions<MainPageEntity>
                {
                    Filter = e => e.Id == mainPageEntity.Id,
                    Include = q => q
                        .Include(e => e.Image)
                        .Include(e => e.Localizations).ThenInclude(l => l.Language)
                        .Include(e => e.MainAboutUs).ThenInclude(a => a!.Localizations).ThenInclude(l => l.Language)
                        .Include(e => e.MainPartners).ThenInclude(p => p!.Localizations).ThenInclude(l => l.Language)
                        .Include(e => e.ImpactStatistics).ThenInclude(s => s!.Image)
                        .Include(e => e.ImpactStatistics).ThenInclude(s => s!.Localizations).ThenInclude(l => l.Language)
                        .Include(e => e.ImpactStatistics).ThenInclude(s => s!.Metrics).ThenInclude(m => m.Localizations).ThenInclude(l => l.Language)
                });

            if (resultEntity is null)
            {
                return Result.Fail<MainPageDto>(
                    ErrorMessagesConstants.FailedToCreateEntity(typeof(MainPageEntity)));
            }

            var resultDto = _mapper.Map<MainPageEntity, MainPageDto>(resultEntity);

            transaction.Complete();

            return Result.Ok(resultDto);
        }
        catch (ValidationException vex)
        {
            return Result.Fail<MainPageDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<MainPageDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(MainPageEntity)));
        }
    }

    private async Task SaveLocalizationsAsync(CreateMainPageDto dto, MainPageEntity entity)
    {
        var impactDto = dto.ImpactStatistics;
        var impactEntity = entity.ImpactStatistics;

        if (impactEntity is null || impactDto is null)
        {
            return;
        }

        bool hasLocalizations = false;

        if (impactDto.Localization is not null)
        {
            await _repositoryWrapper.ImpactStatisticsLocalizationsRepository.CreateAsync(
                new ImpactStatisticsLocalization
                {
                    EntityId = impactEntity.Id,
                    LanguageId = impactDto.Localization.LanguageId,
                    Title = impactDto.Localization.Title,
                });
            hasLocalizations = true;
        }

        var metricsByType = impactEntity.Metrics.ToDictionary(m => m.Type);

        foreach (var metricDto in impactDto.Metrics)
        {
            if (metricDto.Localization is null || !metricsByType.TryGetValue(metricDto.Type, out var metricEntity))
            {
                continue;
            }

            await _repositoryWrapper.MetricLocalizationsRepository.CreateAsync(
                new MetricLocalization
                {
                    EntityId = metricEntity.Id,
                    LanguageId = metricDto.Localization.LanguageId,
                    Value = metricDto.Localization.Value,
                    Name = metricDto.Localization.Name,
                });
            hasLocalizations = true;
        }

        if (hasLocalizations)
        {
            await _repositoryWrapper.SaveChangesAsync();
        }
    }
}
