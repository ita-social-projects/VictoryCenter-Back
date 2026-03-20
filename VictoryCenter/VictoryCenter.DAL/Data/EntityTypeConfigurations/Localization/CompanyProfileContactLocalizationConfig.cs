using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class CompanyProfileContactLocalizationConfig
    : EntityLocalizationConfig<CompanyProfileContactLocalization, CompanyProfileContact>
{
    public override void Configure(EntityTypeBuilder<CompanyProfileContactLocalization> entity)
    {
        base.Configure(entity);

        entity.HasIndex(e => e.LanguageId);

        entity.Property(e => e.Address)
            .HasMaxLength(100);

        entity.Property(e => e.Motto)
            .HasMaxLength(200);
    }
}
