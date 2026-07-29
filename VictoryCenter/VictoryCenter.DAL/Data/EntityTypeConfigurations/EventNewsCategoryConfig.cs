using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class EventNewsCategoryConfig : IEntityTypeConfiguration<EventNewsCategory>
{
    public void Configure(EntityTypeBuilder<EventNewsCategory> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(e => e.Name)
            .IsUnique();

        builder.Property(e => e.CreatedAt)
            .IsRequired();
    }
}
