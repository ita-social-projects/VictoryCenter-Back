using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class HistorySectionConfig : IEntityTypeConfiguration<HistorySection>
{
    public void Configure(EntityTypeBuilder<HistorySection> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Template)
            .IsRequired();

        entity.Property(e => e.Order)
            .IsRequired();

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.HasMany(e => e.Contents)
            .WithOne(c => c.Section)
            .HasForeignKey(c => c.SectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
