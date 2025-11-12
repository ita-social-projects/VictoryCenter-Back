using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class UahBankDetailsConfig : IEntityTypeConfiguration<UahBankDetails>
{
    public void Configure(EntityTypeBuilder<UahBankDetails> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Receiver)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Edrpou)
            .IsRequired()
            .HasMaxLength(8);

        builder.Property(e => e.Iban)
            .IsRequired()
            .HasMaxLength(29);

        builder.Property(e => e.PaymentPurpose)
            .IsRequired()
            .HasMaxLength(500);

        builder.ToTable("UahBankDetails");
    }
}
