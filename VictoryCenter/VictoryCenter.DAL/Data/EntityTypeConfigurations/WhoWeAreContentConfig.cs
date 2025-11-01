using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class WhoWeAreContentConfig : IEntityTypeConfiguration<WhoWeAreContent>
{
    public void Configure(EntityTypeBuilder<WhoWeAreContent> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.SectionId)
            .IsRequired();

        builder.HasOne(c => c.Section)
            .WithMany(s => s.Contents)
            .HasForeignKey(c => c.SectionId);
        builder.Property(c => c.ContentType)
            .IsRequired();

        // TPH setup
        builder.HasDiscriminator<ContentType>(c => c.ContentType)
            .HasValue<CardContent>(ContentType.Card)
            .HasValue<DescriptionContent>(ContentType.Description)
            .HasValue<ImageContent>(ContentType.Image)
            .HasValue<TitleContent>(ContentType.Title);
    }
}

public class CardContentConfig : IEntityTypeConfiguration<CardContent>
{
    public void Configure(EntityTypeBuilder<CardContent> builder)
    {
        builder.Property(c => c.Description);

        builder.HasOne(c => c.Image)
            .WithMany()
            .HasForeignKey(c => c.ImageId);
    }
}

public class DescriptionContentConfig : IEntityTypeConfiguration<DescriptionContent>
{
    public void Configure(EntityTypeBuilder<DescriptionContent> builder)
    {
        builder.Property(c => c.Description);
    }
}

public class ImageContentConfig : IEntityTypeConfiguration<ImageContent>
{
    public void Configure(EntityTypeBuilder<ImageContent> builder)
    {
        builder.HasOne(c => c.Image)
            .WithMany()
            .HasForeignKey(c => c.ImageId);
    }
}

public class TitleContentConfig : IEntityTypeConfiguration<TitleContent>
{
    public void Configure(EntityTypeBuilder<TitleContent> builder)
    {
        builder.Property(c => c.Title);
    }
}
