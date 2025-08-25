using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.AboutUsContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class AboutUsContentConfig : IEntityTypeConfiguration<AboutUsContent>
{
    public void Configure(EntityTypeBuilder<AboutUsContent> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.SectionId)
            .IsRequired();

        builder.Property(c => c.ContentType)
            .IsRequired();

        // TPH дискримінатор
        builder.HasDiscriminator<ContentType>(c => c.ContentType)
            .HasValue<CardContent>(ContentType.Card)
            .HasValue<DescriptionContent>(ContentType.Description)
            .HasValue<ImageContent>(ContentType.Image)
            .HasValue<TitleContent>(ContentType.Title);

        // FK для CardContent.Image
        builder.HasOne(c => (c as CardContent).Image)
            .WithMany()
            .HasForeignKey((c as CardContent).ImageId)
            .OnDelete(DeleteBehavior.SetNull);

        // Тут можна додати конфігурації для властивостей підкласів через Property, наприклад:
        builder.Property((c => (c as CardContent).Description));
        builder.Property((c => (c as CardContent).ImageId));

        builder.Property((c => (c as DescriptionContent).Description));
        builder.Property((c => (c as TitleContent).Title));
    }
}
