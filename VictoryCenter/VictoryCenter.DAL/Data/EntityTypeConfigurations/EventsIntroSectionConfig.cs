using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

internal class EventsIntroSectionConfig : IEntityTypeConfiguration<EventsIntroSection>
{
    public void Configure(EntityTypeBuilder<EventsIntroSection> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.Property(e => e.EventsBlockTitle)
            .IsRequired();

        entity.Property(e => e.PageDescription)
            .IsRequired();
    }
}
