using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.RefreshTokenSessions;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.RefreshTokenSessions;

public class RefreshTokenSessionRepository : RepositoryBase<RefreshTokenSession>, IRefreshTokenSessionRepository
{
    public RefreshTokenSessionRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }

    public async Task<RefreshTokenSession?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return await DbContext.RefreshTokenSessions
            .Include(s => s.AdminUser)
            .FirstOrDefaultAsync(s => s.TokenHash == tokenHash, cancellationToken);
    }

    public async Task<List<RefreshTokenSession>> GetActiveByFamilyIdAsync(Guid familyId, CancellationToken cancellationToken = default)
    {
        return await DbContext.RefreshTokenSessions
            .Where(s => s.FamilyId == familyId && !s.IsRevoked)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<RefreshTokenSession>> GetActiveByAdminUserIdAsync(int adminUserId, CancellationToken cancellationToken = default)
    {
        return await DbContext.RefreshTokenSessions
            .Where(s => s.AdminUserId == adminUserId && !s.IsRevoked)
            .ToListAsync(cancellationToken);
    }

    public async Task RevokeFamilyAsync(Guid familyId, string reason, DateTimeOffset revokedAt, CancellationToken cancellationToken = default)
    {
        var sessions = await DbContext.RefreshTokenSessions
            .Where(s => s.FamilyId == familyId && !s.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.IsRevoked = true;
            session.RevokedAt = revokedAt;
            session.RevocationReason = reason;
        }
    }

    public async Task RevokeAdminSessionsAsync(int adminUserId, string reason, DateTimeOffset revokedAt, CancellationToken cancellationToken = default)
    {
        var sessions = await DbContext.RefreshTokenSessions
            .Where(s => s.AdminUserId == adminUserId && !s.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.IsRevoked = true;
            session.RevokedAt = revokedAt;
            session.RevocationReason = reason;
        }
    }
}
