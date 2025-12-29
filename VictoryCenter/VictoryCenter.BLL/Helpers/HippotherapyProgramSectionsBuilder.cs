using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Helpers;

public static class HippotherapyProgramSectionsBuilder
{
    public static List<HippotherapyProgramSection> Build(
        ICollection<CreateHippotherapyProgramSectionDto>? sections,
        DateTimeOffset createdAt,
        IReadOnlyDictionary<long, Image> imagesById)
    {
        if (sections is null || sections.Count == 0)
        {
            return [];
        }

        var result = new List<HippotherapyProgramSection>(sections.Count);

        foreach (var sectionDto in sections)
        {
            result.Add(new HippotherapyProgramSection
            {
                Template = sectionDto.Template,
                Order = sectionDto.Order,
                CreatedAt = createdAt,
                Contents = BuildContents(sectionDto, imagesById)
            });
        }

        return result;
    }

    private static List<ProgramSectionContent> BuildContents(
        CreateHippotherapyProgramSectionDto sectionDto,
        IReadOnlyDictionary<long, Image> imagesById)
    {
        var contents = new List<ProgramSectionContent>(GetCapacity(sectionDto));
        var order = 0;

        AddTitles(contents, sectionDto.Titles, ref order);
        AddDescriptions(contents, sectionDto.Descriptions, ref order);
        AddImages(contents, sectionDto.ImageIds, imagesById, ref order);

        return contents;
    }

    private static int GetCapacity(CreateHippotherapyProgramSectionDto sectionDto)
    {
        return
            (sectionDto.Titles?.Count ?? 0) +
            (sectionDto.Descriptions?.Count ?? 0) +
            (sectionDto.ImageIds?.Count ?? 0);
    }

    private static void AddTitles(
        List<ProgramSectionContent> contents,
        List<string>? titles,
        ref int order)
    {
        if (titles is null || titles.Count == 0)
        {
            return;
        }

        foreach (var title in titles)
        {
            contents.Add(new TitleProgramContent
            {
                ContentType = ContentType.Title,
                Order = order++,
                Title = title.Trim()
            });
        }
    }

    private static void AddDescriptions(
        List<ProgramSectionContent> contents,
        List<string>? descriptions,
        ref int order)
    {
        if (descriptions is null || descriptions.Count == 0)
        {
            return;
        }

        foreach (var description in descriptions)
        {
            contents.Add(new DescriptionProgramContent
            {
                ContentType = ContentType.Description,
                Order = order++,
                Description = description.Trim()
            });
        }
    }

    private static void AddImages(
        List<ProgramSectionContent> contents,
        List<long>? imageIds,
        IReadOnlyDictionary<long, Image> imagesById,
        ref int order)
    {
        if (imageIds is null || imageIds.Count == 0)
        {
            return;
        }

        foreach (var imageId in imageIds)
        {
            if (!imagesById.TryGetValue(imageId, out var image))
            {
                continue;
            }

            contents.Add(new ImageProgramContent
            {
                ContentType = ContentType.Image,
                Order = order++,
                ImageId = imageId,
                Image = image
            });
        }
    }
}
