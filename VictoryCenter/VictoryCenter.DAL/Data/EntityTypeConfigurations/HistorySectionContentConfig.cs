using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HistoryContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class HistorySectionContentConfig : IEntityTypeConfiguration<HistorySectionContent>
{
    public void Configure(EntityTypeBuilder<HistorySectionContent> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.SectionId)
            .IsRequired();

        builder.Property(c => c.ContentType)
            .IsRequired();

        builder.Property(c => c.Order)
            .IsRequired();

        builder.HasOne(c => c.Section)
            .WithMany(s => s.Contents)
            .HasForeignKey(c => c.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        // TPH setup
        builder.HasDiscriminator(c => c.ContentType)
            .HasValue<TitleHistoryContent>(ContentType.Title)
            .HasValue<DescriptionHistoryContent>(ContentType.Description)
            .HasValue<ImageHistoryContent>(ContentType.Image);
    }
}

public class TitleHistoryContentConfig : IEntityTypeConfiguration<TitleHistoryContent>
{
    public void Configure(EntityTypeBuilder<TitleHistoryContent> builder)
    {
        builder.Property(c => c.Title)
            .IsRequired();
    }
}

public class DescriptionHistoryContentConfig : IEntityTypeConfiguration<DescriptionHistoryContent>
{
    public void Configure(EntityTypeBuilder<DescriptionHistoryContent> builder)
    {
        builder.Property(c => c.Description)
            .IsRequired();
    }
}

public class ImageHistoryContentConfig : IEntityTypeConfiguration<ImageHistoryContent>
{
    public void Configure(EntityTypeBuilder<ImageHistoryContent> builder)
    {
        builder.Property(c => c.ImageId)
            .IsRequired();

        builder.HasOne(c => c.Image)
            .WithMany()
            .HasForeignKey(c => c.ImageId);
    }
}
