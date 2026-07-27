using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public abstract class BaseReportFundsExpendituresRecordConfig<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseReportFundsExpendituresRecord
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Type)
            .IsRequired();

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
    }
}
