using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities.Localization;

public class BackupReportFundsExpendituresSettingsLocalization
{
    public long EntityId { get; set; }
    public long LanguageId { get; set; }
    public string DisclaimerTitle { get; set; } = null!;
    public TranslationStatus TranslationStatus { get; set; } = TranslationStatus.Relevant;
    public DateTimeOffset CreatedAt { get; set; }

    public BackupReportFundsExpendituresSettings Entity { get; set; } = null!;
    public LocalizationLanguage Language { get; set; } = null!;
}
