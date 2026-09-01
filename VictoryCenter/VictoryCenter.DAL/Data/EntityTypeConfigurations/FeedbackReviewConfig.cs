using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class FeedbackReviewConfig : IEntityTypeConfiguration<FeedbackReview>
{
    public void Configure(EntityTypeBuilder<FeedbackReview> builder)
    {
        builder.HasKey(review => review.Id);

        builder.Property(review => review.Id)
            .ValueGeneratedOnAdd();

        builder.Property(review => review.AuthorName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(review => review.Text)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(review => review.Status)
            .IsRequired();

        builder.Property(review => review.Priority)
            .IsRequired();

        builder.Property(review => review.CreatedAt)
            .IsRequired();

        builder.Property(review => review.Priority)
            .IsRequired();

        builder.HasIndex(review => review.Priority)
            .IsUnique();
    }
}
