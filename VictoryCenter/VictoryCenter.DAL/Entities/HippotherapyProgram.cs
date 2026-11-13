using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class HippotherapyProgram : BaseEntity, ITranslatedEntity<HippotherapyProgramLocalization>
{
    [Audited]
    public string Name { get; set; } = null!;

    [Audited]
    public string? Slug { get; set; }

    [Audited]
    public string? Description { get; set; }

    [Audited]
    public Status Status { get; set; }

    [Audited]
    public string? Location { get; set; }

    [Audited]
    public string? ParticipantsCount { get; set; }

    [Audited]
    public string? MeetingsCount { get; set; }

    [Audited]
    public long? BackgroundImageId { get; set; }

    public Image? BackgroundImage { get; set; }

    [Audited]
    public long? PreviewImageId { get; set; }

    public Image? PreviewImage { get; set; }

    public ICollection<HippotherapyProgramCategory> Categories { get; set; } = [];

    public ICollection<HippotherapyProgramSection> Sections { get; set; } = [];

    public ICollection<HippotherapyProgramLocalization> Localizations { get; set; } = [];
}
