using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class WhoWeAreSectionConfig : IEntityTypeConfiguration<WhoWeAreSection>
{
    public void Configure(EntityTypeBuilder<WhoWeAreSection> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.SectionType)
            .IsRequired();

        entity.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        entity.Property(e => e.CreatedAt)
            .IsRequired(); // default in SQL Server

        entity.HasMany(e => e.Contents)
            .WithOne(c => c.Section)
            .HasForeignKey(c => c.SectionId);
    }
}
