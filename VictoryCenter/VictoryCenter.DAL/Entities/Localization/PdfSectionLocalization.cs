using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class PdfSectionLocalization : LocalizationBase<PdfSection>
{
    [Audited]
    public string Title { get; set; } = null!;

    [Audited]
    public string Description { get; set; } = null!;
}
