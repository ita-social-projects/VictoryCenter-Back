using System.Text.RegularExpressions;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.EventNews;

public record CreateEventNewsDto
{
    private string? _title;
    private string? _description;
    public string? Title
    {
        get { return _title; }
        init { _title = value != null ? NormalizeString(value) : null; }
    }

    public string? Description
    {
        get { return _description; }
        init { _description = value != null ? NormalizeString(value) : null; }
    }

    public string? Resource { get; init; }
    public DateTimeOffset? PublishedAt { get; init; }
    public Status Status { get; init; }
    public long? PreviewImageId { get; init; }
    public long? BackgroundImageId { get; init; }
    public List<long> CategoryIds { get; init; } = [];
    public List<CreateEventNewsLocalizationDto> Localizations { get; init; } = [];
    private string NormalizeString(string value) => Regex.Replace(value.Trim(), @"\s+", " ");
}
