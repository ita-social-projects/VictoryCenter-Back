using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class PartnerSectionConfig : IEntityTypeConfiguration<PartnerSection>
{
    public void Configure(EntityTypeBuilder<PartnerSection> entity)
    {
        entity
            .HasKey(e => e.Id);

        entity
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity
            .Property(e => e.Description)
            .IsRequired();

        entity
            .Property(e => e.Title)
            .IsRequired();

        entity
            .Property(e => e.Priority)
            .IsRequired();

        entity
            .HasMany(x => x.Partners)
            .WithOne(x => x.PartnerSection)
            .HasForeignKey(x => x.PartnersSectionId);

        entity
            .Property(e => e.CreatedAt)
            .IsRequired();

        entity
            .HasIndex(e => e.Priority)
            .IsUnique();
    }
}
