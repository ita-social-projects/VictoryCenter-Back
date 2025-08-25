using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class UahBankDetailsConfig : IEntityTypeConfiguration<UahBankDetails>
{
    public void Configure(EntityTypeBuilder<UahBankDetails> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.BankName)
            .HasMaxLength(200)
            .IsRequired();

        entity.Property(e => e.Recipient)
            .HasMaxLength(200)
            .IsRequired();

        entity.Property(e => e.Edrpou)
            .HasMaxLength(10)
            .IsRequired();

        entity.Property(e => e.Iban)
            .HasMaxLength(29)
            .IsRequired();

        entity.Property(e => e.PaymentPurpose)
            .HasMaxLength(500)
            .IsRequired();
    }
}
