namespace VictoryCenter.BLL.Services.BlobStorage;

public sealed record BlobEnvironmentVariables
{
    public required string RootPath { get; set; }
    public required string ImagesSubPath { get; set; }

    public string FullPath => Path.Combine(RootPath, ImagesSubPath);
}
