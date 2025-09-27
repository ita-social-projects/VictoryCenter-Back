using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;
public class CorrespondentBankDetailsConfig : IEntityTypeConfiguration<CorrespondentBankDetails>
{
    public void Configure(EntityTypeBuilder<CorrespondentBankDetails> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .IsRequired();

        builder.Property(e => e.Swift)
            .IsRequired();

        builder.Property(e => e.Account)
            .IsRequired();

        builder.Property(e => e.Iban);

        builder.HasOne(e => e.ForeignBankDetails)
            .WithMany(e => e.CorrespondentBanks)
            .HasForeignKey(e => e.ForeignBankDetailsId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("CorrespondentBankDetails");
    }
}
