using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class TeamMember
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    public string? MiddleName { get; set; }

    [Required]
    public long CategoryId { get; set; }

    [Required]
    public long Priority { get; set; }

    [Required]
    public Status Status { get; set; }

    public string? Description { get; set; }

    public byte[]? Photo { get; set; }

    public string? Email { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    public Category Category { get; set; } = default!;
}
