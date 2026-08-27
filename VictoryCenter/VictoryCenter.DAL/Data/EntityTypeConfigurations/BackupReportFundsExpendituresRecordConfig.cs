using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class BackupReportFundsExpendituresRecordConfig : BaseReportFundsExpendituresRecordConfig<BackupReportFundsExpendituresRecord>
{
    public override void Configure(EntityTypeBuilder<BackupReportFundsExpendituresRecord> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();
    }
}
