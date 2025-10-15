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
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Swift)
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(e => e.Account)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Iban)
            .HasMaxLength(34);

        builder.HasOne(e => e.ForeignBankDetails)
            .WithMany(e => e.CorrespondentBanks)
            .HasForeignKey(e => e.ForeignBankDetailsId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("CorrespondentBankDetails");
    }
}
