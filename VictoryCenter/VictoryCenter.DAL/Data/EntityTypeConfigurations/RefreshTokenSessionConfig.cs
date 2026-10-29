using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class RefreshTokenSessionConfig : IEntityTypeConfiguration<RefreshTokenSession>
{
    public void Configure(EntityTypeBuilder<RefreshTokenSession> entity)
    {
        entity.ToTable("RefreshTokenSessions");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.TokenHash)
            .IsRequired()
            .HasMaxLength(128);

        entity.Property(e => e.RevocationReason)
            .HasMaxLength(256);

        entity.Property(e => e.ReplacedByTokenHash)
            .HasMaxLength(128);

        entity.Property(e => e.ExpiresAt)
            .IsRequired();

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.Property(e => e.IsRevoked)
            .IsRequired();

        entity.HasIndex(e => e.TokenHash)
            .IsUnique();

        entity.HasIndex(e => e.FamilyId);

        entity.HasIndex(e => new { e.AdminUserId, e.IsRevoked });

        entity.HasOne(e => e.AdminUser)
            .WithMany(u => u.RefreshTokenSessions)
            .HasForeignKey(e => e.AdminUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
