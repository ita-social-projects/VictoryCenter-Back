namespace VictoryCenter.IntegrationTests.Utils.Seeders.Images;

public class ImageEntityForSeed
{
    public int Id { get; set; }
    public string BlobName { get; set; } = null!;
    public string MimeType { get; set; } = null!;
    public string Base64 { get; set; } = null!;
}
