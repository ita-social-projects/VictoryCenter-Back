using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class BackupReportProgramExpendituresRecordConfig : IEntityTypeConfiguration<BackupReportProgramExpendituresRecord>
{
    public void Configure(EntityTypeBuilder<BackupReportProgramExpendituresRecord> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.ReportingYear)
            .IsRequired();

        builder.Property(e => e.AmountUah)
            .HasPrecision(13, 2)
            .IsRequired();

        builder.Property(e => e.AmountUsd)
            .HasPrecision(13, 2)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.HasOne(e => e.HippotherapyProgramCategory)
            .WithMany()
            .HasForeignKey(e => e.HippotherapyProgramCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
