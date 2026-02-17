namespace VictoryCenter.BLL.Services.PdfStorage;

public sealed record PdfEnvironmentVariables
{
    public required string RootPath { get; set; }
    public required string PdfSubPath { get; init; }
    public string FullPath => Path.Combine(RootPath, PdfSubPath);
}
