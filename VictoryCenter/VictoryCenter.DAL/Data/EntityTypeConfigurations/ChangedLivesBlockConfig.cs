using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;
internal class ChangedLivesBlockConfig : IEntityTypeConfiguration<ChangedLivesBlock>
{
    public void Configure(EntityTypeBuilder<ChangedLivesBlock> entity)
    {
        entity
            .HasKey(x => x.Id);

        entity
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity
            .Property(e => e.Title)
            .IsRequired();

        entity
            .Property(e => e.ChangedLivesCount)
            .IsRequired();

        entity
            .Property(e => e.ImageId);

        entity
            .HasOne(e => e.Image)
            .WithOne()
            .HasForeignKey<ChangedLivesBlock>(e => e.ImageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
