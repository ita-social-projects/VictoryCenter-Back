using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class CompanyProfileContactConfig : IEntityTypeConfiguration<CompanyProfileContact>
{
    public void Configure(EntityTypeBuilder<CompanyProfileContact> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.ProfileId)
            .IsRequired();

        builder.Property(e => e.Phone)
            .HasMaxLength(20);

        builder.Property(e => e.Address)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Email)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.CorrespondenceEmail)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Motto)
            .HasMaxLength(200);

        builder.HasIndex(e => e.ProfileId)
            .IsUnique();

        builder.Property(e => e.CreatedAt)
            .IsRequired();
    }
}
