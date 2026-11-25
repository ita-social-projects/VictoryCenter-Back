using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Repositories.Models;

public sealed class MainPageTranslationRow
{
    public long EntityId { get; init; }
    public long LanguageId { get; init; }
    public required string LanguageCode { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public TranslationStatus TranslationStatus { get; init; }
}
