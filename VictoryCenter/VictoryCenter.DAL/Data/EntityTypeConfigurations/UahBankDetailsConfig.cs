using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
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
            .IsRequired();

        builder.Property(e => e.Receiver)
            .IsRequired();

        builder.Property(e => e.Edrpou)
            .IsRequired();

        builder.Property(e => e.Iban)
            .IsRequired();

        builder.Property(e => e.PaymentPurpose)
            .IsRequired();

        builder.ToTable("UahBankDetails");
    }
}
