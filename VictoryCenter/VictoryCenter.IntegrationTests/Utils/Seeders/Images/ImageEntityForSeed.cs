namespace VictoryCenter.IntegrationTests.Utils.Seeders.Images;

public class ImageEntityForSeed
{
    public int Id { get; set; }
    public string BlobName { get; set; } = default!;
    public string MimeType { get; set; } = default!;
    public string Base64 { get; set; } = default!;
}
