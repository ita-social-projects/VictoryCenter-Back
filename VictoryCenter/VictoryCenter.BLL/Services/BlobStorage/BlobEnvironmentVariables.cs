namespace VictoryCenter.BLL.Services.BlobStorage;

public sealed record BlobEnvironmentVariables
{
    public required string RootPath { get; set; }
    public required string ImagesSubPath { get; init; }

    public string FullPath => Path.Combine(RootPath, ImagesSubPath);
}
