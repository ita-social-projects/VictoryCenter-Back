using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HistorySection;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HistoryContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

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

            var existingSections = await _repositoryWrapper.HistorySectionsRepository.GetAllAsync(new QueryOptions<HistorySection>
            {
                AsNoTracking = false,
                Include = s => s.Include(s => s.Contents)
            });

            var incomingSections = request.UpdateSections.ToList();

            var toUpdateIds = incomingSections.Where(s => s.Id.HasValue).Select(s => s.Id!.Value);
            var notFoundIds = toUpdateIds.Except(existingSections.Select(s => s.Id));

            if (notFoundIds.Any())
            {
                return Result.Fail<List<HistorySectionDto>>(
                    ErrorMessagesConstants.NotFound(notFoundIds, typeof(HistorySection)));
            }

            var imagesByIdResult = await ImageValidationHelper.ValidateAndGetSectionImagesAsync(
                _repositoryWrapper,
                incomingSections.Cast<CreateHistorySectionDto>().ToList());

            if (imagesByIdResult.IsFailed)
            {
                return Result.Fail<List<HistorySectionDto>>(imagesByIdResult.Errors);
            }

            var now = DateTimeOffset.UtcNow;

            using var transaction = _repositoryWrapper.BeginTransaction();

            var oldSections = existingSections.ToList();
            List<HistorySection> finalSections;
            var replacedSections = false;

            var sectionsChanged = EnsureReplaceSameSections(oldSections, incomingSections, imagesByIdResult.Value);
            if (!sectionsChanged)
            {
                replacedSections = true;

                if (oldSections.Count > 0)
                {
                    _repositoryWrapper.HistorySectionsRepository.DeleteRange(oldSections);
                }

                var rebuiltSections = HistorySectionsBuilder.Build(
                    incomingSections.Cast<CreateHistorySectionDto>().ToList(),
                    now,
                    imagesByIdResult.Value);

                if (rebuiltSections.Count > 0)
                {
                    await _repositoryWrapper.HistorySectionsRepository.CreateRangeAsync(rebuiltSections);
                }

                finalSections = rebuiltSections;
            }
            else
            {
                finalSections = oldSections;
            }

            var affectedRows = await _repositoryWrapper.SaveChangesAsync();

            if (replacedSections && affectedRows <= 0)
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

    private static bool EnsureReplaceSameSections(
        List<HistorySection> oldSections,
        List<UpdateHistorySectionDto> newSections,
        IReadOnlyDictionary<long, Image> imagesById)
    {
        if (oldSections.Count != newSections.Count)
        {
            return false;
        }

        Dictionary<int, HistorySection> oldSectionsMap;
        Dictionary<int, UpdateHistorySectionDto> newSectionsMap;

        try
        {
            oldSectionsMap = oldSections.ToDictionary(section => section.Order);
            newSectionsMap = newSections.ToDictionary(section => section.Order);
        }
        catch (ArgumentException)
        {
            return false;
        }

        if (oldSectionsMap.Count != newSectionsMap.Count)
        {
            return false;
        }

        foreach (var (sectionOrder, newSection) in newSectionsMap)
        {
            if (!oldSectionsMap.TryGetValue(sectionOrder, out var oldSection))
            {
                return false;
            }

            if (oldSection.Template != newSection.Template)
            {
                return false;
            }

            var newContents = newSection.Contents ?? [];
            var oldContents = oldSection.Contents;

            Dictionary<int, CreateHistorySectionContentDto> newContentsMap;
            Dictionary<int, HistorySectionContent> oldContentsMap;

            try
            {
                newContentsMap = newContents.ToDictionary(content => content.Order);
                oldContentsMap = oldContents.ToDictionary(content => content.Order);
            }
            catch (ArgumentException)
            {
                return false;
            }

            if (newContentsMap.Count != oldContentsMap.Count)
            {
                return false;
            }

            foreach (var (contentOrder, newContent) in newContentsMap)
            {
                if (!oldContentsMap.TryGetValue(contentOrder, out var oldContent))
                {
                    return false;
                }

                if (newContent.ContentType != oldContent.ContentType)
                {
                    return false;
                }

                if (!TryApplyContentFieldUpdates(oldContent, newContent, imagesById, out var contentChanged))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static bool TryApplyContentFieldUpdates(
        HistorySectionContent oldContent,
        CreateHistorySectionContentDto newContent,
        IReadOnlyDictionary<long, Image> imagesById,
        out bool contentChanged)
    {
        contentChanged = false;

        return newContent.ContentType switch
        {
            ContentType.Title when oldContent is TitleHistoryContent titleContent
                => UpdateTitleContent(titleContent, newContent, out contentChanged),
            ContentType.Description when oldContent is DescriptionHistoryContent descriptionContent
                => UpdateDescriptionContent(descriptionContent, newContent, out contentChanged),
            ContentType.Image when oldContent is ImageHistoryContent imageContent
                => UpdateImageContent(imageContent, newContent, imagesById, out contentChanged),
            _ => false,
        };
    }

    private static bool UpdateTitleContent(
        TitleHistoryContent content,
        CreateHistorySectionContentDto source,
        out bool changed)
    {
        changed = false;
        if (source.Title is null)
        {
            return false;
        }

        var newValue = source.Title.Trim();
        changed = !string.Equals(content.Title, newValue, StringComparison.Ordinal);
        content.Title = newValue;
        return true;
    }

    private static bool UpdateDescriptionContent(
        DescriptionHistoryContent content,
        CreateHistorySectionContentDto source,
        out bool changed)
    {
        changed = false;
        if (source.Description is null)
        {
            return false;
        }

        var newValue = source.Description.Trim();
        changed = !string.Equals(content.Description, newValue, StringComparison.Ordinal);
        content.Description = newValue;
        return true;
    }

    private static bool UpdateImageContent(
        ImageHistoryContent content,
        CreateHistorySectionContentDto source,
        IReadOnlyDictionary<long, Image> imagesById,
        out bool changed)
    {
        changed = false;
        if (source.ImageId is null || !imagesById.TryGetValue(source.ImageId.Value, out var image))
        {
            return false;
        }

        changed = false;
        content.ImageId = source.ImageId.Value;
        content.Image = image;
        return true;
    }
}
