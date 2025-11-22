using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class LocalizationLanguageConfig : IEntityTypeConfiguration<LocalizationLanguage>
{
    public void Configure(EntityTypeBuilder<LocalizationLanguage> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(2)
            .IsUnicode(false);

        entity.HasIndex(e => e.Code)
            .IsUnique();

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50);
    }
}
