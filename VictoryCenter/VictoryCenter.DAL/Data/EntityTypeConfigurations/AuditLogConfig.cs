using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class AuditLogConfig : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(a => a.Id);

        builder.ToTable("AuditLogs", "audit");

        builder.Property(a => a.ActorId).HasMaxLength(450);
        builder.Property(a => a.EntityType).HasMaxLength(200);
        builder.Property(a => a.EntityId).HasMaxLength(450);
        builder.Property(a => a.CorrelationId).HasMaxLength(100);

        builder.Property(a => a.Operation)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(a => a.Timestamp);

        builder.HasIndex(a => new { a.EntityType, a.EntityId, a.Timestamp });
    }
}
