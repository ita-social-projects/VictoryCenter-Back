using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

internal class ImpactStatisticsConfig : IEntityTypeConfiguration<ImpactStatistics>
{
    public void Configure(EntityTypeBuilder<ImpactStatistics> entity)
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
            .Property(e => e.ImageId);

        entity
            .HasOne(e => e.Image)
            .WithOne()
            .HasForeignKey<ImpactStatistics>(e => e.ImageId)
            .OnDelete(DeleteBehavior.SetNull);

        entity
            .Property(e => e.MainPageId)
            .IsRequired();

        entity
            .HasMany(e => e.Metrics)
            .WithOne(e => e.Statistics)
            .HasForeignKey(e => e.StatisticId)
            .OnDelete(DeleteBehavior.Cascade);

        entity
            .Property(e => e.CreatedAt)
            .IsRequired();
    }
}
