using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class ReportFundsExpendituresSettingsConfig : IEntityTypeConfiguration<ReportFundsExpendituresSettings>
{
    private const long SingletonSettingsId = 1;

    public void Configure(EntityTypeBuilder<ReportFundsExpendituresSettings> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.DisclaimerTitle)
            .IsRequired();

        builder.Property(e => e.ExchangeRate)
            .HasPrecision(18, 6)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("CK_ReportFundsExpendituresSettings_SingletonId", $"Id = {SingletonSettingsId}");
        });
    }
}
