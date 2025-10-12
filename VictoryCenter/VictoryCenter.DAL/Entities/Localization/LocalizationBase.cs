namespace VictoryCenter.DAL.Entities.Localization;

public abstract class LocalizationBase<T>
    where T : class
{
    public long EntityId { get; set; }

    public long LanguageId { get; set; }

    public T Entity { get; set; } = null!;

    public LocalizationLanguage Language { get; set; } = null!;
}
