using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace VictoryCenter.DAL.Entities;

[Table("images", Schema = "media")]
public class Image
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string BlobName { get; set; } = null!;

    [NotMapped]
    public string? Base64 { get; set; }

    [Required]
    [MaxLength(10)]
    public string MimeType { get; set; } = null!;
}
