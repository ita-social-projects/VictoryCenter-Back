using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class CompanyProfileSocialLinkConfig : IEntityTypeConfiguration<CompanyProfileSocialLink>
{
    public void Configure(EntityTypeBuilder<CompanyProfileSocialLink> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.ProfileId)
            .IsRequired();

        builder.Property(e => e.SocialPlatform)
            .IsRequired();

        builder.Property(e => e.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(e => new { e.ProfileId, e.SocialPlatform })
            .IsUnique();

        builder.Property(e => e.CreatedAt)
            .IsRequired();
    }
}
