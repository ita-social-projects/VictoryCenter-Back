using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class AdditionalFieldConfig : IEntityTypeConfiguration<AdditionalField>
{
    public void Configure(EntityTypeBuilder<AdditionalField> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.FieldName)
            .HasMaxLength(50);

        entity.Property(e => e.FieldValue)
            .HasMaxLength(100);

        entity
            .HasOne(e => e.UahBankDetails)
            .WithMany(e => e.AdditionalFields)
            .HasForeignKey(e => e.UahBankDetailsId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        entity
            .HasOne(e => e.ForeignBankDetails)
            .WithMany(e => e.AdditionalFields)
            .HasForeignKey(e => e.ForeignBankDetailsId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        entity
            .HasOne(e => e.SupportMethod)
            .WithMany(e => e.AdditionalFields)
            .HasForeignKey(e => e.SupportMethodId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
