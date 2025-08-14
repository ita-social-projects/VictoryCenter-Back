using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class AdditionalFieldConfig : IEntityTypeConfiguration<AdditionalField>
{
    public void Configure(EntityTypeBuilder<AdditionalField> entity)
    {
        entity.Property(e => e.FieldName)
            .HasMaxLength(50);

        entity.Property(e => e.FieldValue)
            .HasMaxLength(100);

        entity
            .HasOne<UahBankDetails>()
            .WithMany(e => e.AdditionalFields)
            .HasForeignKey(e => e.UahBankDetailsId);

        entity
            .HasOne<ForeignBankDetails>()
            .WithMany(e => e.AdditionalFields)
            .HasForeignKey(e => e.ForeignBankDetailsId);

        entity
            .HasOne<SupportMethod>()
            .WithMany(e => e.AdditionalFields)
            .HasForeignKey(e => e.SupportMethodId);
    }
}
