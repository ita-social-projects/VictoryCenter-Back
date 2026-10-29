using Microsoft.AspNetCore.Identity;

namespace VictoryCenter.DAL.Entities;

public class AdminUser : IdentityUser<int>
{
    public string? RefreshToken { get; set; }
    public DateTimeOffset? RefreshTokenValidTo { get; set; }
    public DateTimeOffset CreatedAt { get; init; }
    public ICollection<RefreshTokenSession> RefreshTokenSessions { get; set; } = [];
}
