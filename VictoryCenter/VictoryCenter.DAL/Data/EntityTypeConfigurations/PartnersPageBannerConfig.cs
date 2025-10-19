using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

internal class PartnersPageBannerConfig : IEntityTypeConfiguration<PartnersPageBanner>
{
    public void Configure(EntityTypeBuilder<PartnersPageBanner> entity)
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
            .Property(e => e.Description);

        entity
            .Property(e => e.ImageId);

        entity
            .HasOne(e => e.Image)
            .WithOne()
            .HasForeignKey<PartnersPageBanner>(e => e.ImageId)
            .OnDelete(DeleteBehavior.SetNull);

        entity
            .Property(e => e.CreatedAt)
            .IsRequired();
    }
}
