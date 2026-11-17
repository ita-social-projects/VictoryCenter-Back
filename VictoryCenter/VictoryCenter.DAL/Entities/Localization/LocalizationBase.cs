using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities.Localization;

[Auditable]
public abstract class LocalizationBase<T>
    where T : class
{
    [Audited]
    public long EntityId { get; set; }

    [Audited]
    public long LanguageId { get; set; }

    public T Entity { get; set; } = null!;

    [Audited]
    public TranslationStatus TranslationStatus { get; set; } = TranslationStatus.Relevant;

    public LocalizationLanguage Language { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }
}
