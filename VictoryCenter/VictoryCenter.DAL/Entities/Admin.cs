using Microsoft.AspNetCore.Identity;

namespace VictoryCenter.DAL.Entities;

public class Admin : IdentityUser<int>
{
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenValidTo { get; set; }
    public DateTime CreatedAt { get; init; }
}
