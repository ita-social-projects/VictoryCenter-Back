using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

internal class HippotherapyLandingPageEthicsPrincipleConfig : IEntityTypeConfiguration<HippotherapyLandingPageEthicsPrinciple>
{
    public void Configure(EntityTypeBuilder<HippotherapyLandingPageEthicsPrinciple> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.EthicsSectionId)
            .IsRequired();

        entity.Property(e => e.Text)
            .IsRequired();

        entity.Property(e => e.Priority)
            .IsRequired();

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.HasIndex(e => new { e.EthicsSectionId, e.Priority })
            .IsUnique();
    }
}
