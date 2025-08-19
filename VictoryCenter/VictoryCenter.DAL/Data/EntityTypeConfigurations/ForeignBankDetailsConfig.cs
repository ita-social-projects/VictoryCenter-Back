using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class ForeignBankDetailsConfig : IEntityTypeConfiguration<ForeignBankDetails>
{
    public void Configure(EntityTypeBuilder<ForeignBankDetails> entity)
    {
        entity.Property(e => e.BankName)
            .HasMaxLength(200)
            .IsRequired();

        entity.Property(e => e.Recipient)
            .HasMaxLength(200)
            .IsRequired();

        entity.Property(e => e.Iban)
            .HasMaxLength(29)
            .IsRequired();

        entity.Property(e => e.SwiftCode)
            .HasMaxLength(20)
            .IsRequired();

        entity.Property(e => e.Address)
            .HasMaxLength(200);
    }
}
