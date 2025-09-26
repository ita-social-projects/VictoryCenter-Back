using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class VisitorPageConfig : IEntityTypeConfiguration<VisitorPage>
{
    public void Configure(EntityTypeBuilder<VisitorPage> entity)
    {
        entity
            .HasKey(e => e.Id);

        entity
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity
            .Property(e => e.Slug)
            .IsRequired();

        entity
            .HasIndex(e => e.Slug)
            .IsUnique();

        entity
            .Property(e => e.Title)
            .IsRequired();

        entity
            .HasIndex(e => e.Title)
            .IsUnique();

        entity
            .Property(e => e.CreatedAt)
            .IsRequired();
    }
}
