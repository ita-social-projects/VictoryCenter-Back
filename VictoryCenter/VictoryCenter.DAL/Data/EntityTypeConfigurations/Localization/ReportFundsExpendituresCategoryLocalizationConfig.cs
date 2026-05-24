using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class ReportFundsExpendituresCategoryLocalizationConfig
    : EntityLocalizationConfig<ReportFundsExpendituresCategoryLocalization, ReportFundsExpendituresCategory>
{
    public override void Configure(EntityTypeBuilder<ReportFundsExpendituresCategoryLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Name)
            .IsRequired();
    }
}
