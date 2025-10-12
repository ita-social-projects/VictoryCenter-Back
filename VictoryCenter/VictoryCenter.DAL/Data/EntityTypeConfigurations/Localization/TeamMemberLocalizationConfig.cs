using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class TeamMemberLocalizationConfig : IEntityTypeConfiguration<TeamMemberLocalization>
{
    public void Configure(EntityTypeBuilder<TeamMemberLocalization> entity)
    {
        entity.HasKey(tl => new { tl.EntityId, tl.LanguageId });

        entity.HasOne(tl => tl.Entity)
            .WithMany(t => t.Localizations)
            .HasForeignKey(tl => tl.EntityId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.Property(e => e.FullName)
            .IsRequired();

        entity.Property(e => e.Description);
    }
}
