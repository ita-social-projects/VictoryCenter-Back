using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class PdfReportConfig : IEntityTypeConfiguration<PdfReport>
{
    public void Configure(EntityTypeBuilder<PdfReport> entity)
    {
        entity.ToTable("PdfReports", "media");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        entity.Property(e => e.BlobName)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(e => e.FileSizeBytes)
            .IsRequired();

        entity.Property(e => e.Priority)
            .IsRequired();

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.HasIndex(e => e.Priority)
            .IsUnique();
    }
}
