using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class BackupReportFundsExpendituresCategoryLocalizationConfig
    : IEntityTypeConfiguration<BackupReportFundsExpendituresCategoryLocalization>
{
    public void Configure(EntityTypeBuilder<BackupReportFundsExpendituresCategoryLocalization> entity)
    {
        entity.HasKey(e => new { e.EntityId, e.LanguageId });

        entity.Property(e => e.Name)
            .IsRequired();

        entity.Property(e => e.TranslationStatus)
            .IsRequired()
            .HasDefaultValue(TranslationStatus.Relevant);

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.HasOne(e => e.Entity)
            .WithMany(e => e.Localizations)
            .HasForeignKey(e => e.EntityId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.Language)
            .WithMany()
            .HasForeignKey(e => e.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
