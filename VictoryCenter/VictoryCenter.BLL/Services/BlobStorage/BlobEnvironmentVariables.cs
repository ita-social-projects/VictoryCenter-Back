namespace VictoryCenter.BLL.Services.BlobStorage;
public sealed record BlobEnvironmentVariables
{
    required public string BlobStoreKey { get; init; }
    required public string BlobStorePath { get; init; }
}
