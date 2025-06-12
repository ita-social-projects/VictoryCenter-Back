using System.ComponentModel.DataAnnotations;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.TeamMembers;

public record TeamMemberDto
{
    
    public required long Id { get; set; }
    
    public required string FirstName { get; set; } = null!;

    
    public required string LastName { get; set; } = null!;

    public string? MiddleName { get; set; }

    
    public required long CategoryId { get; set; }

    
    public required long Priority { get; set; }

    
    public required Status Status { get; set; }

    public string? Description { get; set; }

    public byte[]? Photo { get; set; }

    public string? Email { get; set; }
    
}
