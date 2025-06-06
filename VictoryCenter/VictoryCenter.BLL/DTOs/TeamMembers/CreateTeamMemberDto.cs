using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.TeamMembers;

public class CreateTeamMemberDto
{
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }

    public string? MiddleName { get; set; }
    
    public required int CategoryId { get; set; } 
    
    public Status Status { get; set; }
    
    public string? Description { get; set; }
    
    public byte[]? Photo { get; set; }
    
    public string? Email { get; set; }
    
}