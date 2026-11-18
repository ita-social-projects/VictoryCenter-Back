using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using VictoryCenter.BLL.Interfaces.Audit;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Services.Audit;

public class AuditLogFactory : IAuditLogFactory
{
    private static readonly JsonSerializerOptions ChangesJsonOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TimeProvider _timeProvider;

    public AuditLogFactory(IHttpContextAccessor httpContextAccessor, TimeProvider timeProvider)
    {
        _httpContextAccessor = httpContextAccessor;
        _timeProvider = timeProvider;
    }

    public AuditLog Create(
        string entityType,
        string entityId,
        AuditOperation operation,
        IReadOnlyDictionary<string, AuditChange>? changes)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        return new AuditLog
        {
            ActorId = httpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier),
            Operation = operation,
            EntityType = entityType,
            EntityId = entityId,
            Timestamp = _timeProvider.GetUtcNow(),
            CorrelationId = httpContext?.TraceIdentifier,
            Changes = changes is { Count: > 0 } ? JsonSerializer.Serialize(changes, ChangesJsonOptions) : null,
        };
    }
}
