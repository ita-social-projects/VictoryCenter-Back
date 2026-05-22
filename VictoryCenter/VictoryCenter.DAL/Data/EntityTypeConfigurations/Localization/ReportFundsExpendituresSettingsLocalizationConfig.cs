using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class ReportFundsExpendituresSettingsLocalizationConfig
    : EntityLocalizationConfig<ReportFundsExpendituresSettingsLocalization, ReportFundsExpendituresSettings>
{
    public override void Configure(EntityTypeBuilder<ReportFundsExpendituresSettingsLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.DisclaimerTitle)
            .HasMaxLength(1000)
            .IsRequired();
    }
}
