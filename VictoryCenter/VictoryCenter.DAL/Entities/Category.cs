using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VictoryCenter.DAL.Entities;

[Table("categories")]
public class Category
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] 
    public string Title { get; set; } = null!;
    
    public string? Description { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
}
