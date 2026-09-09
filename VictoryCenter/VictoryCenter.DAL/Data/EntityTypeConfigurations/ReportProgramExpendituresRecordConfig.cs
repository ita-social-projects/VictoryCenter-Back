using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class ReportProgramExpendituresRecordConfig : IEntityTypeConfiguration<ReportProgramExpendituresRecord>
{
    public void Configure(EntityTypeBuilder<ReportProgramExpendituresRecord> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

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
            .WithMany(e => e.ReportProgramExpendituresRecords)
            .HasForeignKey(e => e.HippotherapyProgramCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.HippotherapyProgramCategoryId)
            .IsUnique();
    }
}
