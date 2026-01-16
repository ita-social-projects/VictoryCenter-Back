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
        var dtoContents = sectionDto.Contents ?? [];
        if (dtoContents.Count == 0)
        {
            return [];
        }

        var contents = new List<ProgramSectionContent>(dtoContents.Count);

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

    private static ProgramSectionContent? CreateContent(
        CreateProgramSectionContentDto dto,
        IReadOnlyDictionary<long, Image> imagesById)
    {
        if (dto.ContentType == ContentType.Title)
        {
            return new TitleProgramContent
            {
                ContentType = ContentType.Title,
                Order = dto.Order,
                GroupIndex = dto.GroupIndex,
                Title = dto.Title!.Trim()
            };
        }

        if (dto.ContentType == ContentType.Description)
        {
            return new DescriptionProgramContent
            {
                ContentType = ContentType.Description,
                Order = dto.Order,
                GroupIndex = dto.GroupIndex,
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

            return new ImageProgramContent
            {
                ContentType = ContentType.Image,
                Order = dto.Order,
                GroupIndex = dto.GroupIndex,
                ImageId = dto.ImageId.Value,
                Image = image
            };
        }

        if (dto.ContentType == ContentType.Author)
        {
            return new AuthorProgramContent
            {
                ContentType = ContentType.Author,
                Order = dto.Order,
                GroupIndex = dto.GroupIndex,
                Name = dto.Author!.Trim()
            };
        }

        return null;
    }
}
