namespace VictoryCenter.BLL.Constants;

public static class ContentLockConstants
{
    public const string LockIdHeader = "X-Content-Lock-Id";
    public const string ConnectionIdHeader = "X-Content-Connection-Id";
    public const string EventNewsResourceType = "EventNews";
    public const string SessionItemKey = "ContentLockSession";
    public const string ValidLockRequiredMessage = "A valid content lock is required for this update.";
    public const string AuthenticatedAdministratorIdentityRequiredMessage = "Authenticated administrator identity is required.";
    public const string UnsupportedResourceMessage = "The requested content lock resource is not supported.";
}
