using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

internal class MetricConfig : IEntityTypeConfiguration<Metric>
{
    public void Configure(EntityTypeBuilder<Metric> entity)
    {
        entity
            .HasKey(e => e.Id);

        entity
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity
            .Property(e => e.Value)
            .IsRequired();

        entity
            .Property(e => e.Signature)
            .IsRequired();

        entity
            .Property(e => e.StatisticId)
            .IsRequired();

        entity
            .Property(e => e.CreatedAt)
            .IsRequired();
    }
}
