using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Concurrency.WebSocket;
using VictoryCenter.BLL.Interfaces.Concurrency;
using VictoryCenter.BLL.Options;

namespace VictoryCenter.WebAPI.Filters;

public sealed class RequireContentLockAttribute : TypeFilterAttribute
{
    public RequireContentLockAttribute(string resourceType, string routeValueName)
        : base(typeof(RequireContentLockFilter))
    {
        Arguments = [resourceType, routeValueName];
    }
}

public sealed class RequireContentLockFilter : IAsyncActionFilter
{
    private readonly IContentLockManager _lockManager;
    private readonly IContentLockNotifier _notifier;
    private readonly ILogger<RequireContentLockFilter> _logger;
    private readonly ProblemDetailsFactory _problemDetailsFactory;
    private readonly ContentLockOptions _options;
    private readonly IContentLockResourcePolicy _resourcePolicy;
    private readonly string _resourceType;
    private readonly string _routeValueName;

    public RequireContentLockFilter(
        IContentLockManager lockManager,
        IContentLockNotifier notifier,
        ILogger<RequireContentLockFilter> logger,
        ProblemDetailsFactory problemDetailsFactory,
        IOptions<ContentLockOptions> options,
        IContentLockResourcePolicy resourcePolicy,
        string resourceType,
        string routeValueName)
    {
        _lockManager = lockManager;
        _notifier = notifier;
        _logger = logger;
        _problemDetailsFactory = problemDetailsFactory;
        _options = options.Value;
        _resourcePolicy = resourcePolicy;
        _resourceType = resourceType;
        _routeValueName = routeValueName;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        context.HttpContext.Response.Headers["X-Content-Lock-Enforced"] = _options.EnforcementEnabled.ToString().ToLowerInvariant();
        if (!_options.EnforcementEnabled)
        {
            await next();
            return;
        }

        if (long.TryParse(context.RouteData.Values.GetValueOrDefault(_routeValueName)?.ToString(), out var routeId)
            && routeId <= 0)
        {
            await next();
            return;
        }

        if (!TryGetSession(context, out var resource, out var session)
            || !_lockManager.TryBeginUpdate(session.LockId, resource, session.OwnerIdentity, session.ConnectionId))
        {
            context.Result = LockedResult(context.HttpContext);
            return;
        }

        context.HttpContext.Items[ContentLockConstants.SessionItemKey] = session;
        try
        {
            await next();
        }
        finally
        {
            var releasedLock = _lockManager.EndUpdate(session.LockId, resource, session.OwnerIdentity, session.ConnectionId);
            if (releasedLock is not null)
            {
                await NotifyLockReleasedSafelyAsync(releasedLock);
            }
        }
    }

    private bool TryGetSession(
        ActionExecutingContext context,
        out ContentLockResourceDto resource,
        out ContentLockSessionDto session)
    {
        resource = default!;
        session = default!;
        if (!long.TryParse(context.RouteData.Values.GetValueOrDefault(_routeValueName)?.ToString(), out var resourceId)
            || !context.HttpContext.Request.Headers.TryGetValue(ContentLockConstants.LockIdHeader, out var lockId)
            || !context.HttpContext.Request.Headers.TryGetValue(ContentLockConstants.ConnectionIdHeader, out var connectionId)
            || string.IsNullOrWhiteSpace(lockId)
            || string.IsNullOrWhiteSpace(connectionId)
            || string.IsNullOrWhiteSpace(context.HttpContext.User.FindFirstValue(ClaimTypes.Email)))
        {
            return false;
        }

        resource = new ContentLockResourceDto
        {
            ResourceType = _resourceType,
            ResourceId = resourceId,
        };
        if (!_resourcePolicy.IsSupported(resource))
        {
            return false;
        }

        session = new ContentLockSessionDto
        {
            LockId = lockId.ToString(),
            OwnerIdentity = context.HttpContext.User.FindFirstValue(ClaimTypes.Email)!,
            ConnectionId = connectionId.ToString(),
        };
        return true;
    }

    private ObjectResult LockedResult(HttpContext httpContext)
    {
        var details = _problemDetailsFactory.CreateProblemDetails(
            httpContext,
            statusCode: StatusCodes.Status423Locked,
            detail: ContentLockConstants.ValidLockRequiredMessage);
        return new ObjectResult(details) { StatusCode = StatusCodes.Status423Locked };
    }

    private async Task NotifyLockReleasedSafelyAsync(ContentLockLifecycleDto releasedLock)
    {
        try
        {
            await _notifier.NotifyLockReleasedAsync(releasedLock);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unable to publish a content-lock release notification after an update completed.");
        }
    }
}
