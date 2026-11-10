using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Repositories.Models;

public sealed class WhoWeAreSectionPreviewRow
{
    public long Id { get; init; }
    public SectionType SectionType { get; init; }
    public required string Title { get; init; }
    public long? LanguageId { get; init; }
    public TranslationStatus? TranslationStatus { get; init; }
}
