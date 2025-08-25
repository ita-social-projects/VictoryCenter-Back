using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class CorrespondentBankConfig : IEntityTypeConfiguration<CorrespondentBank>
{
    public void Configure(EntityTypeBuilder<CorrespondentBank> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.BankName)
            .HasMaxLength(200);

        entity.Property(e => e.Account)
            .HasMaxLength(20);

        entity.Property(e => e.SwiftCode)
            .HasMaxLength(20);

        entity
            .HasOne(e => e.ForeignBankDetails)
            .WithMany(e => e.CorrespondentBanks)
            .HasForeignKey(e => e.ForeignBankDetailsId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
