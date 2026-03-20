using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class CompanyProfileRequisiteConfig : IEntityTypeConfiguration<CompanyProfileRequisite>
{
    public void Configure(EntityTypeBuilder<CompanyProfileRequisite> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.ProfileId)
            .IsRequired();

        builder.Property(e => e.Recipient)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Edrpou)
            .HasMaxLength(8)
            .IsRequired();

        builder.Property(e => e.Address)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(e => e.ProfileId)
            .IsUnique();

        builder.Property(e => e.CreatedAt)
            .IsRequired();
    }
}
