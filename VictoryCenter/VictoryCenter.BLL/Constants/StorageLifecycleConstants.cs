namespace VictoryCenter.BLL.Constants;

public static class StorageLifecycleConstants
{
    public const string SectionName = "StorageLifecycle";
    public const string RetryLimitReached = "RetryLimitReached";
    public const string FileStillReferenced = "FileStillReferenced";
    public const string CleanupNotSafe = "CleanupNotSafe";
    public const string InvalidRetryDelay = "The initial storage retry delay must not exceed the maximum delay.";
}
