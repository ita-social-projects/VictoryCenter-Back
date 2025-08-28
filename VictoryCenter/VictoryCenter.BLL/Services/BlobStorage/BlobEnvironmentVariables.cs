namespace VictoryCenter.BLL.Services.BlobStorage;

public sealed record BlobEnvironmentVariables
{
    public required string BlobStoreKey { get; init; }
    public required string BlobStorePath { get; init; }
}
