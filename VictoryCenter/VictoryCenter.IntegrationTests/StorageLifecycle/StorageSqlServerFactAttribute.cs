namespace VictoryCenter.IntegrationTests.StorageLifecycle;

public sealed class StorageSqlServerFactAttribute : FactAttribute
{
    public const string ConnectionVariable = "VICTORYCENTER_STORAGE_SQL_TEST_CONNECTION";

    public StorageSqlServerFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(ConnectionVariable)))
        {
            Skip = $"Set {ConnectionVariable} to a dedicated SQL Server with CREATE DATABASE permission. Tests create and drop isolated databases.";
        }
    }
}
