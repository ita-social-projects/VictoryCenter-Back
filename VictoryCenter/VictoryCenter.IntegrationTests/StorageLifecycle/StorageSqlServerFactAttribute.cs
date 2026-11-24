namespace VictoryCenter.IntegrationTests.StorageLifecycle;

public sealed class StorageSqlServerFactAttribute : FactAttribute
{
    public const string ConnectionVariable = "VICTORYCENTER_STORAGE_SQL_TEST_CONNECTION";
    private const string CiVariable = "CI";

    public StorageSqlServerFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(ConnectionVariable))
            && string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(CiVariable)))
        {
            Skip = $"Set {ConnectionVariable} to a dedicated SQL Server with CREATE DATABASE permission. Tests create and drop isolated databases.";
        }
    }
}
