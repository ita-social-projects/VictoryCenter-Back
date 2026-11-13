using System.ComponentModel.DataAnnotations.Schema;
using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities;

[Auditable]
[Table("images", Schema = "media")]
public class Image : BaseEntity
{
    [Audited]
    public string BlobName { get; set; } = null!;

    [Audited]
    public string? Url { get; set; }

    [Audited]
    public string MimeType { get; set; } = null!;

    public DateTimeOffset UpdatedAt { get; set; }

    public Guid StorageVersion { get; set; } = Guid.NewGuid();
}
