namespace VictoryCenter.DAL.Entities.Localization;

public class PdfSectionLocalization : LocalizationBase<PdfSection>
{
    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;
}
