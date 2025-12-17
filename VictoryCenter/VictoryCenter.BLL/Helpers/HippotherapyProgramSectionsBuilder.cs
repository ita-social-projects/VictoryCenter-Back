using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Helpers;

public static class HippotherapyProgramSectionsBuilder
{
    public static List<HippotherapyProgramSection> Build(
        List<CreateHippotherapyProgramSectionDto>? sections,
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
        var capacity =
            (sectionDto.Titles?.Count ?? 0) +
            (sectionDto.Descriptions?.Count ?? 0) +
            (sectionDto.ImageIds?.Count ?? 0);

        var contents = new List<ProgramSectionContent>(capacity);
        var order = 0;

        foreach (var title in sectionDto.Titles ?? [])
        {
            contents.Add(new TitleProgramContent
            {
                ContentType = ContentType.Title,
                Order = order++,
                Title = title.Trim()
            });
        }

        foreach (var description in sectionDto.Descriptions ?? [])
        {
            contents.Add(new DescriptionProgramContent
            {
                ContentType = ContentType.Description,
                Order = order++,
                Description = description.Trim()
            });
        }

        foreach (var imageId in sectionDto.ImageIds ?? [])
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

        return contents;
    }
}
