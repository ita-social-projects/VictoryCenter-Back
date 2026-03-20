using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class CompanyProfileRequisiteLocalizationConfig
    : EntityLocalizationConfig<CompanyProfileRequisiteLocalization, CompanyProfileRequisite>
{
    public override void Configure(EntityTypeBuilder<CompanyProfileRequisiteLocalization> entity)
    {
        base.Configure(entity);

        entity.HasIndex(e => e.LanguageId);

        entity.Property(e => e.Recipient)
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(e => e.Address)
            .HasMaxLength(100)
            .IsRequired();
    }
}
