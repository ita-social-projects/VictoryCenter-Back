namespace VictoryCenter.BLL.Constants;

public static class ContentLockConstants
{
    #pragma warning disable S2339
    public const string EventNewsResourceType = "EventNews";
    #pragma warning restore S2339
    public static readonly string LockIdHeader = "X-Content-Lock-Id";
    public static readonly string ConnectionIdHeader = "X-Content-Connection-Id";
    public static readonly string SessionItemKey = "ContentLockSession";
    public static readonly string ValidLockRequiredMessage = "A valid content lock is required for this update.";
    public static readonly string AuthenticatedAdministratorIdentityRequiredMessage = "Authenticated administrator identity is required.";
    public static readonly string UnsupportedResourceMessage = "The requested content lock resource is not supported.";
}
