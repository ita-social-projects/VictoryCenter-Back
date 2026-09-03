using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class VideoReviewConfig : IEntityTypeConfiguration<VideoReview>
{
    public void Configure(EntityTypeBuilder<VideoReview> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Link)
            .IsRequired();

        builder.Property(e => e.IsArchived)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(e => e.ArchivedAt)
            .IsRequired(false);

        builder.Property(e => e.Priority)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();
    }
}
