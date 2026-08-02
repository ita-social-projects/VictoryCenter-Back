using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNewsCategories;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.EventNewsCategories.Update;

public class UpdateEventNewsCategoryLocalizationHandler
    : IRequestHandler<UpdateEventNewsCategoryLocalizationCommand, Result<AdminEventNewsCategoryLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public UpdateEventNewsCategoryLocalizationHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<AdminEventNewsCategoryLocalizationDto>> Handle(
        UpdateEventNewsCategoryLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        var localization = await _repositoryWrapper.EventNewsCategoryLocalizationsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<EventNewsCategoryLocalization>
            {
                Filter = entity => entity.EntityId == request.EntityId && entity.LanguageId == request.LanguageId,
                Include = query => query.Include(entity => entity.Language),
                AsNoTracking = false
            });

        if (localization is null)
        {
            return Result.Fail<AdminEventNewsCategoryLocalizationDto>(ErrorMessagesConstants.NotFound(
                (request.EntityId, request.LanguageId),
                typeof(EventNewsCategoryLocalization)));
        }

        var normalizedName = request.Localization.Name.Trim();
        if (await _repositoryWrapper.EventNewsCategoryLocalizationsRepository.ExistsAsync(
                entity => entity.LanguageId == request.LanguageId
                    && entity.EntityId != request.EntityId
                    && entity.Name == normalizedName))
        {
            return Result.Fail<AdminEventNewsCategoryLocalizationDto>(
                EventNewsCategoryConstants.DuplicateLocalizedName);
        }

        if (string.Equals(localization.Name, normalizedName, StringComparison.Ordinal)
            && localization.TranslationStatus == TranslationStatus.Relevant)
        {
            return Result.Ok(_mapper.Map<AdminEventNewsCategoryLocalizationDto>(localization));
        }

        localization.Name = normalizedName;
        localization.TranslationStatus = TranslationStatus.Relevant;

        try
        {
            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(_mapper.Map<AdminEventNewsCategoryLocalizationDto>(localization));
            }
        }
        catch (DbUpdateException exception) when (exception.IsUniqueConstraintException())
        {
            return Result.Fail<AdminEventNewsCategoryLocalizationDto>(
                EventNewsCategoryConstants.DuplicateLocalizedName);
        }

        return Result.Fail<AdminEventNewsCategoryLocalizationDto>(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(EventNewsCategoryLocalization)));
    }
}
