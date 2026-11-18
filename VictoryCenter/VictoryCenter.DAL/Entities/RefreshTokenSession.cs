using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class RefreshTokenSession : BaseEntity
{
    public int AdminUserId { get; set; }

    public AdminUser AdminUser { get; set; } = null!;

    public Guid FamilyId { get; set; }

    public string TokenHash { get; set; } = null!;

    public DateTimeOffset ExpiresAt { get; set; }

    public bool IsRevoked { get; set; }

    public DateTimeOffset? RevokedAt { get; set; }

    public string? RevocationReason { get; set; }

    public string? ReplacedByTokenHash { get; set; }
}
