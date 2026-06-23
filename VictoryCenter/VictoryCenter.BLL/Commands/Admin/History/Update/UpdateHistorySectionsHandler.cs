using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HistorySection;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.DAL.Entities.HistoryContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace VictoryCenter.BLL.Commands.Admin.History.Update;

public class UpdateHistorySectionsHandler : IRequestHandler<UpdateHistorySectionsCommand, Result<List<HistorySectionDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateHistorySectionsCommand> _validator;

    public UpdateHistorySectionsHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateHistorySectionsCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<List<HistorySectionDto>>> Handle(
        UpdateHistorySectionsCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var existingSections = (await _repositoryWrapper.HistorySectionsRepository.GetAllAsync(new QueryOptions<HistorySection>
            {
                Include = q => q.Include(s => s.Contents)
            })).ToList();

            var incomingSections = request.UpdateSections.ToList();

            if (existingSections.Count == 0 && incomingSections.Count == 0)
            {
                return Result.Ok<List<HistorySectionDto>>([]);
            }

            var existingSectionIds = existingSections.Select(s => s.Id).ToHashSet();
            var unknownIds = incomingSections
                .Where(s => s.Id > 0 && !existingSectionIds.Contains(s.Id))
                .Select(s => s.Id)
                .ToList();

            if (unknownIds.Count > 0)
            {
                return Result.Fail<List<HistorySectionDto>>(
                    ErrorMessagesConstants.NotFound(unknownIds, typeof(HistorySection)));
            }

            var contentMismatches = incomingSections
                .SelectMany(s => s.Contents ?? [])
                .Where(c => c.Id > 0)
                .Where(c =>
                {
                    var oldContent = existingSections
                        .SelectMany(s => s.Contents)
                        .FirstOrDefault(oc => oc.Id == c.Id);

                    return oldContent != null && oldContent.ContentType != c.ContentType;
                })
                .Select(c => c.Id)
                .ToList();

            if (contentMismatches.Count > 0)
            {
                return Result.Fail<List<HistorySectionDto>>(
                    $"Content type mismatch for content ID(s): {string.Join(", ", contentMismatches)}");
            }

            var imagesByIdResult = await ImageValidationHelper.ValidateAndGetSectionImagesAsync(
                _repositoryWrapper,
                incomingSections.ToList());

            if (imagesByIdResult.IsFailed)
            {
                return Result.Fail<List<HistorySectionDto>>(imagesByIdResult.Errors);
            }

            var now = DateTimeOffset.UtcNow;

            using var transaction = _repositoryWrapper.BeginTransaction();

            var finalSections = await ReplaceSections(
                existingSections,
                incomingSections,
                now,
                imagesByIdResult.Value);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<List<HistorySectionDto>>(
                    ErrorMessagesConstants.FailedToUpdateEntity(typeof(HistorySection)));
            }

            transaction.Complete();

            return Result.Ok(_mapper.Map<List<HistorySectionDto>>(finalSections));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<List<HistorySectionDto>>(vex.Errors.Select(e => e.ErrorMessage));
        }
    }

    private async Task<List<HistorySection>> ReplaceSections(
        List<HistorySection> oldSections,
        List<UpdateHistorySectionDto> newSections,
        DateTimeOffset createdAt,
        IReadOnlyDictionary<long, Image> imagesById)
    {
        var result = new List<HistorySection>();
        var changedContentIds = new List<long>();
        var oldSectionsDict = oldSections.ToDictionary(s => s.Id);

        var sectionsToRemove = oldSections.Where(os => !newSections.Any(ns => ns.Id == os.Id)).ToList();
        if (sectionsToRemove.Count > 0)
        {
            _repositoryWrapper.HistorySectionsRepository.DeleteRange(sectionsToRemove);
        }

        foreach (var newSectionDto in newSections)
        {
            if (newSectionDto.Id > 0 && oldSectionsDict.TryGetValue(newSectionDto.Id, out var existingSection))
            {
                existingSection.Template = newSectionDto.Template;
                existingSection.Order = newSectionDto.Order;

                var oldContentsDict = existingSection.Contents.ToDictionary(c => c.Id);
                var newContentsDto = newSectionDto.Contents ?? new List<UpdateHistorySectionContentDto>();

                var contentsToRemove = existingSection.Contents.Where(oc => !newContentsDto.Any(nc => nc.Id == oc.Id)).ToList();
                if (contentsToRemove.Count > 0)
                {
                    _repositoryWrapper.HistorySectionContentsRepository.DeleteRange(contentsToRemove);
                    foreach (var contentToRemove in contentsToRemove)
                    {
                        existingSection.Contents.Remove(contentToRemove);
                    }
                }

                foreach (var newContentDto in newContentsDto.OrderBy(x => x.Order))
                {
                    if (newContentDto.Id > 0 && oldContentsDict.TryGetValue(newContentDto.Id, out var existingContent))
                    {
                        bool textChanged = false;

                        if (existingContent is TitleHistoryContent thc)
                        {
                            if (thc.Title != newContentDto.Title?.Trim())
                            {
                                textChanged = true;
                            }

                            thc.Title = newContentDto.Title?.Trim();
                            thc.Order = newContentDto.Order;
                        }
                        else if (existingContent is DescriptionHistoryContent dhc)
                        {
                            if (dhc.Description != newContentDto.Description?.Trim())
                            {
                                textChanged = true;
                            }

                            dhc.Description = newContentDto.Description?.Trim();
                            dhc.Order = newContentDto.Order;
                        }
                        else if (existingContent is ImageHistoryContent ihc)
                        {
                            ihc.Order = newContentDto.Order;
                            if (newContentDto.ImageId.HasValue && imagesById.TryGetValue(newContentDto.ImageId.Value, out var img))
                            {
                                ihc.ImageId = newContentDto.ImageId.Value;
                                ihc.Image = img;
                            }
                        }

                        _repositoryWrapper.HistorySectionContentsRepository.Update(existingContent);

                        if (textChanged)
                        {
                            changedContentIds.Add(existingContent.Id);
                        }
                    }
                    else
                    {
                        var newContent = CreateContent(newContentDto, imagesById);
                        if (newContent != null)
                        {
                            existingSection.Contents.Add(newContent);
                        }
                    }
                }

                _repositoryWrapper.HistorySectionsRepository.Update(existingSection);
                result.Add(existingSection);
            }
            else
            {
                var newSection = new HistorySection
                {
                    Template = newSectionDto.Template,
                    Order = newSectionDto.Order,
                    CreatedAt = createdAt,
                    Contents = BuildContents(newSectionDto, imagesById)
                };
                await _repositoryWrapper.HistorySectionsRepository.CreateAsync(newSection);
                result.Add(newSection);
            }
        }

        if (changedContentIds.Count > 0)
        {
            var localizations = (await _repositoryWrapper.HistorySectionContentLocalizationsRepository
                .GetAllAsync(new QueryOptions<HistorySectionContentLocalization>
                {
                    Filter = l => changedContentIds.Contains(l.EntityId)
                })).ToList();

            if (localizations.Count > 0)
            {
                foreach (var loc in localizations)
                {
                    loc.TranslationStatus = TranslationStatus.Outdated;
                }

                _repositoryWrapper.HistorySectionContentLocalizationsRepository.UpdateRange(localizations);
            }
        }

        return result;
    }

    private List<HistorySectionContent> BuildContents(
        UpdateHistorySectionDto sectionDto,
        IReadOnlyDictionary<long, Image> imagesById)
    {
        var dtoContents = sectionDto.Contents ?? [];
        if (dtoContents.Count == 0)
        {
            return [];
        }

        var contents = new List<HistorySectionContent>(dtoContents.Count);

        foreach (var dto in dtoContents.OrderBy(x => x.Order))
        {
            var entity = CreateContent(dto, imagesById);
            if (entity != null)
            {
                contents.Add(entity);
            }
        }

        return contents;
    }

    private HistorySectionContent? CreateContent(
        UpdateHistorySectionContentDto dto,
        IReadOnlyDictionary<long, Image> imagesById)
    {
        if (dto.ContentType == ContentType.Title)
        {
            return new TitleHistoryContent
            {
                ContentType = ContentType.Title,
                Order = dto.Order,
                Title = dto.Title?.Trim()
            };
        }

        if (dto.ContentType == ContentType.Description)
        {
            return new DescriptionHistoryContent
            {
                ContentType = ContentType.Description,
                Order = dto.Order,
                Description = dto.Description?.Trim()
            };
        }

        if (dto.ContentType == ContentType.Image)
        {
            if (dto.ImageId is null or <= 0)
            {
                return null;
            }

            if (!imagesById.TryGetValue(dto.ImageId.Value, out var image))
            {
                return null;
            }

            return new ImageHistoryContent
            {
                ContentType = ContentType.Image,
                Order = dto.Order,
                ImageId = dto.ImageId.Value,
                Image = image
            };
        }

        return null;
    }
}
