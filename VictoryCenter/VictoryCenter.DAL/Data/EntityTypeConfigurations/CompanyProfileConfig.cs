using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class CompanyProfileConfig : IEntityTypeConfiguration<CompanyProfile>
{
    public void Configure(EntityTypeBuilder<CompanyProfile> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.HasOne(e => e.Contact)
            .WithOne(e => e.Profile)
            .HasForeignKey<CompanyProfileContact>(e => e.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Requisite)
            .WithOne(e => e.Profile)
            .HasForeignKey<CompanyProfileRequisite>(e => e.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.SocialLinks)
            .WithOne(e => e.Profile)
            .HasForeignKey(e => e.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.CreatedAt)
            .IsRequired();
    }
}
