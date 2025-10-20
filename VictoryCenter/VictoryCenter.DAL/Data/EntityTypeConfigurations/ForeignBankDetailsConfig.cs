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
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Receiver)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Iban)
            .IsRequired()
            .HasMaxLength(34);

        builder.Property(e => e.Swift)
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(e => e.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasMany(e => e.CorrespondentBanks)
            .WithOne(e => e.ForeignBankDetails)
            .HasForeignKey(e => e.ForeignBankDetailsId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("ForeignBankDetails");
    }
}
