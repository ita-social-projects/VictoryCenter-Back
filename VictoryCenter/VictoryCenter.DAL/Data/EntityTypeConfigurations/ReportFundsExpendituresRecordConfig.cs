using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class ReportFundsExpendituresRecordConfig : BaseReportFundsExpendituresRecordConfig<ReportFundsExpendituresRecord>
{
    public override void Configure(EntityTypeBuilder<ReportFundsExpendituresRecord> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.HasOne(e => e.Category)
            .WithMany(e => e.Records)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
