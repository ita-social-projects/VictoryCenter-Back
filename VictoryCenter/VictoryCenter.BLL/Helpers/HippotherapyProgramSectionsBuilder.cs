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
                Contents = BuildContents(sectionDto, imagesById, createdAt)
            });
        }

        return result;
    }

    public static ProgramSectionContent? CreateContent(
        CreateProgramSectionContentDto dto,
        IReadOnlyDictionary<long, Image> imagesById,
        DateTimeOffset createdAt)
    {
        return dto.ContentType switch
        {
            ContentType.Title => CreateTitleContent(dto),
            ContentType.Description => CreateDescriptionContent(dto),
            ContentType.Image => CreateImageContent(dto, imagesById),
            ContentType.Author => CreateAuthorContent(dto),
            ContentType.FaqQuestion => CreateFaqQuestionContent(dto, createdAt),
            _ => null,
        };
    }

    private static TitleProgramContent CreateTitleContent(CreateProgramSectionContentDto dto) => new()
    {
        ContentType = ContentType.Title,
        Order = dto.Order,
        GroupIndex = dto.GroupIndex,
        Title = dto.Title!.Trim()
    };

    private static DescriptionProgramContent CreateDescriptionContent(CreateProgramSectionContentDto dto) => new()
    {
        ContentType = ContentType.Description,
        Order = dto.Order,
        GroupIndex = dto.GroupIndex,
        Description = dto.Description!.Trim()
    };

    private static ImageProgramContent? CreateImageContent(
        CreateProgramSectionContentDto dto,
        IReadOnlyDictionary<long, Image> imagesById)
    {
        if (dto.ImageId is null or <= 0 || !imagesById.TryGetValue(dto.ImageId.Value, out var image))
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

    private static AuthorProgramContent CreateAuthorContent(CreateProgramSectionContentDto dto) => new()
    {
        ContentType = ContentType.Author,
        Order = dto.Order,
        GroupIndex = dto.GroupIndex,
        Name = dto.Author!.Trim()
    };

    private static FaqQuestionProgramContent? CreateFaqQuestionContent(
        CreateProgramSectionContentDto dto,
        DateTimeOffset createdAt)
    {
        if (dto.FaqQuestion is null)
        {
            return null;
        }

        var content = new FaqQuestionProgramContent
        {
            ContentType = ContentType.FaqQuestion,
            Order = dto.Order,
            GroupIndex = dto.GroupIndex,
        };

        if (dto.FaqQuestion.Id is > 0)
        {
            content.FaqQuestionId = dto.FaqQuestion.Id.Value;
            return content;
        }

        if (string.IsNullOrWhiteSpace(dto.FaqQuestion.QuestionText) ||
            string.IsNullOrWhiteSpace(dto.FaqQuestion.AnswerText))
        {
            return null;
        }

        content.FaqQuestion = new FaqQuestion
        {
            QuestionText = dto.FaqQuestion.QuestionText.Trim(),
            AnswerText = dto.FaqQuestion.AnswerText.Trim(),
            Status = Status.Published,
            CreatedAt = createdAt
        };

        return content;
    }

    private static List<ProgramSectionContent> BuildContents(
        CreateHippotherapyProgramSectionDto sectionDto,
        IReadOnlyDictionary<long, Image> imagesById,
        DateTimeOffset createdAt)
    {
        var dtoContents = sectionDto.Contents ?? [];
        if (dtoContents.Count == 0)
        {
            return [];
        }

        var contents = new List<ProgramSectionContent>(dtoContents.Count);

        foreach (var dto in dtoContents.OrderBy(x => x.Order))
        {
            var entity = CreateContent(dto, imagesById, createdAt);
            if (entity is null)
            {
                continue;
            }

            contents.Add(entity);
        }

        return contents;
    }
}
