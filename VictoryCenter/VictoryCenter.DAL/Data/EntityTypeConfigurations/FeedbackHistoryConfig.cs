using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class FeedbackHistoryConfig : IEntityTypeConfiguration<FeedbackHistory>
{
    public void Configure(EntityTypeBuilder<FeedbackHistory> entity)
    {
        entity
            .HasKey(e => e.Id);

        entity
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity
            .Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(50);

        entity
            .Property(e => e.Story)
            .IsRequired()
            .HasMaxLength(1000);

        entity
            .Property(e => e.Priority)
            .IsRequired();

        entity
            .Property(e => e.Status)
            .IsRequired();

        entity
            .HasOne(e => e.Image)
            .WithMany()
            .HasForeignKey(e => e.ImageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
