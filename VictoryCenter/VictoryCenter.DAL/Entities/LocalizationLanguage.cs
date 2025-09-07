namespace VictoryCenter.DAL.Entities;

public class LocalizationLanguage
{
    public long Id { get; set; }

    // ISO 639-1 is used for distinguishing languages
    public string Code { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
