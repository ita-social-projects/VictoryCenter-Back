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
            .IsRequired();

        entity
            .Property(e => e.Story)
            .IsRequired();

        entity
            .HasOne(e => e.Image)
            .WithOne()
            .HasForeignKey<FeedbackHistory>(e => e.ImageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
