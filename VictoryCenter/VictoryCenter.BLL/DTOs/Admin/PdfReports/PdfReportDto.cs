namespace VictoryCenter.BLL.DTOs.Admin.PdfReports;

public record PdfReportDto
{
    public long Id { get; init; }
    public required string Name { get; init; }
    public required string BlobName { get; init; }
    public long FileSizeBytes { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public long Priority { get; init; }
    public long? LanguageId { get; init; }
    public string? LanguageCode { get; init; }
}
