namespace VictoryCenter.BLL.Constants;

public static class StorageLifecycleConstants
{
    public static readonly string SectionName = "StorageLifecycle";
    public static readonly string RetryLimitReached = "RetryLimitReached";
    public static readonly string FileStillReferenced = "FileStillReferenced";
    public static readonly string CleanupNotSafe = "CleanupNotSafe";
    public static readonly string InvalidRetryDelay = "The initial storage retry delay must not exceed the maximum delay.";
}
