using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;

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
            .Property(e => e.Name)
            .IsRequired();

        // Kind is derived from the legacy Type column so that the previous backend version, which only
        // knows Type, can still read and write metrics after a rollback. Old types: Raised = 2, others are Standard.
        entity
            .Property(e => e.Kind)
            .HasComputedColumnSql("CASE WHEN [Type] = 2 THEN 1 ELSE 0 END", stored: false);

        entity
            .Property<LegacyMetricType>(Metric.LegacyTypeProperty)
            .IsRequired();

        entity
            .Property(e => e.Prefix);

        entity
            .Property(e => e.IsAutoSynced)
            .IsRequired()
            .HasDefaultValue(false);

        entity
            .Property(e => e.StatisticId)
            .IsRequired();

        entity
            .Property(e => e.CreatedAt)
            .IsRequired();

        entity
            .Property(e => e.IsHidden)
            .IsRequired()
            .HasDefaultValue(false);

        entity
            .Property(e => e.Priority)
            .IsRequired();

        entity
            .Property(e => e.RowVersion)
            .IsRowVersion();
    }
}
