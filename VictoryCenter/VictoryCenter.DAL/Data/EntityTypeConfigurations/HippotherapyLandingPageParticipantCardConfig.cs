using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

internal class HippotherapyLandingPageParticipantCardConfig : IEntityTypeConfiguration<HippotherapyLandingPageParticipantCard>
{
    public void Configure(EntityTypeBuilder<HippotherapyLandingPageParticipantCard> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.ParticipantsSectionId)
            .IsRequired();

        entity.Property(e => e.Description)
            .IsRequired();

        entity.Property(e => e.ImageId);

        entity.HasOne(e => e.Image)
            .WithOne()
            .HasForeignKey<HippotherapyLandingPageParticipantCard>(e => e.ImageId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.Property(e => e.Priority)
            .IsRequired();

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.HasIndex(e => new { e.ParticipantsSectionId, e.Priority })
            .IsUnique();
    }
}
