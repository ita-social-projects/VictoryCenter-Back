using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

public class PdfReport : BaseEntity, IOrderableEntity
{
    public string Name { get; set; } = null!;
    public string BlobName { get; set; } = null!;
    public long FileSizeBytes { get; set; }
    public long Priority { get; set; }
    public long? LanguageId { get; set; }
    public LocalizationLanguage? Language { get; set; }
}
