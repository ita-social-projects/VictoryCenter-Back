using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class PdfReport : BaseEntity, IOrderableEntity
{
    [Audited]
    public string Name { get; set; } = null!;

    [Audited]
    public string BlobName { get; set; } = null!;

    [Audited]
    public long FileSizeBytes { get; set; }

    [Audited]
    public long Priority { get; set; }

    [Audited]
    public long LanguageId { get; set; }

    public LocalizationLanguage Language { get; set; } = null!;
}
