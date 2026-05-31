using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class PdfSectionLocalizationConfig : EntityLocalizationConfig<PdfSectionLocalization, PdfSection>
{
    public override void Configure(EntityTypeBuilder<PdfSectionLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Title)
            .IsRequired();

        entity.Property(e => e.Description)
            .IsRequired();
    }
}
