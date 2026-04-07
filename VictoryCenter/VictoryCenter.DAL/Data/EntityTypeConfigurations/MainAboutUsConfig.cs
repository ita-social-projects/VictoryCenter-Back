using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

internal class MainAboutUsConfig : IEntityTypeConfiguration<MainAboutUs>
{
    public void Configure(EntityTypeBuilder<MainAboutUs> entity)
    {
        entity
            .HasKey(e => e.Id);

        entity
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity
            .Property(e => e.Title)
            .IsRequired();

        entity
            .Property(e => e.Description)
            .IsRequired();

        entity
            .Property(e => e.MainPageId)
            .IsRequired();

        entity
            .Property(e => e.CreatedAt)
            .IsRequired();
    }
}
