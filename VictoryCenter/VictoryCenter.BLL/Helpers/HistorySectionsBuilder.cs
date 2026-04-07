using VictoryCenter.BLL.DTOs.Admin.HistorySection;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HistoryContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Helpers;

public static class HistorySectionsBuilder
{
    public static List<HistorySection> Build(
        ICollection<CreateHistorySectionDto>? sections,
        DateTimeOffset createdAt,
        IReadOnlyDictionary<long, Image> imagesById)
    {
        if (sections is null || sections.Count == 0)
        {
            return [];
        }

        var result = new List<HistorySection>(sections.Count);

        foreach (var sectionDto in sections)
        {
            result.Add(new HistorySection
            {
                Template = sectionDto.Template,
                Order = sectionDto.Order,
                CreatedAt = createdAt,
                Contents = BuildContents(sectionDto, imagesById)
            });
        }

        return result;
    }

    private static List<HistorySectionContent> BuildContents(
        CreateHistorySectionDto sectionDto,
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
            if (entity is null)
            {
                continue;
            }

            contents.Add(entity);
        }

        return contents;
    }

    private static HistorySectionContent? CreateContent(
        CreateHistorySectionContentDto dto,
        IReadOnlyDictionary<long, Image> imagesById)
    {
        if (dto.ContentType == ContentType.Title)
        {
            return new TitleHistoryContent
            {
                ContentType = ContentType.Title,
                Order = dto.Order,
                Title = dto.Title!.Trim()
            };
        }

        if (dto.ContentType == ContentType.Description)
        {
            return new DescriptionHistoryContent
            {
                ContentType = ContentType.Description,
                Order = dto.Order,
                Description = dto.Description!.Trim()
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
