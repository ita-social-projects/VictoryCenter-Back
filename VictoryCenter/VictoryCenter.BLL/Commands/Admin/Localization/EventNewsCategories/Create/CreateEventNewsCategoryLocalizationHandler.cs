using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNewsCategories;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.EventNewsCategories.Create;

public class CreateEventNewsCategoryLocalizationHandler
    : IRequestHandler<CreateEventNewsCategoryLocalizationCommand, Result<AdminEventNewsCategoryLocalizationDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public CreateEventNewsCategoryLocalizationHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<AdminEventNewsCategoryLocalizationDto>> Handle(
        CreateEventNewsCategoryLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Localization;
        if (!await _repositoryWrapper.EventNewsCategoryRepository.ExistsAsync(category => category.Id == dto.EntityId))
        {
            return Result.Fail<AdminEventNewsCategoryLocalizationDto>(
                ErrorMessagesConstants.NotFound(dto.EntityId, typeof(EventNewsCategory)));
        }

        var language = await _repositoryWrapper.LocalizationLanguagesRepository.GetFirstOrDefaultAsync(
            new QueryOptions<LocalizationLanguage>
            {
                Filter = entity => entity.Id == dto.LanguageId,
                AsNoTracking = true
            });

        if (language is null)
        {
            return Result.Fail<AdminEventNewsCategoryLocalizationDto>(
                ErrorMessagesConstants.NotFound(dto.LanguageId, typeof(LocalizationLanguage)));
        }

        if (await _repositoryWrapper.EventNewsCategoryLocalizationsRepository.ExistsAsync(
                localization => localization.EntityId == dto.EntityId && localization.LanguageId == dto.LanguageId))
        {
            return Result.Fail<AdminEventNewsCategoryLocalizationDto>(
                EventNewsCategoryConstants.LocalizationAlreadyExists);
        }

        var normalizedName = dto.Name.Trim();
        if (await _repositoryWrapper.EventNewsCategoryLocalizationsRepository.ExistsAsync(
                localization => localization.LanguageId == dto.LanguageId && localization.Name == normalizedName))
        {
            return Result.Fail<AdminEventNewsCategoryLocalizationDto>(
                EventNewsCategoryConstants.DuplicateLocalizedName);
        }

        var localization = new EventNewsCategoryLocalization
        {
            EntityId = dto.EntityId,
            LanguageId = dto.LanguageId,
            Name = normalizedName,
            TranslationStatus = TranslationStatus.Relevant,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _repositoryWrapper.EventNewsCategoryLocalizationsRepository.CreateAsync(localization);

        try
        {
            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(new AdminEventNewsCategoryLocalizationDto
                {
                    EntityId = localization.EntityId,
                    Language = new LocalizationInfoDto
                    {
                        Id = language.Id,
                        Code = language.Code
                    },
                    Name = localization.Name,
                    TranslationStatus = localization.TranslationStatus
                });
            }
        }
        catch (DbUpdateException exception) when (exception.IsUniqueConstraintException())
        {
            var localizationAlreadyExists =
                await _repositoryWrapper.EventNewsCategoryLocalizationsRepository.ExistsAsync(
                    entity => entity.EntityId == dto.EntityId && entity.LanguageId == dto.LanguageId);

            return Result.Fail<AdminEventNewsCategoryLocalizationDto>(
                localizationAlreadyExists
                    ? EventNewsCategoryConstants.LocalizationAlreadyExists
                    : EventNewsCategoryConstants.DuplicateLocalizedName);
        }

        return Result.Fail<AdminEventNewsCategoryLocalizationDto>(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(EventNewsCategoryLocalization)));
    }
}
