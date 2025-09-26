using System.ComponentModel.DataAnnotations.Schema;
using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

[Table("images", Schema = "media")]
public class Image : BaseEntity
{
    public string BlobName { get; set; } = null!;

    public string? Url { get; set; }

    public string MimeType { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }
}
