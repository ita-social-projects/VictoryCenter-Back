using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.DAL.Repositories.Interfaces.RefreshTokenSessions;

public interface IRefreshTokenSessionRepository : IRepositoryBase<RefreshTokenSession>
{
    Task<RefreshTokenSession?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task<List<RefreshTokenSession>> GetActiveByFamilyIdAsync(Guid familyId, CancellationToken cancellationToken = default);

    Task<List<RefreshTokenSession>> GetActiveByAdminUserIdAsync(int adminUserId, CancellationToken cancellationToken = default);

    Task RevokeFamilyAsync(Guid familyId, string reason, DateTimeOffset revokedAt, CancellationToken cancellationToken = default);

    Task RevokeAdminSessionsAsync(int adminUserId, string reason, DateTimeOffset revokedAt, CancellationToken cancellationToken = default);
}
