using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class BackupReportFundsExpendituresSettingsConfig : IEntityTypeConfiguration<BackupReportFundsExpendituresSettings>
{
    public void Configure(EntityTypeBuilder<BackupReportFundsExpendituresSettings> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.DisclaimerTitle)
            .IsRequired();

        builder.Property(e => e.ExchangeRate)
            .HasPrecision(18, 6)
            .IsRequired();

        builder.Property(e => e.ProgramExpendituresReportingYear)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();
    }
}
