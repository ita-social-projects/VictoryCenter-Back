using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Constants;

public static class WhoWeAreConstants
{
    public static readonly Dictionary<SectionType, (int MinLen, int MaxLen)> ValidationDescriptionRules = new()
    {
        { SectionType.Main, (10, 300) },
        { SectionType.WhatWeDo, (10, 300) },
        { SectionType.WhoWeSupport, (10, 300) },
        { SectionType.Team, (10, 360) },
        { SectionType.People, (10, 60) }
    };
    public static readonly (int MinLen, int MaxLen) ValidationTitleRules = new(10, 50);
    public static string WrongContentType => "Content has wrong content type";
    public static string ContentCanNotBeNull => "Content cannot be null";
    public static string EntityDoNotBelongToTheSection(Type entity, long sectionId)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        return $"Entity {entity.Name} does not belong to the section with id {sectionId}";
    }
}
