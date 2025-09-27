using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;
public class ForeignBankDetailsConfig : IEntityTypeConfiguration<ForeignBankDetails>
{
    public void Configure(EntityTypeBuilder<ForeignBankDetails> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .IsRequired();

        builder.Property(e => e.Receiver)
            .IsRequired();

        builder.Property(e => e.Iban)
            .IsRequired();

        builder.Property(e => e.Swift)
            .IsRequired();

        builder.Property(e => e.Address)
            .IsRequired();

        builder.HasMany(e => e.CorrespondentBanks)
            .WithOne(e => e.ForeignBankDetails)
            .HasForeignKey(e => e.ForeignBankDetailsId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("ForeignBankDetails");
    }
}
