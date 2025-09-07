using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class LocalizationLanguageConfig : IEntityTypeConfiguration<LocalizationLanguage>
{
    public void Configure(EntityTypeBuilder<LocalizationLanguage> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Code)
            .IsRequired();

        entity.HasIndex(e => e.Code)
            .IsUnique();
    }
}
