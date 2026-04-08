using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

internal class MainPageConfig : IEntityTypeConfiguration<MainPage>
{
    public void Configure(EntityTypeBuilder<MainPage> entity)
    {
        entity
            .HasKey(e => e.Id);

        entity
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity
            .Property(e => e.Title)
            .IsRequired();

        entity
            .Property(e => e.Description)
            .IsRequired();

        entity
            .Property(e => e.ImageId);

        entity
            .HasOne(e => e.Image)
            .WithOne()
            .HasForeignKey<MainPage>(e => e.ImageId)
            .OnDelete(DeleteBehavior.SetNull);

        entity
            .HasOne(e => e.MainAboutUs)
            .WithOne(e => e.MainPage)
            .HasForeignKey<MainAboutUs>(e => e.MainPageId)
            .OnDelete(DeleteBehavior.Cascade);

        entity
            .HasOne(e => e.MainPartners)
            .WithOne(e => e.MainPage)
            .HasForeignKey<MainPartners>(e => e.MainPageId)
            .OnDelete(DeleteBehavior.Cascade);

        entity
            .HasMany(e => e.ImpactStatistics)
            .WithOne(e => e.MainPage)
            .HasForeignKey(e => e.MainPageId)
            .OnDelete(DeleteBehavior.Cascade);

        entity
            .Property(e => e.CreatedAt)
            .IsRequired();
    }
}
