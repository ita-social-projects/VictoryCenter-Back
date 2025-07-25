using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class ImageConfig : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> entity)
    {
        entity.ToTable("Images", "media");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.BlobName)
            .IsRequired();

        entity.Property(e => e.MimeType)
            .IsRequired();

        entity.Property(e => e.CreatedAt)
            .IsRequired();
    }
}
