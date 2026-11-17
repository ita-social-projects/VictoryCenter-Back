namespace VictoryCenter.BLL.Constants;

public static class ContentLockEvents
{
    public static readonly string Acquired = "LockAcquired";
    public static readonly string Denied = "LockDenied";
    public static readonly string Released = "LockReleased";
    public static readonly string Expired = "LockExpired";
    public static readonly string EntityChanged = "EntityChanged";
}
