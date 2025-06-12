using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.TeamMembers;

public record CreateTeamMemberDto
{
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }

    public string? MiddleName { get; set; }
    
    public required long CategoryId { get; set; } 
    
    public Status Status { get; set; }
    
    public string? Description { get; set; }
    
    public byte[]? Photo { get; set; }
    
    public string? Email { get; set; }
    
}
