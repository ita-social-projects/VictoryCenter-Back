using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Interfaces.Audit;

public interface IAuditLogFactory
{
    AuditLog Create(
        string entityType,
        string entityId,
        AuditOperation operation,
        IReadOnlyDictionary<string, AuditChange>? changes);
}
