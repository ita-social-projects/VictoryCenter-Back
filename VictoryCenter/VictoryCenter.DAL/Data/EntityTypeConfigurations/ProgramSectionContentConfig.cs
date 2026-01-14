using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class ProgramSectionContentConfig : IEntityTypeConfiguration<ProgramSectionContent>
{
    public void Configure(EntityTypeBuilder<ProgramSectionContent> builder)
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

        builder.Property(c => c.GroupIndex);

        builder.HasOne(c => c.Section)
            .WithMany(s => s.Contents)
            .HasForeignKey(c => c.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        // TPH setup
        builder.HasDiscriminator(c => c.ContentType)
            .HasValue<TitleProgramContent>(ContentType.Title)
            .HasValue<DescriptionProgramContent>(ContentType.Description)
            .HasValue<ImageProgramContent>(ContentType.Image);
    }
}

public class TitleProgramContentConfig : IEntityTypeConfiguration<TitleProgramContent>
{
    public void Configure(EntityTypeBuilder<TitleProgramContent> builder)
    {
        builder.Property(c => c.Title)
            .IsRequired();
    }
}

public class DescriptionProgramContentConfig : IEntityTypeConfiguration<DescriptionProgramContent>
{
    public void Configure(EntityTypeBuilder<DescriptionProgramContent> builder)
    {
        builder.Property(c => c.Description)
            .IsRequired();
    }
}

public class ImageProgramContentConfig : IEntityTypeConfiguration<ImageProgramContent>
{
    public void Configure(EntityTypeBuilder<ImageProgramContent> builder)
    {
        builder.Property(c => c.ImageId)
            .IsRequired();

        builder.HasOne(c => c.Image)
            .WithMany()
            .HasForeignKey(c => c.ImageId);
    }
}
