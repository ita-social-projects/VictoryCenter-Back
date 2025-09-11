using System.ComponentModel.DataAnnotations.Schema;
using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

[Table("images", Schema = "media")]
public class Image : IBaseEntity
{
    public long Id { get; set; }

    public string BlobName { get; set; } = null!;

    public string? Url { get; set; }

    public string MimeType { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
