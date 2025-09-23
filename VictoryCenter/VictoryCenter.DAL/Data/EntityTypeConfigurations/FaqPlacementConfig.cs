using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class FaqPlacementConfig : IEntityTypeConfiguration<FaqPlacement>
{
    public void Configure(EntityTypeBuilder<FaqPlacement> entity)
    {
        entity
            .HasKey(e => new { e.PageId, e.QuestionId });

        entity
            .HasOne(e => e.Page)
            .WithMany(e => e.Questions)
            .HasForeignKey(e => e.PageId)
            .OnDelete(DeleteBehavior.Restrict);

        entity
            .HasOne(e => e.Question)
            .WithMany(e => e.Placements)
            .HasForeignKey(e => e.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        entity
            .HasIndex(e => new { e.PageId, e.Priority })
            .IsUnique();
    }
}
