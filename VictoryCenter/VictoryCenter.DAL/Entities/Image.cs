using System.ComponentModel.DataAnnotations.Schema;

namespace VictoryCenter.DAL.Entities;

[Table("images", Schema = "media")]
public class Image
{
    public long Id { get; set; }

    public string BlobName { get; set; } = null!;

    public string? Base64 { get; set; }

    public string MimeType { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
