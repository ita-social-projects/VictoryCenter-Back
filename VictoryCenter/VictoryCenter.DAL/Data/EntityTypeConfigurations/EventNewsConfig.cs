using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class EventNewsConfig : IEntityTypeConfiguration<EventNews>
{
    public void Configure(EntityTypeBuilder<EventNews> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Slug)
            .HasMaxLength(450);

        builder.HasIndex(e => e.Slug)
            .IsUnique()
            .HasFilter("[Slug] IS NOT NULL");

        builder.Property(e => e.Resource);

        builder.Property(e => e.PublishedAt);

        builder.Property(e => e.Status)
            .IsRequired();

        builder.Property(e => e.PreviewImageId);

        builder.Property(e => e.BackgroundImageId);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder
            .HasOne(e => e.PreviewImage)
            .WithOne()
            .HasForeignKey<EventNews>(e => e.PreviewImageId);

        builder
            .HasOne(e => e.BackgroundImage)
            .WithOne()
            .HasForeignKey<EventNews>(e => e.BackgroundImageId);

        builder.HasMany(e => e.Categories)
            .WithMany(e => e.EventsNews)
            .UsingEntity<Dictionary<string, object>>(
                "EventNewsEventNewsCategory",
                category => category
                    .HasOne<EventNewsCategory>()
                    .WithMany()
                    .HasForeignKey("CategoriesId")
                    .OnDelete(DeleteBehavior.Restrict),
                eventNews => eventNews
                    .HasOne<EventNews>()
                    .WithMany()
                    .HasForeignKey("EventsNewsId")
                    .OnDelete(DeleteBehavior.Cascade),
                join => join.ToTable("EventNewsEventNewsCategories"));
    }
}
