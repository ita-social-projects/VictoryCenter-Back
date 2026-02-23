using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;
internal class CollectedFundsBlockConfig : IEntityTypeConfiguration<CollectedFundsBlock>
{
    public void Configure(EntityTypeBuilder<CollectedFundsBlock> entity)
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
            .Property(e => e.CollectedAmount)
            .IsRequired();

        entity
            .Property(e => e.ImageId);

        entity
            .HasOne(e => e.Image)
            .WithOne()
            .HasForeignKey<CollectedFundsBlock>(e => e.ImageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
