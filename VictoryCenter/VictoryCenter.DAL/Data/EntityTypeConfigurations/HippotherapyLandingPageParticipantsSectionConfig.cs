using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

internal class HippotherapyLandingPageParticipantsSectionConfig : IEntityTypeConfiguration<HippotherapyLandingPageParticipantsSection>
{
    public void Configure(EntityTypeBuilder<HippotherapyLandingPageParticipantsSection> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.HippotherapyLandingPageId)
            .IsRequired();

        entity.Property(e => e.Title)
            .IsRequired();

        entity.HasMany(e => e.ParticipantCards)
            .WithOne(c => c.ParticipantsSection)
            .HasForeignKey(c => c.ParticipantsSectionId);

        entity.Property(e => e.CreatedAt)
            .IsRequired();
    }
}
