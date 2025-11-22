using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities.Localization;

public abstract class LocalizationBase<T>
    where T : class
{
    public long EntityId { get; set; }

    public long LanguageId { get; set; }

    public T Entity { get; set; } = null!;

    public TranslationStatus TranslationStatus { get; set; } = TranslationStatus.Relevant;

    public LocalizationInfo Language { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }
}
